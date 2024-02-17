using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.UIElements;
using Cinemachine;

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
    }
}
