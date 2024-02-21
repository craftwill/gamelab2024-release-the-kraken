using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnData", menuName = "Kraken/Spawning/EnemySpawnData")]
public class EnemySpawnData : ScriptableObject
{
    public bool sheepMelee;
    public int meleeSheepSpawnRatio { get; set; } = 0;
    public GameObject meleeSheepPrefab { get; set; } = null;

    public bool sheepRange;
    public int rangeSheepSpawnRatio { get; set; } = 0;
    public GameObject rangeSheepPrefab { get; set; } = null;

    private class Entry
    {
        int weight;
        GameObject entry;
    }
    private List<Entry> entries = new List<Entry>();

    public GameObject GetRandomEnemyToSpawn()
    {
        //hardcoded random for now
        int random = Random.Range(0, 100);
        if (random >= 50) return meleeSheepPrefab;
        else return rangeSheepPrefab;
    }

    public void FieldChanged()
    {
        //to be implemented
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(EnemySpawnData))]
class CustomTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnemySpawnData data = (EnemySpawnData)target;

        if (data.sheepMelee)
        {
            int ratio = EditorGUILayout.IntField(nameof(data.sheepMelee) + " spawn ratio", data.meleeSheepSpawnRatio);
            GameObject prefab = (GameObject)EditorGUILayout.ObjectField(nameof(data.sheepMelee) + " prefab", data.meleeSheepPrefab, typeof(GameObject), false);

            if(data.meleeSheepSpawnRatio != ratio || data.meleeSheepPrefab != prefab)
            {
                data.meleeSheepSpawnRatio = ratio;
                data.meleeSheepPrefab = prefab;

                data.FieldChanged();
            }
        }
        else
        {
            data.meleeSheepSpawnRatio = 0;
        }

        if (data.sheepRange)
        {
            data.rangeSheepSpawnRatio = EditorGUILayout.IntField(nameof(data.sheepRange) + " spawn ratio", data.rangeSheepSpawnRatio);
            data.rangeSheepPrefab = (GameObject)EditorGUILayout.ObjectField(nameof(data.sheepMelee) + " prefab", data.rangeSheepPrefab, typeof(GameObject), false);
        }
        else
        {
            data.rangeSheepSpawnRatio = 0;
        }
    }
}

#endif

