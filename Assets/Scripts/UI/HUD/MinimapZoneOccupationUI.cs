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
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite[] _sprites;
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
            float percentage = ((float)enemyCount / maxEnemyCount);
            if (enemyCount == 0)
            {
                _spriteRenderer.sprite = null;
            }
            else
            {
                for (int i = 0; i < _sprites.Length; i++)
                {
                    if (percentage < (1.0f/_sprites.Length) * (i+1))
                    {
                        _spriteRenderer.sprite = _sprites[i];
                        break;
                    }
                }
            }
        }
    }
}
