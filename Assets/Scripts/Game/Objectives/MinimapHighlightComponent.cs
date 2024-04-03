using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MinimapHighlightComponent : MonoBehaviourPun
    {
        [SerializeField] GameObject _border;
        public void SetVisible(bool visible)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            photonView.RPC(nameof(RPC_All_SetHighlightVisible), RpcTarget.All, visible);
        }

        [PunRPC]
        private void RPC_All_SetHighlightVisible(bool visible)
        {
            if (_border != null)
            {
                _border.SetActive(visible);
            }
        }
    }

}