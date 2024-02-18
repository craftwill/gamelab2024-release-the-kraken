using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public enum EnemyClanType
    {
        Sheep,
        Wolf
    }

    [CreateAssetMenu(fileName = "EnemySpawnPack", menuName = "Kraken/Spawning/Enemy Spawn Pack")]
    public class EnemySpawnPack : ScriptableObject
    {
        public EnemyClanType enemyClanType;
        public GameObject normalMinion;
        public GameObject meleeMinion;
        public GameObject rangeMinion;

        public GameObject GetRandomMinion()
        {
            GameObject[] enemies = new GameObject[] { normalMinion, meleeMinion, rangeMinion };
            return enemies[Random.Range(0, enemies.Length)];
        }
    }
}