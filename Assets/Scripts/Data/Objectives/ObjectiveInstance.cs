using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class ObjectiveInstance
    {
        public ObjectiveSO objectiveSO;

        public bool IsCompleted { get; private set; } = false;

        public ObjectiveInstance(ObjectiveSO objectiveSO)
        {
            this.objectiveSO = objectiveSO;
        }
    }

}