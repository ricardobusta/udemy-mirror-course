using System;
using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Targeter targeter;
        [SerializeField] private float chaseRange;

        #region Server

        [ServerCallback]
        private void Update()
        {
            if (targeter.HasTarget)
            {
                var target = targeter.Target;
                var targetTransform = target.transform;
                if ((targetTransform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(target.transform.position);
                }
                else if(agent.hasPath)
                {
                    agent.ResetPath();
                }
                
                return;
            }
            
            if (!agent.hasPath || agent.remainingDistance > agent.stoppingDistance)
            {
                return;
            }

            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            targeter.ClearTarget();
            
            if (!NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas))
            {
                return; // Do nothing if invalid position
            }

            agent.SetDestination(hit.position);
        }

        #endregion Server

        #region Client

        #endregion Client
    }
}