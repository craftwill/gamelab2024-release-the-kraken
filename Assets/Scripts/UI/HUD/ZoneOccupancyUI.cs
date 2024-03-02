
using UnityEngine;

using TMPro;

using Bytes;

namespace Kraken.UI
{
    public class ZoneOccupancyUI : KrakenUIElement
    {
        [Header("ZoneOccupancyUI")]
        [SerializeField] private TextMeshProUGUI _txtOccupancy;

        private void Start()
        {
            EventManager.AddEventListener(EventNames.UpdateCurrentZoneOccupancyUI, HandleUpdateCurrentZoneOccupancyUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateCurrentZoneOccupancyUI, HandleUpdateCurrentZoneOccupancyUI);
        }

        private void HandleUpdateCurrentZoneOccupancyUI(BytesData data)
        {
            var occupancyData = data as UpdateZoneOccupancyUIData;
            string color = "<color=" + ((occupancyData.EnemyCount > occupancyData.MaxEnemyCount) ? "red" : "white") + ">";
            _txtOccupancy.text = $"{color}{occupancyData.EnemyCount}</color>/<color=white>{occupancyData.MaxEnemyCount}</color>";
        }
    }
}
