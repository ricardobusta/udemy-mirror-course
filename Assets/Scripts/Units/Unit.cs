using System;
using Combat;
using Mirror;
using UnityEngine;

namespace Units
{
   public class Unit : NetworkBehaviour
   {
      [SerializeField] private SpriteRenderer selectHighlight;
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
         OnServerUnitSpawned?.Invoke(this);
      }

      public override void OnStopServer()
      {
         OnServerUnitDespawned?.Invoke(this);
      }

      public override void OnStartClient()
      {
         Select(false);

         if(!isClientOnly || !hasAuthority) { return; }
         OnAuthorityUnitSpawned?.Invoke(this);
      }

      public override void OnStopClient()
      {
         if(!isClientOnly || !hasAuthority) { return; }
         OnAuthorityUnitDespawned?.Invoke(this);
      }

      #endregion Server
   
      #region Client
   
      public void Select(bool selected)
      {
         selectHighlight.enabled = selected;
      }

      #endregion
   }
}
