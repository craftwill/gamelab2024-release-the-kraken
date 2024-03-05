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
                _percentage.color = Color.red;
            }
            else
            {
                _percentage.color = Color.black;
            }
        }
    }
}
