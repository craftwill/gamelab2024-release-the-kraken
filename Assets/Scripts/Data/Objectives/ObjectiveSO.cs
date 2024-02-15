using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "Objective", menuName = "Kraken/Systems/Objective")]
    public class ObjectiveSO : ScriptableObject
    {
        public string objectiveName = "Default Objective Name";
    }
}