
using Bytes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public class VictoryScreenUI : BaseEndGameScrenUI
    {
        [SerializeField] private GameFlowManager _gameFlowManager;
        [SerializeField] private Button _btnNextNight;
        private void Start()
        {
            _gameFlowManager = Object.FindObjectOfType<GameFlowManager>(); //temp but i don't wanna lock the game scene
            EventManager.AddEventListener(EventNames.ShowVictoryScreenUI, HandleShowScreenUI);
            _btnNextNight.interactable = PhotonNetwork.IsMasterClient;
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowVictoryScreenUI, HandleShowScreenUI);
        }

        public void Btn_NextNight()
        {
            _btnNextNight.interactable = false;
            _gameFlowManager.GoToNextNight();
        }

        
    }
}
