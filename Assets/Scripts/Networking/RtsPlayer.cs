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
        [SerializeField] private int initialResources;
        [SerializeField] private LayerMask buildingBlockLayer;
        [SerializeField] private float buildingRangeLimit;

        public List<Unit> MyUnits { get; } = new List<Unit>();
        public List<Building> MyBuildings { get; } = new List<Building>();

        public static Dictionary<int, Building> buildingMap;

        [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        private int _resources;

        public event Action<int> ClientOnResourcesUpdated;

        public int Resources => _resources;

        #region Server

        public override void OnStartServer()
        {
            Unit.OnServerUnitSpawned += ServerHandleUnitSpawned;
            Unit.OnServerUnitDespawned += ServerHandleUnitDespawned;
            Building.OnServerBuildingSpawned += ServerHandleBuildingSpawned;
            Building.OnServerBuildingDespawned += ServerHandleBuildingDespawned;

            _resources = initialResources;
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

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority)
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

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (!BelongsToPlayer(unit))
            {
                return;
            }

            MyUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (!BelongsToPlayer(unit))
            {
                return;
            }

            MyUnits.Remove(unit);
        }

        private void ServerHandleBuildingSpawned(Building building)
        {
            if (!BelongsToPlayer(building))
            {
                return;
            }

            MyBuildings.Add(building);
        }

        private void ServerHandleBuildingDespawned(Building building)
        {
            if (!BelongsToPlayer(building))
            {
                return;
            }

            MyBuildings.Remove(building);
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

        private bool BelongsToPlayer(NetworkBehaviour entity)
        {
            return entity.connectionToClient.connectionId == connectionToClient.connectionId;
        }

        [Server]
        public void SetResources(int resources)
        {
            _resources = resources;
        }

        [Server]
        public void AddResources(int resources)
        {
            _resources += resources;
        }

        #endregion Server

        #region Client

        private void ClientHandleResourcesUpdated(int oldResources, int newResources)
        {
            ClientOnResourcesUpdated?.Invoke(newResources);
        }

        #endregion EndRegion
        
        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 position, int price)
        {
            if (_resources < price)
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