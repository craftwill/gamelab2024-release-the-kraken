using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Kraken/Enemy Config")]
    public class EnemyConfigSO : ScriptableObject
    {
        public string enemyName = "Default Enemy Name";

        [Header("Stats")]
        public float maxHealth = 3f;
        public float moveSpeed = 1.0f;

        [Header("Combat")]
        public float attackRange = 2.5f;
        public float damageDealt = 1;
        public float attackCooldown = 1f;
        [Tooltip("How long is the hitbox out")] public float attackDuration = 0.1f;
        public float lockedIntoAttackDuration = 0.5f;

        [Header("Zone occupancy")]
        public int zoneOccupancyCount = 1;

        [Header("Pathfinding")]
        public float pathfindingDistanceRadius = 100f;
    }
}