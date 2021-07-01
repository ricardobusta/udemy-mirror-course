using System.Collections.Generic;
using Mirror;
using Units;

namespace Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        public List<Unit> myUnits = new List<Unit>();

        public override void OnStartServer()
        {
            Unit.OnServerUnitSpawned += ServerHandleUnitSpawned;
            Unit.OnServerUnitDespawned += ServerHandleUnitDespawned;
        }

        public override void OnStopServer()
        {
            Unit.OnServerUnitSpawned -= ServerHandleUnitSpawned;
            Unit.OnServerUnitDespawned -= ServerHandleUnitDespawned;
        }
        
        public override void OnStartClient()
        {
            if (!isClientOnly)
            {
                return;
            }
            Unit.OnAuthorityUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.OnAuthorityUnitDespawned += AuthorityHandleUnitDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly)
            {
                return;
            }
            Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.OnAuthorityUnitDespawned -= AuthorityHandleUnitDespawned;
        }


        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (!UnitBelongsToPlayer(unit))
            {
                return;
            }
            
            myUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (!UnitBelongsToPlayer(unit))
            {
                return;
            }

            myUnits.Remove(unit);
        }
        
        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            if (!hasAuthority)
            {
                return;
            }
            
            myUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            if (!hasAuthority)
            {
                return;
            }

            myUnits.Remove(unit);
        }
        
        private bool UnitBelongsToPlayer(Unit unit)
        {
            return unit.connectionToClient.connectionId == connectionToClient.connectionId;
        }
    }
}