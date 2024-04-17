using Bytes;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public class DefeatScreenUI : BaseEndGameScrenUI
    {
        [SerializeField] private GameObject _sheepLost;
        [SerializeField] private GameObject _playerLost;
        [SerializeField] private GameObject _timeLost;
        [SerializeField] Button _startOverButton;
        
        private void Start()
        {
            EventManager.AddEventListener(EventNames.ShowDefeatScreenUI, HandleShowScreenUI);
            EventManager.AddEventListener(EventNames.ShowDefeatByZoneUI, HandleShowDefeatByZoneUI);
            EventManager.AddEventListener(EventNames.ShowDefeatByPlayerUI, HandleShowDefeatByPlayerUI);
            EventManager.AddEventListener(EventNames.ShowDefeatTimeLeftUI, HandleShowDefeatTimeLeftUI);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowDefeatScreenUI, HandleShowScreenUI);
            EventManager.RemoveEventListener(EventNames.ShowDefeatByZoneUI, HandleShowDefeatByZoneUI);
            EventManager.RemoveEventListener(EventNames.ShowDefeatByPlayerUI, HandleShowDefeatByPlayerUI);
            EventManager.RemoveEventListener(EventNames.ShowDefeatTimeLeftUI, HandleShowDefeatTimeLeftUI);
        }

        protected override void HandleShowScreenUI(BytesData data)
        {
            PlayerPrefs.SetInt(Config.GAME_NIGHT_KEY, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0));
            base.HandleShowScreenUI(data);
            StartCoroutine(PreventAccidentClick());
        }

        private void HandleShowDefeatByZoneUI(BytesData data)
        {
            _sheepLost.SetActive(true);
        }

        private void HandleShowDefeatByPlayerUI(BytesData data)
        {
            _playerLost.SetActive(true);
        }

        private void HandleShowDefeatTimeLeftUI(BytesData data)
        {
            _timeLost.SetActive(true);
        }

        public virtual void Btn_Restart()
        {
            if (_onlyOnce)
            {
                AnimateManager.GetInstance().ClearAllAnimations();
                _onlyOnce = false;
                if (!PhotonNetwork.IsMasterClient) return;
                EventManager.Dispatch(EventNames.StopAllSounds, null);
                PhotonNetwork.LoadLevel("Lobby");
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
            if (PhotonNetwork.IsMasterClient)
            {
                _startOverButton.gameObject.SetActive(true);
                _startOverButton.Select();
            }
            else
            {
                _backToMenuButton.Select();
            }
        }
    }
}
