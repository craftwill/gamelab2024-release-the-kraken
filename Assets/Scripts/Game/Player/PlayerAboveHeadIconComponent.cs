using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerAboveHeadIconComponent : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
