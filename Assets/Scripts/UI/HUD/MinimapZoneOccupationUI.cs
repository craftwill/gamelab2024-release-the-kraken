using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kraken
{
    public class MinimapZoneOccupationUI : MonoBehaviourPun
    {
        [SerializeField] private TextMeshProUGUI _percentage;
        [SerializeField] private OccupancySoundComponent _soundComponent;
        private bool _isOverloaded = false;

        public void SetOccupation(int enemyCount, int maxEnemyCount)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(RPC_All_UpdateDisplay), RpcTarget.All, enemyCount, maxEnemyCount);
            }
        }

        [PunRPC]
        private void RPC_All_UpdateDisplay(int enemyCount, int maxEnemyCount)
        {
            int percentageInt = (int)Mathf.Round(((float)enemyCount / maxEnemyCount) * 100);
            _percentage.text = percentageInt.ToString() + "%";
            if (enemyCount > maxEnemyCount)
            {
                if (!_isOverloaded)
                {
                    _soundComponent.PlayFullCapacitySound();
                }
                _percentage.color = Color.red;
                _isOverloaded = true;
            }
            else
            {
                _percentage.color = Color.black;
                _isOverloaded = false;
            }
        }
    }
}
