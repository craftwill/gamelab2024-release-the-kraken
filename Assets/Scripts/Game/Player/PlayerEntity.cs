using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Kraken
{
    public class PlayerEntity : MonoBehaviourPun
    {
        private bool _isOwner;

        private void Start()
        {
            if (photonView.AmOwner)
            {
                _isOwner = true;
            }
        }

        private void Update()
        {
            if (_isOwner)
            {
                transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Config.current.moveSpeed * Time.deltaTime;
            }
        }
    }
}
