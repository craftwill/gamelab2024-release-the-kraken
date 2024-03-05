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

        public MinimapHighlightComponent MinimapHighlight { get; private set; } = null;

        public ObjectiveInstance(ObjectiveSO objectiveSO, Zone zone, MinimapHighlightComponent minimapHighlight)
        {
            this.objectiveSO = objectiveSO;
            this.Zone = zone;
            this.MinimapHighlight = minimapHighlight;

        }

        public void TriggerObjective()
        {
            objectiveSO.TriggerObjective(this);
            MinimapHighlight?.SetVisible(true);
        }

        public void EndObjective(bool goToNext)
        {
            //If EndObjective has already been called, don't do anything
            if (IsCompleted) return;
            IsCompleted = true;
            objectiveSO.EndObjective(this);
            MinimapHighlight?.SetVisible(false);
            if (goToNext) EventManager.Dispatch(EventNames.NextObjective, null);
        }
    }

}