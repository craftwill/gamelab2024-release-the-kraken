
using Bytes;
using Photon.Pun;
using System.Collections;
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

        protected override void HandleShowScreenUI(BytesData data)
        {
            PlayerPrefs.SetInt(Config.GAME_NIGHT_KEY, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0) + 1);
            base.HandleShowScreenUI(data);
            StartCoroutine(PreventAccidentClick());
        }

        public void Btn_NextNight()
        {
            if (_onlyOnce)
            {
                _onlyOnce = false;
                _btnNextNight.interactable = false;
                _gameFlowManager.GoToNextNight();
                AkSoundEngine.StopAll();
            }
        }

        public override void SetVisible(bool isVisible)
        {
            base.SetVisible(isVisible);
        }

        private IEnumerator PreventAccidentClick()
        {
            yield return new WaitForSeconds(2f);

            _backToMenuButton.gameObject.SetActive(true);
            _btnNextNight.gameObject.SetActive(true);
            _btnNextNight.Select();
        }
    }
}
