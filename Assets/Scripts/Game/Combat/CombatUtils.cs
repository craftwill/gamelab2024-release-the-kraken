using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class CombatUtils
    {
        static public List<EnemyEntity> GetEnemyEntitiesInRadius(Vector3 originPosition, float radius)
        {
            Collider[] collidersFoundInSphere = Physics.OverlapSphere(originPosition, radius, Physics.AllLayers, QueryTriggerInteraction.Collide);
            List<EnemyEntity> enemiesFound = new List<EnemyEntity>();

            foreach (Collider col in collidersFoundInSphere)
            {
                EnemyEntity enemyEntity = col.GetComponent<EnemyEntity>();
                if (enemyEntity != null)
                {
                    if (enemiesFound.Contains(enemyEntity)) continue;

                    enemiesFound.Add(enemyEntity);
                }
            }

            return enemiesFound;
        }

        private static PlayerEntity[] PlayerEnties = null;

        static public PlayerEntity[] GetPlayerEntities()
        {
            if (PlayerEnties is null) PlayerEnties = GameObject.FindObjectsByType<PlayerEntity>(FindObjectsSortMode.None);
            return PlayerEnties;
        }
    }
}