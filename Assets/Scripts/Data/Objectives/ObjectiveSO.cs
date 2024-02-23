using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public abstract class ObjectiveSO : ScriptableObject
    {
        [Header("General config")]
        public string objectiveName = "Default Objective Name";
        [Tooltip("Time before next objective spawns")] public int objectiveTimer = 60;

        public virtual void TriggerObjective(ObjectiveInstance instance)
        {
            Animate.Delay(objectiveTimer, () => EventManager.Dispatch(EventNames.NextObjective, null));
        }
    }
}