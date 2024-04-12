using Bytes;
using Photon.Pun;
using UnityEngine;

namespace Kraken.UI
{
    public class DefeatScreenUI : BaseEndGameScrenUI
    {
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
            
        }

        private void HandleShowDefeatByPlayerUI(BytesData data)
        {
            
        }

        public virtual void Btn_Restart()
        {
            PhotonNetwork.LoadLevel("Lobby");
        }
    }
}
