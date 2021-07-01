﻿using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class UnitMovement: NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        
        #region Server

        [Command]
        public void CmdMove(Vector3 position)
        {
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