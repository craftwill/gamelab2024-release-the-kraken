using Bytes;
using Kraken.Network;
using UnityEngine;

namespace Kraken
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager Instance { get; set; }
        [SerializeField] private ConfigSO _buildConfig;
        [SerializeField] private ConfigSO _unityConfig;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                if (Application.isEditor)
                    Config.current = _unityConfig;
                else
                    Config.current = _buildConfig;
            }

            VerifyConnectedDevices();
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            EventManager.AddEventListener(EventNames.LateStart, HandleLateStart);
        }

        protected void HandleLateStart(BytesData data)
        {
            Debug.Log("Late start!");

            // Only connect once in the MainMenu LateStart()
            EventManager.Dispatch(EventNames.TryConnectToPhoton, null);
            EventManager.RemoveEventListener(EventNames.LateStart, HandleLateStart);
        }

        #region Getters and setters for Player preferences and settings

        static public bool IsLocalPlayerNickNameSet()
        {
            // FALSE if NickName is empty or null
            if (PlayerPrefs.GetString(Config.PLAYER_NAME_KEY).Equals(string.Empty) || PlayerPrefs.GetString(Config.PLAYER_NAME_KEY).Equals(null))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static public string GetLocalPlayerNickName()
        {
            if (IsLocalPlayerNickNameSet())
            {
                return PlayerPrefs.GetString(Config.PLAYER_NAME_KEY);
            }
            Debug.LogError("GameManager -- No NickName set. Something's wrong: creating a random one.");
            return NetworkUtils.CreateRandomNickname();
        }

        static public void SetLocalPlayerNickName(string name)
        {
            PlayerPrefs.SetString(Config.PLAYER_NAME_KEY, name);
            PlayerPrefs.Save();
            Debug.Log("GameManager -- PlayerPrefs saved.");
        }

        #endregion

        private void VerifyConnectedDevices()
        {
            /*foreach (var gamepad in Gamepad.all)
            {
                Debug.Log($"GAME_MANAGER -- {gamepad} connected!");
                InputSystem.AddDevice(gamepad);
                InputSystem.EnableDevice(gamepad);
                InputSystem.Update();
            }*/
        }
    }
}