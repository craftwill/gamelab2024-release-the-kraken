using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class InflictDamageComponent : MonoBehaviour
    {
        public EntityClan Damageclan { get; set; } = EntityClan.Ally;
        public float Damage { get; set; } = 1f;
        public Transform Source = null;
    }
}
