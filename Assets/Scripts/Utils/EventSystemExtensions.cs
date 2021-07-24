using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Utils
{
    public static class EventSystemExtensions
    {
        private static readonly List<RaycastResult> RaycastResult = new List<RaycastResult>();
        private static readonly int UILayer = LayerMask.NameToLayer("UI");

        public static bool IsPointerOverUIObject(this EventSystem eventSystem)
        {
            // Maybe have a monobehaviour receiving pointereventdata on mouse movement instead, or earlyUpdate,
            // and only performing ray casting once for every object that need this information.
            var eventData = new PointerEventData(eventSystem) {position = Mouse.current.position.ReadValue()};
            return IsPointerOverUIObject(eventSystem, eventData);
        }
        
        public static bool IsPointerOverUIObject(this EventSystem eventSystem, PointerEventData eventData)
        {
            RaycastResult.Clear();
            eventSystem.RaycastAll(eventData, RaycastResult);

            for (var i = 0; i < RaycastResult.Count; i++)
            {
                if (RaycastResult[i].gameObject.layer == UILayer)
                {
                    return true;
                }
            }

            return false;
        }
    }
}