using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
   [SerializeField] private SpriteRenderer selectHighlight;

   #region Server

   

   #endregion Server
   
   #region Client
   
   public void Select(bool selected)
   {
      selectHighlight.enabled = selected;
   }
   
   private void Start()
   {
      Select(false);
   }
   
   #endregion
}
