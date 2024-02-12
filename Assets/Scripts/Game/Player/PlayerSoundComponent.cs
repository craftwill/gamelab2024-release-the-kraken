using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundComponent : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event _sprintSound;   // Temporary probably

    public void PlaySprintSFX()
    {
        _sprintSound.Post(gameObject);
    }
}
