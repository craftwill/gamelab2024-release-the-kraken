using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.UIElements;
using Cinemachine;
using Bytes;

namespace Kraken
{
    public class PlayerEntity : Entity
    {
        private bool _isOwner;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
            }
        }

        protected override void HandleDie()
        {
            base.HandleDie();

            StringDataBytes bytes = new StringDataBytes(PhotonNetwork.LocalPlayer.UserId);
            EventManager.Dispatch(EventNames.PlayerDeath, bytes);
        }
    }
}
