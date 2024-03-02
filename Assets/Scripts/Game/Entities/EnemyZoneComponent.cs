using Kraken.Game;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kraken
{
    public class EnemyZoneComponent : MonoBehaviourPun
    {
        public int ZoneCount { get; private set; } = 0;
        public List<Zone> CurrentZones { get; private set; } = new List<Zone>();

        public void InitSettings(int zoneCount)
        {
            ZoneCount = zoneCount;
        }

        public void SetZoneToEnemy(Zone z)
        {
            CurrentZones.Add(z);
        }

        public void RemoveZoneToEnemy(Zone z)
        {
            CurrentZones.Remove(z);
        }

        public void RemoveEnemyFromZones()
        {
            foreach(Zone z in CurrentZones)
            {
                z.ChangeEnemyCount(-ZoneCount);
            }
        }
    }
}