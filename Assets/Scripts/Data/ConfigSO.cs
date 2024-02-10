using UnityEngine;

namespace Kraken
{
    [CreateAssetMenu(fileName = "GlobalConfig", menuName = "Kraken/Create Global Config")]
    public class ConfigSO : ScriptableObject
    {
        [Header("Testing")]
        public bool showDebugLogs;

        [Header("Network")]
        public string loadCustomSceneName = "";
        public bool isSkipMainMenu = false;

        [Header("Player Controls settings")]
        public float moveSpeed = 7.5f;
        public float sprintSpeed = 14f;
        public float rotationSpeed = 50f;
        public float gravity = 5f;
        public float xCameraSensitivity = 360f;
        public float yCameraSensitivity = 2f;
    }
}