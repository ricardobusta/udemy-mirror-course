using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using Mirror;
using Units;
using UnityEngine;

namespace Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        public static readonly Color[] TeamColors = {
            new Color(0.9f, 0.1f,0.1f), 
            new Color(0.1f, 0.1f, 0.9f), 
            new Color(0.1f, 0.9f, 0.1f), 
            new Color(0.9f, 0.8f, 0.1f)
        };
        public static readonly Color NeutralColor = new Color(0.5f,0.3f,0.3f);

        [SerializeField] private int initialResources;
        [SerializeField] private LayerMask buildingBlockLayer;
        [SerializeField] private float buildingRangeLimit;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private int maxUnitBuildLimit;

        public static event Action<RtsPlayer, string> OnMessageReceived;
        
        public List<Unit> MyUnits { get; } = new List<Unit>();
        public List<Building> MyBuildings { get; } = new List<Building>();

        [field: SyncVar(hook = nameof(ClientHandleTeamColorUpdated))]
        public int TeamColor { get; private set; }

        private Material _playerMaterial;
        
        [field: SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        public int Resources { get; private set; }
        
        public static Dictionary<int, Building> buildingMap;

        public event Action<int> ClientOnResourcesUpdated;

        public static event Action ClientOnInfoUpdated;
        public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

        public static event Action<int, int> ClientOnUnitCountUpdated;

        public Transform CameraTransform => cameraTransform;

        [field: SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
        public bool IsPartyOwner { get; private set; }

        [field: SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        public string DisplayName { get; private set; }

        [field: SyncVar(hook = nameof(OnPlayerUnitCountUpdated))]
        public int UnitCount { get; private set; }
        
        [field: SyncVar(hook = nameof(OnPlayerBuildLimitUpdated))]
        public int UnitBuildLimit { get; private set; }

        public int MaxUnitBuildLimit => Mathf.Min(UnitBuildLimit, maxUnitBuildLimit);

        public static string localDisplayName;

        #region Server

        public override void OnStartServer()
        {
            Unit.OnServerUnitSpawned += ServerHandleUnitSpawned;
            Unit.OnServerUnitDespawned += ServerHandleUnitDespawned;
            Building.OnServerBuildingSpawned += ServerHandleBuildingSpawned;
            Building.OnServerBuildingDespawned += ServerHandleBuildingDespawned;

            UnitCount = 0;
            Resources = initialResources;
            
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopServer()
        {
            Unit.OnServerUnitSpawned -= ServerHandleUnitSpawned;
            Unit.OnServerUnitDespawned -= ServerHandleUnitDespawned;
            Building.OnServerBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.OnServerBuildingDespawned -= ServerHandleBuildingDespawned;
        }

        public override void OnStartAuthority()
        {
            if (NetworkServer.active)
            {
                return;
            }

            Unit.OnAuthorityUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.OnAuthorityUnitDespawned += AuthorityHandleUnitDespawned;
            Building.OnAuthorityBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.OnAuthorityBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStartClient()
        {
            if (NetworkServer.active)
            {
                return;
            }

            DontDestroyOnLoad(gameObject);
            
            RtsNetworkManager.RtsSingleton.Players.Add(this);
        }

        public override void OnStopClient()
        {
            ClientOnInfoUpdated?.Invoke();
            
            if (!isClientOnly)
            {
                return;
            }
            
            RtsNetworkManager.RtsSingleton.Players.Remove(this);

            if (!hasAuthority)
            {
                return;
            }

            Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.OnAuthorityUnitDespawned -= AuthorityHandleUnitDespawned;
            Building.OnAuthorityBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.OnAuthorityBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
        {
            if (!buildingMap.TryGetValue(buildingId, out var buildingToPlace))
            {
                return;
            }
            
            if (!CanPlaceBuilding(buildingToPlace.GetComponent<BoxCollider>(), position, buildingToPlace.Price))
            {
                return;
            }

            var buildingInstance =
                Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);
            NetworkServer.Spawn(buildingInstance, connectionToClient);

            AddResources(-buildingToPlace.Price);
        }

        [Command]
        public void CmdStartGame()
        {
            if (!IsPartyOwner)
            {
                return;
            }
            
            RtsNetworkManager.RtsSingleton.StartGame();
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (!BelongsToPlayer(unit))
            {
                return;
            }

            MyUnits.Add(unit);
            UnitCount++;
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (!BelongsToPlayer(unit))
            {
                return;
            }

            MyUnits.Remove(unit);
            UnitCount--;
        }

        private void ServerHandleBuildingSpawned(Building building)
        {
            if (!BelongsToPlayer(building))
            {
                return;
            }

            MyBuildings.Add(building);
            UnitBuildLimit += building.BuildLimit;
        }

        private void ServerHandleBuildingDespawned(Building building)
        {
            if (!BelongsToPlayer(building))
            {
                return;
            }

            MyBuildings.Remove(building);
            UnitBuildLimit -= building.BuildLimit;
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            MyUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            MyUnits.Remove(unit);
        }

        private void AuthorityHandleBuildingSpawned(Building building)
        {
            MyBuildings.Add(building);
        }

        private void AuthorityHandleBuildingDespawned(Building building)
        {
            MyBuildings.Remove(building);
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
        {
            if (!hasAuthority)
            {
                return;
            }

            AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
        }

        private bool BelongsToPlayer(NetworkBehaviour entity)
        {
            return entity.connectionToClient != null && entity.connectionToClient.connectionId == connectionToClient.connectionId;
        }

        [Command]
        private void CmdSetDisplayName(string newName)
        {
            SetDisplayName(newName);
        } 

        [Server]
        public void SetDisplayName(string newName)
        {
            DisplayName = newName;
        }

        [Server]
        public void SetTeamColor(int newTeamColor)
        {
            TeamColor = newTeamColor;
        }

        [Server]
        public void SetResources(int resources)
        {
            Resources = resources;
        }

        [Server]
        public void AddResources(int resources)
        {
            Resources += resources;
        }

        [Server]
        public void SetPartyOwner(bool state)
        {
            IsPartyOwner = state;
        }
        
        [Command]
        public void CommandSendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                RpcReceiveMessage(message.Trim());
            }
        }

        #endregion Server

        #region Client
        
        [ClientRpc]
        public void RpcReceiveMessage(string message)
        {
            OnMessageReceived?.Invoke(this, message);
        }

        private void ClientHandleResourcesUpdated(int oldResources, int newResources)
        {
            ClientOnResourcesUpdated?.Invoke(newResources);
        }

        private void ClientHandleDisplayNameUpdated(string oldName, string newName)
        {
            ClientOnInfoUpdated?.Invoke();
        }

        private void ClientHandleTeamColorUpdated(int oldColor, int newColor)
        {
            ClientOnInfoUpdated?.Invoke();
        }

        [ClientRpc]
        public void GetClientDisplayName()
        {
            if (!hasAuthority)
            {
                return;
            }
            CmdSetDisplayName(localDisplayName);
        }

        public void OnPlayerUnitCountUpdated(int _, int newUnitCount)
        {
            ClientOnUnitCountUpdated?.Invoke(newUnitCount, MaxUnitBuildLimit);
        }

        public void OnPlayerBuildLimitUpdated(int _, int newBuildLimit)
        {
            ClientOnUnitCountUpdated?.Invoke(UnitCount, Mathf.Min(newBuildLimit, maxUnitBuildLimit));
        }

        #endregion Client
        
        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 position, int price)
        {
            if (Resources < price)
            {
                return false;
            }

            if (Physics.CheckBox(position + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity,
                buildingBlockLayer))
            {
                return false;
            }

            if (!MyBuildings.Any(building =>
                (position - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit))
            {
                return false;
            }

            return true;
        }
    }
}