using System;
using Mirror;
using Rts.Networking;
using UnityEngine;

public class Unit : NetworkBehaviour
{
   [SerializeField] private SpriteRenderer selectHighlight;

   public static event Action<Unit> OnServerUnitSpawned;
   public static event Action<Unit> OnServerUnitDespawned;
   
   public static event Action<Unit> OnAuthorityUnitSpawned;
   public static event Action<Unit> OnAuthorityUnitDespawned;
   
   public UnitMovement UnitMovement { get; private set; }
   
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

   private void Awake()
   {
      UnitMovement = GetComponent<UnitMovement>();
   }

   #endregion
}
