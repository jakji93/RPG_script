using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] float dwellingTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float tollorenceDistance = 1f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        Fighter myFighter;
        GameObject player;
        Health myHealth;
        Mover myMover;

        LazyValue<Vector3> guardLocation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            myFighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            myHealth = GetComponent<Health>();
            myMover = GetComponent<Mover>();
            guardLocation = new LazyValue<Vector3>(GetInitialPosition);
        }

        private Vector3 GetInitialPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            guardLocation.ForceInit();
        }

        private void Update()
        {
            if (myHealth.GetAlreadyDead())
            {
                return;
            }
            if (InAttackRangeOfPlayer() && myFighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardLocation.value;
            if (patrolPath)
            {
                if (AtWaypolint())
                {
                    timeSinceAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentPosition();
            }
            myFighter.Cancel();
            if (timeSinceAtWaypoint > dwellingTime)
            {
                myMover.StartMoveAction(nextPosition, patrolSpeedFraction); 
            }
        }

        private bool AtWaypolint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentPosition());
            return distanceToWaypoint < tollorenceDistance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentPosition()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            myFighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            return Vector3.Distance(transform.position, player.transform.position) <= chaseDistance;
        }


        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    } 
}
