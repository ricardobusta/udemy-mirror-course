using Buildings;
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
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip movingClip;
        [SerializeField] private AudioClip idleClip;

        [SyncVar(hook = nameof(OnMovingStateChanged))] public bool moving;

        #region Server

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            if (targeter.HasTarget)
            {
                var target = targeter.Target;
                var targetTransform = target.transform;
                if ((targetTransform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    moving = true;
                    agent.SetDestination(target.transform.position);
                }
                else if (agent.hasPath)
                {
                    moving = false;
                    agent.ResetPath();
                }

                return;
            }

            if (!agent.hasPath)
            {
                moving = false;
                return;
            }
            
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                moving = true;
                return;
            }
            
            moving = false;
            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            ServerMove(position);
        }

        [Server]
        public void ServerMove(Vector3 position)
        {
            targeter.ClearTarget();

            if (!NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas))
            {
                return; // Do nothing if invalid position
            }

            agent.SetDestination(hit.position);
        }

        [Server]
        private void ServerHandleGameOver()
        {
            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        #endregion Server

        #region Client

        private void OnMovingStateChanged(bool _, bool newMoving)
        {
            audioSource.clip = newMoving ? movingClip : idleClip;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        
        #endregion Client
    }
}