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
            if (!targetGameObject.TryGetComponent<Targetable>(out var newTarget))
            {
                return;
            }
            
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