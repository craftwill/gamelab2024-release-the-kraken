using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class ObjectiveInstance
    {
        public ObjectiveSO objectiveSO;

        public bool IsCompleted { get; private set; } = false;
        public Zone Zone { get; private set; } = null;

        public ObjectiveInstance(ObjectiveSO objectiveSO, Zone zone)
        {
            this.objectiveSO = objectiveSO;
            this.Zone = zone;
        }

        public void TriggerObjective()
        {
            objectiveSO.TriggerObjective(this);
        }

        public void EndObjective(bool goToNext)
        {
            //If EndObjective has already been called, don't do anything
            if (IsCompleted) return;
            IsCompleted = true;
            if (goToNext) EventManager.Dispatch(EventNames.NextObjective, null);
        }
    }

}