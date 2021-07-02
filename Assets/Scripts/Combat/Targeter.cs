using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        public Targetable Target { get; private set; }
        public bool HasTarget { get; private set; }

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

            Target = newTarget;
            HasTarget = true;
        }

        [Server]
        public void ClearTarget()
        {
            Target = null;
            HasTarget = false;
        }

        #endregion Server

        #region Client

        #endregion Client
    }
}