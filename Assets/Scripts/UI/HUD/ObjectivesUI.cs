using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using WebSocketSharp;

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

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateObjectiveUI, HandleUpdateObjectiveUI);
        }

        public void HandleUpdateObjectiveUI(BytesData data)
        {
            UpdateObjectiveUIData objectiveInstanceData = (data as UpdateObjectiveUIData);

            if (objectiveInstanceData.ObjectiveName.IsNullOrEmpty())
            {
                SetVisible(false);
                return;
            }

            SetVisible(true);
            _txtObjectiveName.text = objectiveInstanceData.ObjectiveName;
        }
    }
}
