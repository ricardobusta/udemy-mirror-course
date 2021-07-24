using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Utils
{
    public static class ExtensionMethods
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
        
        public static bool RayCast(this Camera camera, out RaycastHit hit, float maxDist, int layerMask)
        {
            var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            return Physics.Raycast(ray, out hit, maxDist, layerMask);
        }
        
        public static void Shuffle<T>(this IList<T> list)  
        {  
            var i = list.Count;  
            while (i > 1) {  
                i--;  
                var j = Random.Range(0, i+1);  
                var t = list[j];  
                list[j] = list[i];  
                list[i] = t;  
            }  
        }
    }
}