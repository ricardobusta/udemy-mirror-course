using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        public Targetable Target { get; private set; }
        public bool HasTarget => Target != null;

        #region Server

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

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
        }

        [Server]
        public void ClearTarget()
        {
            Target = null;
        }

        [Server]
        private void ServerHandleGameOver()
        {
            
        }

        #endregion Server

        #region Client

        #endregion Client
    }
}