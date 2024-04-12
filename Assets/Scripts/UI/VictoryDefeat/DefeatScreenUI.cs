using Bytes;
using Photon.Pun;
using UnityEngine;

namespace Kraken.UI
{
    public class DefeatScreenUI : BaseEndGameScrenUI
    {
        [SerializeField] private GameObject _sheepLost;
        [SerializeField] private GameObject _playerLost;
        private void Start()
        {
            EventManager.AddEventListener(EventNames.ShowDefeatScreenUI, HandleShowScreenUI);
            EventManager.AddEventListener(EventNames.ShowDefeatByZoneUI, HandleShowDefeatByZoneUI);
            EventManager.AddEventListener(EventNames.ShowDefeatByPlayerUI, HandleShowDefeatByPlayerUI);
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
            PhotonNetwork.LoadLevel("Lobby");
        }
    }
}
