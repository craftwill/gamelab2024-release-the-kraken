using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerMinimapDirectionComponent : MonoBehaviourPun
    {
        private void Start()
        {
            if (!photonView.AmOwner)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
