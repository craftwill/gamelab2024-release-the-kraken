using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "EnemySpawnData", menuName = "Kraken/Spawning/EnemySpawnData")]
    public class EnemySpawnDataSO : ScriptableObject
    {
        public bool sheepMelee;
        [HideInInspector] public int meleeSheepSpawnRatio = 0;
        [HideInInspector] public GameObject meleeSheepPrefab = null;

        public bool sheepRange;
        [HideInInspector] public int rangeSheepSpawnRatio = 0;
        [HideInInspector] public GameObject rangeSheepPrefab = null;


        [HideInInspector] public List<string> spawnableMobs = new List<string>();
        //This class is unused but let it be known I shed blood and tears to make this steaming piece of shit that is a mix of curse code and brilliancy

        public GameObject GetRandomEnemyToSpawn()
        {
            //hardcoded random for now
            int random = Random.Range(0, 100);
            if (random >= 50) return meleeSheepPrefab;
            else return rangeSheepPrefab;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(EnemySpawnDataSO))]
    class CustomTypeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EnemySpawnDataSO data = (EnemySpawnDataSO)target;
            bool flag = false;
            flag = flag || AddEditorUI(data.sheepMelee, nameof(data.sheepMelee), ref data.meleeSheepSpawnRatio, ref data.meleeSheepPrefab, data);
            flag = flag || AddEditorUI(data.sheepRange, nameof(data.sheepRange), ref data.rangeSheepSpawnRatio, ref data.rangeSheepPrefab, data);
            if (flag)
            {
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
            }
        }

        public bool AddEditorUI(bool flag, string name, ref int ratio, ref GameObject prefab, EnemySpawnDataSO data)
        {
            bool hasChanges = false;
            EditorGUILayout.Space();
            if (flag)
            {
                int newRatio = EditorGUILayout.IntField(name + " spawn ratio", ratio);
                
                GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(name + " prefab", prefab, typeof(GameObject), false);
                if(newRatio != ratio)
                {
                    
                    ratio = newRatio;
                    hasChanges = true;
                }
                if(newPrefab != prefab)
                {
                    hasChanges = true;
                    prefab = newPrefab;
                }

            }
            else
            {
                data.spawnableMobs.Remove(name);
                ratio = 0;
                prefab = null;
            }
            return hasChanges;
        }
    }

#endif
}