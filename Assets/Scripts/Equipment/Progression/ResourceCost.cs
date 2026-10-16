using System;
using Resources;
using UnityEngine;

namespace Equipment.Progression
{
    [Serializable]
    public class ResourceCost
    {
        public ResourceType Resource;

        [Min(1)]
        public int Amount = 1;
    }
}