using System.Collections.Generic;
using Buildings;
using Mirror;
using Units;
using UnityEngine;

namespace Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        public List<Unit> MyUnits { get; } = new List<Unit>();
        public List<Building> MyBuildings { get; } = new List<Building>();

        private Dictionary<int, Building> _buildingMap;
        
        #region Server
        
        public override void OnStartServer()
        {
            Unit.OnServerUnitSpawned += ServerHandleUnitSpawned;
            Unit.OnServerUnitDespawned += ServerHandleUnitDespawned;
            Building.OnServerBuildingSpawned += ServerHandleBuildingSpawned;
            Building.OnServerBuildingDespawned += ServerHandleBuildingDespawned;
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

        public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
        {
            if(!_buildingMap.TryGetValue(buildingId, out var buildingToPlace))
            {
                return;
            }

            var buildingInstance = Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);
            NetworkServer.Spawn(buildingInstance, connectionToClient);
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
        
        #endregion Server

        #region Client

        public void SetBuildingMap(Dictionary<int, Building> buildingMap)
        {
            _buildingMap = buildingMap;
        }

        #endregion EndRegion
    }
}