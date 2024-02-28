
using UnityEngine;

using Bytes;

namespace Kraken.UI
{
    public abstract class BaseEndGameScrenUI : KrakenUIElement
    {
        protected virtual void HandleShowScreenUI(BytesData data)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetVisible(true);
        }

        public virtual void Btn_BackToTitle()
        {
            EventManager.Dispatch(EventNames.LeaveGame, null);
        }
    }
}
