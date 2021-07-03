using System;
using Combat;
using Mirror;
using UnityEngine;

namespace Units
{
   public class Unit : NetworkBehaviour
   {
      [SerializeField] private Health health;
      [SerializeField] private GameObject selectHighlight;
      [SerializeField] private Targeter targeter; 
      [SerializeField] private UnitMovement unitMovement; 

      public static event Action<Unit> OnServerUnitSpawned;
      public static event Action<Unit> OnServerUnitDespawned;
   
      public static event Action<Unit> OnAuthorityUnitSpawned;
      public static event Action<Unit> OnAuthorityUnitDespawned;

      public UnitMovement UnitMovement => unitMovement;
      public Targeter Targeter => targeter;
      
      #region Server

      public override void OnStartServer()
      {
         health.ServerOnDie += ServerHandleOnDie;
         OnServerUnitSpawned?.Invoke(this);
      }

      public override void OnStopServer()
      {
         health.ServerOnDie -= ServerHandleOnDie;
         OnServerUnitDespawned?.Invoke(this);
      }

      [Server]
      private void ServerHandleOnDie()
      {
         NetworkServer.Destroy(gameObject);
      }

      #endregion Server
   
      #region Client
      
      public override void OnStartAuthority()
      {
         if(!hasAuthority) { return; }
         OnAuthorityUnitSpawned?.Invoke(this);
      }

      public override void OnStopClient()
      {
         if(!hasAuthority) { return; }
         OnAuthorityUnitDespawned?.Invoke(this);
      }
   
      public void Select(bool selected)
      {
         Debug.Log($"Selecting tank {netIdentity.netId} {selected} {selectHighlight.name}");
         selectHighlight.SetActive(selected);
         Debug.Log(selectHighlight.activeSelf);
      }

      private void Start()
      {
         Select(false);
      }

      #endregion
   }
}
