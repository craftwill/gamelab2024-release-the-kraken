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
using static UnityEditor.PlayerSettings;

namespace Kraken
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _txtPlayerList;
        [SerializeField] private TMP_Text _txtRoomCode;
        [SerializeField] private GameObject _controller1;
        [SerializeField] private GameObject _controller2;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private Button _btnStart;
        private GameObject _controlledController;
        private int _currentControllerPositionIndex = 1;
        private int[] controllerPositions = { -300, 0, 300 };
        private bool _movementInput = false;
        private bool _requireTwoPlayers;

        private void Start()
        {
            if (Config.current.isSkipMainMenu)
            {
                Btn_OnStartGame();
            }
            _txtRoomCode.text = PhotonNetwork.CurrentRoom.Name;
            TogglePlayer2();
            _requireTwoPlayers = Config.current.requireTwoPlayers;

            //Initialize controller 1 position for joining player
            if (PhotonNetwork.MasterClient.CustomProperties != null && PhotonNetwork.MasterClient.CustomProperties.ContainsKey("player"))
            {
                Vector3 newPos = _controller1.transform.localPosition;
                newPos.x = controllerPositions[(int)PhotonNetwork.MasterClient.CustomProperties["player"]];
                _controller1.transform.localPosition = newPos;
            }

            _moveInput.action.performed += OnMove;
            _moveInput.action.canceled += OnInputReleased;
        }

        private void Update()
        {
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
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            photonView.RPC(nameof(RPC_All_UpdateLobbyView), RpcTarget.All);
            TogglePlayer2();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            photonView.RPC(nameof(RPC_All_UpdateLobbyView), RpcTarget.All);
            TogglePlayer2();
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

        [PunRPC]
        private void RPC_All_UpdateLobbyView() 
        {
            LocalUpdateLobbyView();
        }

        private void LocalUpdateLobbyView()
        {
            _txtPlayerList.text = "";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];

                _txtPlayerList.text += player.NickName;
                if (player.UserId == PhotonNetwork.LocalPlayer.UserId)
                {
                    _txtPlayerList.text += " (YOU)";
                }

                _txtPlayerList.text += "\n";
            }
        }

        public void Btn_OnStartGame() 
        {
            if (AreAllPlayersReady())
            {
                photonView.RPC(nameof(RPC_Master_OpenGameScene), RpcTarget.MasterClient);
            }
        }

        public void Btn_OnLeaveLobby()
        {
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
                    if (player.CustomProperties == null)
                    {
                        Debug.Log("player " + player.UserId + " has no custom properties");
                    }
                    else if (!player.CustomProperties.ContainsKey("player"))
                    {
                        Debug.Log("player " + player.UserId + "'s properties have no player key");
                    }
                    return false;
                }
                if ((int)player.CustomProperties["player"] == 1)
                {
                    Debug.Log("player " + player.UserId + " has not selected a character");
                    return false;
                }
                Debug.Log("player " + player.UserId + " is ready to go");
            }
            return true;
        }
    }
}
