using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Rts.Networking
{
    public class UnitMovement: NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent agent;

        private Camera _mainCamera;
        
        #region Server

        [Command]
        private void CmdMove(Vector3 position)
        {
            if (!NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas))
            {
                return; // Do nothing if invalid position
            }

            agent.SetDestination(hit.position);
        }
        
        #endregion Server
        
        #region Client

        public override void OnStartAuthority()
        {
            _mainCamera = Camera.main;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Mouse.current.rightButton.wasPressedThisFrame)
            {
                return; 
            }

            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity))
            {
                return; 
            }
            
            CmdMove(hit.point);
        }
        
        #endregion Client
    }
}