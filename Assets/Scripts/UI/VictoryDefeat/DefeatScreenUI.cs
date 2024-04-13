using Bytes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Kraken.UI
{
    public class DefeatScreenUI : BaseEndGameScrenUI
    {
        [SerializeField] private GameObject _sheepLost;
        [SerializeField] private GameObject _playerLost;
        [SerializeField] Button _startOverButton;
        private void Start()
        {
            EventManager.AddEventListener(EventNames.ShowDefeatScreenUI, HandleShowScreenUI);
            EventManager.AddEventListener(EventNames.ShowDefeatByZoneUI, HandleShowDefeatByZoneUI);
            EventManager.AddEventListener(EventNames.ShowDefeatByPlayerUI, HandleShowDefeatByPlayerUI);
            _startOverButton.interactable = PhotonNetwork.IsMasterClient;
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowDefeatScreenUI, HandleShowScreenUI);
            EventManager.RemoveEventListener(EventNames.ShowDefeatByZoneUI, HandleShowDefeatByZoneUI);
            EventManager.RemoveEventListener(EventNames.ShowDefeatByPlayerUI, HandleShowDefeatByPlayerUI);
        }

        protected override void HandleShowScreenUI(BytesData data)
        {
            PlayerPrefs.SetInt(Config.GAME_NIGHT_KEY, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0));
            base.HandleShowScreenUI(data);
        }

        private void HandleShowDefeatByZoneUI(BytesData data)
        {
            _sheepLost.SetActive(true);
        }

        private void HandleShowDefeatByPlayerUI(BytesData data)
        {
            _playerLost.SetActive(true);
        }

        public virtual void Btn_Restart()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.LoadLevel("Lobby");
        }
    }
}
