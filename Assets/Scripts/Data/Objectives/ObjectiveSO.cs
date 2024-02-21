using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kraken
{
    public enum ObjectiveType
    {
        None = 0,
        Boss = 1,
        MiniBoss = 2,
        AreaControl = 3
    }

    
    public abstract class ObjectiveSO : ScriptableObject
    {
        [Header("General config")]
        public string objectiveName = "Default Objective Name";
        [Tooltip("Time before next objective spawns")] public int objectiveTimer = 60;

        public virtual void TriggerObjective(ObjectiveInstance instance)
        {
            Animate.Delay(objectiveTimer, () => EventManager.Dispatch(EventNames.NextObjective, null));
        }

        //public void OnMyFieldChanged(ObjectiveType from, ObjectiveType to)
        //{

        //}
    }

//#if UNITY_EDITOR

//    [CustomEditor(typeof(ObjectiveSO))]
//    class CustomTypeEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            ObjectiveSO customType = (ObjectiveSO)target;

//            ObjectiveType test = (ObjectiveType)EditorGUILayout.EnumPopup(customType.objective);
            
//            if (test != customType.objective)
//            {
//                customType.OnMyFieldChanged(customType.objective, test);
//                customType.objective = (ObjectiveType)test;
//            }

//            //customType.MyProperty = EditorGUILayout.IntField("Property", customType.MyProperty);
//        }
//    }

//#endif
}