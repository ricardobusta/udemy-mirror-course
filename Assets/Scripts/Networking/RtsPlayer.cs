using System.Collections.Generic;
using Buildings;
using Mirror;
using Units;

namespace Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        public List<Unit> MyUnits { get; } = new List<Unit>();
        public List<Building> MyBuildings { get; } = new List<Building>();

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
    }
}