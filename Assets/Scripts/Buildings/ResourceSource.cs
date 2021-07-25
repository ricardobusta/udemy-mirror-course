using System;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class ResourceSource : MonoBehaviour
    {
        public static List<ResourceSource> resources = new List<ResourceSource>();
        
        private void Awake()
        {
            resources.Add(this);
        }

        private void OnDestroy()
        {
            resources.Remove(this);
        }
    }
}