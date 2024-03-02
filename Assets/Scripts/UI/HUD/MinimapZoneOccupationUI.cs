using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kraken.UI
{
    public class MinimapZoneOccupationUI : KrakenUIElement
    {
        [SerializeField] private TextMeshProUGUI _percentage;

        public void SetOccupation(int enemyCount, int maxEnemyCount)
        {
            int percentageInt = (int)Mathf.Round(((float)enemyCount / maxEnemyCount) * 100);
            _percentage.text = percentageInt.ToString() + "%";
            if (enemyCount > maxEnemyCount)
            {
                _percentage.color = Color.red;
            }
            else
            {
                _percentage.color = Color.black;
            }
        }
    }
}
