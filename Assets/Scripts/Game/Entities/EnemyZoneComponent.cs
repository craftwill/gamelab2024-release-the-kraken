using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class EnemyZoneComponent : MonoBehaviourPun
    {
        public int ZoneCount { get; private set; } = 0;

        public void InitSettings(int zoneCount)
        {
            ZoneCount = zoneCount;
        }
    }
}