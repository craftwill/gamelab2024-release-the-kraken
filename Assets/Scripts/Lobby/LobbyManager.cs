using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.VisualScripting;

using Photon.Realtime;
using Photon.Pun;

using TMPro;

using Bytes;
using System.IO;
using Newtonsoft.Json;

namespace Kraken
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _txtPlayerList;
        [SerializeField] private TMP_Text _txtRoomCode;
        [SerializeField] private GameObject _controller1;
        [SerializeField] private GameObject _controller2;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private InputActionReference _submitInput;
        [SerializeField] private InputActionReference _cancelInput;
        [SerializeField] private Button _btnStart;
        private GameObject _controlledController;
        private int _currentControllerPositionIndex = 1;
        private int[] controllerPositions = { -300, 0, 300 };
        private bool _movementInput = false;
        private bool _requireTwoPlayers;

        private void Start()
        {
            // Reset night count to 0
            if (PlayerPrefs.HasKey(Config.GAME_NIGHT_KEY))
            {
                PlayerPrefs.SetInt(Config.GAME_NIGHT_KEY, 0);
            }

            if (Config.current.isSkipMainMenu)
            {
                Btn_OnStartGame();
            }
            _currentControllerPositionIndex = 1;

            //creates the file or resets it on a new game
            File.WriteAllText(TowerManager.GetFilePath(), JsonConvert.SerializeObject(new Dictionary<int, int>()));

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "player", 1 }
            });

            _txtRoomCode.text = PhotonNetwork.CurrentRoom.Name;
            TogglePlayer2();
            _requireTwoPlayers = Config.current.requireTwoPlayers;

            _moveInput.action.performed += OnMove;
            _moveInput.action.canceled += OnInputReleased;
            _submitInput.action.performed += OnSubmit;
            _cancelInput.action.performed += OnCancel;

            if (!PhotonNetwork.IsMasterClient)
            {
                _btnStart.interactable = false;
                _btnStart.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (AreAllPlayersReady())
            {
                _btnStart.interactable = true;
            }
            else
            {
                _btnStart.interactable = false;
            }
        }

        private void OnDestroy()
        {
            _moveInput.action.performed -= OnMove;
            _moveInput.action.canceled -= OnInputReleased;
            _submitInput.action.performed -= OnSubmit;
            _cancelInput.action.performed -= OnCancel;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            TogglePlayer2();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            TogglePlayer2();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            //Initialize controller 1 position for joining player
            if (PhotonNetwork.MasterClient.CustomProperties != null && PhotonNetwork.MasterClient.CustomProperties.ContainsKey("player"))
            {
                Vector3 newPos = _controller1.transform.localPosition;
                newPos.x = controllerPositions[(int)PhotonNetwork.MasterClient.CustomProperties["player"]];
                _controller1.transform.localPosition = newPos;
            }
        }

        private void TogglePlayer2()
        {
            if (PhotonNetwork.PlayerList.Length == 2)
            {
                _controller2.SetActive(true);
            }
            else
            {
                _controller2.SetActive(false);
                Vector3 newPos = _controller2.transform.localPosition;
                newPos.x = controllerPositions[1];
                _controller2.transform.localPosition = newPos;
            }

            _controlledController = PhotonNetwork.IsMasterClient ? _controller1 : _controller2;
        }

        public void Btn_OnStartGame() 
        {
            if (AreAllPlayersReady() && PhotonNetwork.IsMasterClient)
            {
                _btnStart.interactable = false;
                _btnStart.gameObject.SetActive(false);
                JoinGameScene();
            }
        }

        public void Btn_OnLeaveLobby()
        {
            AnimateManager.GetInstance().ClearAllAnimations();
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            // Once room left return to main menu
            PhotonNetwork.LoadLevel(1);
        }

        public void Btn_OnCopyCode()
        {
            TextEditor tEditor = new TextEditor()
            {
                text = PhotonNetwork.CurrentRoom.Name
            };
            tEditor.SelectAll();
            tEditor.Copy();
            Debug.Log("Room code copied");
        }

        /// <summary>
        /// RPC event called by Master Client to load the game scene
        /// </summary>
        [PunRPC]
        private void RPC_Master_OpenGameScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // MasterClient is like: yeah, sure, you can join!
                //photonView.RPC(nameof(RPC_JoinGameScene), RpcTarget.All);
                JoinGameScene();
            }
        }

        private void JoinGameScene()
        {
            var customSceneName = Config.current.loadCustomSceneName;
            if (customSceneName != "")
            {
                Debug.LogWarning("You loaded a custom scene with name: " + customSceneName);
                PhotonNetwork.LoadLevel(customSceneName);
                return;
            }
            PhotonNetwork.LoadLevel("Game");
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            if (_movementInput) return;
            _movementInput = true;
            float direction = value.ReadValue<Vector2>().x;
            if (direction < 0 && _currentControllerPositionIndex > 0 && !IsPlayerTaken(_currentControllerPositionIndex - 1))
            {
                _currentControllerPositionIndex--;
            }
            else if (direction > 0 && _currentControllerPositionIndex < 2 && !IsPlayerTaken(_currentControllerPositionIndex + 1))
            {
                _currentControllerPositionIndex++;
            }
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "player", _currentControllerPositionIndex }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            int controllerToMove = _controlledController == _controller1 ? 1 : 2;
            photonView.RPC(nameof(RPC_All_MoveController), RpcTarget.All, _currentControllerPositionIndex, controllerToMove);
        }

        public void OnSubmit(InputAction.CallbackContext value)
        {
            Btn_OnStartGame();
        }

        public void OnCancel(InputAction.CallbackContext value)
        {
            Btn_OnLeaveLobby();
        }

        public void OnInputReleased(InputAction.CallbackContext value)
        {
            _movementInput = false;
        }

        private bool IsPlayerTaken(int id)
        {
            if (id == 1) return false;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties == null || !player.CustomProperties.ContainsKey("player")) continue;
                if ((int) player.CustomProperties["player"] == id)
                {
                    return true;
                }
            }
            return false;
        }

        [PunRPC]
        private void RPC_All_MoveController(int pos, int controller)
        {
            GameObject controllerIcon = controller == 1 ? _controller1 : _controller2;
            Vector3 newPos = controllerIcon.transform.localPosition;
            newPos.x = controllerPositions[pos];
            controllerIcon.transform.localPosition = newPos;
        }

        private bool AreAllPlayersReady()
        {
            if (_requireTwoPlayers && PhotonNetwork.PlayerList.Length != 2) return false;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties == null || !player.CustomProperties.ContainsKey("player"))
                {
                    return false;
                }
                if ((int)player.CustomProperties["player"] == 1)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
