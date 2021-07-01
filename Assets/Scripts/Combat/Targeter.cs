using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        [SerializeField] private Targetable target;
        
        #region Server
        
        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            Debug.Log("Trying to set target");
            if (!targetGameObject.TryGetComponent<Targetable>(out var newTarget))
            {
                Debug.Log("Invalid target");
                return;
            }

            Debug.Log($"Target found {targetGameObject.name}");
            target = newTarget;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }
        
        #endregion Server

        #region Client

        

        #endregion Client
    }
}