using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class MinimapAlwaysFaceNorthComponent : MonoBehaviour
    {
        [SerializeField] private Vector3 direction = Vector3.down;
        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

}
