using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundComponent : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event _sprintSound;   // Temporary probably

    [PunRPC]
    public void RPC_All_PlaySprintSound()
    {
        _sprintSound.Post(gameObject);
    }
}
