using Combat;
using Mirror;
using Networking;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Random = UnityEngine.Random;

namespace Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private float spawnDistance;
        [SerializeField] private float spawnMoveDistance;
        [SerializeField] private ProgressCounter progressCounter;
        [SerializeField] private int maxUnitQueue;
        [SerializeField] private float unitSpawnTime;

        [SyncVar(hook = nameof(OnQueueCountUpdate))]
        private int queuedUnits;

        [SyncVar(hook = nameof(OnProgressUpdate))]
        private float unitTimer;

        private Vector3? _rallyPoint;
        private RtsPlayer _player;
        private bool _playerSet;
        private EventSystem _eventSystem;
        
        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleOnDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleOnDie;
        }

        [Server]
        private void ProduceUnits()
        {
            if (queuedUnits == 0)
            {
                return;
            }

            unitTimer += Time.deltaTime;

            if (unitTimer < unitSpawnTime)
            {
                return;
            }

            var (spawnPos, rallyPos, rotation) = GetSpawnInfo();

            var unitInstance = Instantiate(unitPrefab, spawnPos, rotation);
            unitInstance.GetComponent<UnitMovement>().ServerMove(rallyPos);

            NetworkServer.Spawn(unitInstance.gameObject, connectionToClient);

            queuedUnits--;
            unitTimer = 0;
        }

        [Server]
        private void ServerHandleOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdQueueUnit()
        {
            if (queuedUnits >= maxUnitQueue)
            {
                return;
            }

            if (!_playerSet)
            {
                return;
            }
            
            if (_player.Resources < unitPrefab.ResourceCost)
            {
                return;
            }

            _player.AddResources(-unitPrefab.ResourceCost);
            queuedUnits++;
        }

        [Server]
        private (Vector3 spawnPosition, Vector3 rallyPosition, Quaternion rotation) GetSpawnInfo()
        {
            var spawnerPosition = transform.position;

            Vector3 rallyPosition;
            if (_rallyPoint.HasValue)
            {
                rallyPosition = _rallyPoint.Value;
            }
            else
            {
                var spawnAngle = Random.Range(0f, Mathf.PI * 2);
                rallyPosition = spawnerPosition + new Vector3(Mathf.Cos(spawnAngle) * spawnMoveDistance, 0,
                    Mathf.Sin(spawnAngle) * spawnDistance * 1.1f);
            }

            var spawnDirection = (rallyPosition - spawnerPosition).normalized;
            var spawnPosition = spawnerPosition + (spawnDirection * spawnDistance);
            var rotation = Quaternion.LookRotation(spawnDirection, Vector3.up);

            return (spawnPosition, rallyPosition, rotation);
        }
        
        [ServerCallback]
        private void Update()
        {
            ProduceUnits();
        }

        #endregion Server

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!hasAuthority || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            
            if (_eventSystem.IsPointerOverUIObject(eventData))
            {
                return;
            }

            CmdQueueUnit();
        }

        public void OnQueueCountUpdate(int oldValue, int newValue)
        {
            progressCounter.gameObject.SetActive(newValue != 0);
            progressCounter.SetCounter(newValue);
        }

        public void OnProgressUpdate(float oldValue, float newValue)
        {
            progressCounter.SetValue(newValue / unitSpawnTime);
        }

        #endregion Client

        private void Start()
        {
            _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            _playerSet = _player!=null;
            _eventSystem = EventSystem.current;
        }
    }
}