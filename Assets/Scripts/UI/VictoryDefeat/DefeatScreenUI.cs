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
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.ShowDefeatScreenUI, HandleShowScreenUI);
        }

        protected override void HandleShowScreenUI(BytesData data)
        {
            PlayerPrefs.SetInt(Config.GAME_NIGHT_KEY, PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0));
            base.HandleShowScreenUI(data);
        }

        public virtual void Btn_Restart()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.LoadLevel("Lobby");
        }
    }
}
