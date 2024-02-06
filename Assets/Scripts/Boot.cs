using Kraken;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class Boot : MonoBehaviour
    {
        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
