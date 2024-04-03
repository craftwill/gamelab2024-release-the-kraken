using MoreMountains.Feedbacks;
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
        [SerializeField] private MMF_Player _feedbackPlayer;
        [SerializeField] private OccupancySoundComponent _soundComponent;
        [SerializeField] private SpriteRenderer _highlight;
        private bool _isOverloaded = false;
        private bool _isAlertOnCooldown = false;
        private Color highlightColor;

        private void Start()
        {
            _feedbackPlayer.Initialization();
        }

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

            if (enemyCount > maxEnemyCount)
            {
                _feedbackPlayer.PlayFeedbacks();
                if (!_isOverloaded && !_isAlertOnCooldown)
                {
                    _soundComponent.PlayFullCapacitySound();
                    StartCoroutine(AlertSoundCooldown());
                }
                _isOverloaded = true;
            }
            else
            {
                _feedbackPlayer.StopFeedbacks();
                _isOverloaded = false;
            }

            highlightColor = _highlight.color;
            highlightColor.a = Mathf.Clamp(percentage,0f,1f);
            _highlight.color = highlightColor;
        }

        private IEnumerator AlertSoundCooldown()
        {
            _isAlertOnCooldown = true;
            yield return new WaitForSeconds(Config.current.zoneFullAlertSoundCooldown);
            _isAlertOnCooldown = false;
        }
    }
}
