
using Bytes;
using Photon.Pun;
using UnityEngine;

namespace Kraken.UI
{
    public class VictoryScreenUI : BaseEndGameScrenUI
    {
        [SerializeField] private GameFlowManager _gameFlowManager;
        private void Start()
        {
            _gameFlowManager = Object.FindObjectOfType<GameFlowManager>(); //temp but i don't wanna lock the game scene
            EventManager.AddEventListener(EventNames.ShowVictoryScreenUI, HandleShowScreenUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowVictoryScreenUI, HandleShowScreenUI);
        }

        public void Btn_NextNight()
        {
            _gameFlowManager.GoToNextNight();
        }

        
    }
}
