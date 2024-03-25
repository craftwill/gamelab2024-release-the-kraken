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
        public List<Zone> Zones { get; private set; } = null;

        public List<MinimapHighlightComponent> MinimapHighlights { get; private set; } = null;

        public ObjectiveInstance(ObjectiveSO objectiveSO, List<Zone> zones, List<MinimapHighlightComponent> minimapHighlights)
        {
            this.objectiveSO = objectiveSO;
            this.Zones = zones;
            this.MinimapHighlights = minimapHighlights;

        }

        public void TriggerObjective()
        {
            objectiveSO.TriggerObjective(this);
            if (MinimapHighlights is not null)
            {
                foreach(MinimapHighlightComponent mhc in MinimapHighlights)
                {
                    mhc?.SetVisible(true);
                }
            }
        }

        public void EndObjective(bool goToNext)
        {
            //If EndObjective has already been called, don't do anything
            if (IsCompleted) return;
            IsCompleted = true;
            objectiveSO.EndObjective(this);
            if (MinimapHighlights is not null)
            {
                foreach (MinimapHighlightComponent mhc in MinimapHighlights)
                {
                    mhc?.SetVisible(false);
                }
            }
            if (goToNext) EventManager.Dispatch(EventNames.NextObjective, null);
        }
    }

}