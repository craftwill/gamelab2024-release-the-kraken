using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class EnemyMinimapComponent : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(Config.current.showBasicEnemies);
        }
    }

}