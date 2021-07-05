using System;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class Building : NetworkBehaviour
    {
        [SerializeField] private GameObject buildingPreview;
        [SerializeField] private Sprite icon;
        [SerializeField] private int id = -1;
        [SerializeField] private int price;
        
        public static event Action<Building> OnServerBuildingSpawned;
        public static event Action<Building> OnServerBuildingDespawned;
   
        public static event Action<Building> OnAuthorityBuildingSpawned;
        public static event Action<Building> OnAuthorityBuildingDespawned;

        public Sprite Icon => icon;
        public int Id => id;
        public int Price => price;
        public GameObject BuildingPreview => buildingPreview;

        #region Server

        public override void OnStartServer()
        {
            OnServerBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            OnServerBuildingDespawned?.Invoke(this);
        }

        #endregion Server

        #region Client

        public override void OnStartAuthority()
        {
            if(!hasAuthority) { return; }
            OnAuthorityBuildingSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if(!hasAuthority) { return; }
            OnAuthorityBuildingDespawned?.Invoke(this);
        }

        #endregion Client
    }
}