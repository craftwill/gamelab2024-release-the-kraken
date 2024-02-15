using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Kraken.UI
{
    public class ObjectivesUI : KrakenUIElement
    {
        [Header("ObjectivesUI")]
        [SerializeField] private TextMeshProUGUI _txtObjectiveName;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdateObjectiveUI, HandleUpdateObjectiveUI);
        }

        public void HandleUpdateObjectiveUI(BytesData data)
        {
            ObjectiveInstance objectiveInstanceData = (data as UpdateObjectiveUIData).ObjectiveInstance;

            Debug.Log("123");

            if (objectiveInstanceData == null)
            {
                SetVisible(false);
                return;
            }

            SetVisible(true);
            _txtObjectiveName.text = objectiveInstanceData.objectiveSO.objectiveName;
        }
    }
}
