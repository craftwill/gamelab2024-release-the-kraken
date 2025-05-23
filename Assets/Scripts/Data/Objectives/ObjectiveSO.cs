using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public abstract class ObjectiveSO : ScriptableObject
    {
        public enum Type { Territory, Miniboss, Boss };
        [Header("General config")]
        public string objectiveName = "Default Objective Name";
        [Tooltip("Time before next objective spawns")] public int objectiveTimer = 60;
        [SerializeField] public Type objectiveType;
        [SerializeField] public string reinforcementText;

        public virtual void TriggerObjective(ObjectiveInstance instance) { }
        public virtual void EndObjective(ObjectiveInstance instance) 
        {
            instance.Zones?.ForEach(x => x?.SetIsActiveZone(false));
            EventManager.Dispatch(EventNames.UpdateCurrentZoneOccupancyUI, new UpdateZoneOccupancyUIData(0, 10));
        }
    }
}