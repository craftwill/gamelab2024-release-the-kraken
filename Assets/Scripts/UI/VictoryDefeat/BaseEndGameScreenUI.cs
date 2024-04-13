
using UnityEngine;

using Bytes;

namespace Kraken.UI
{
    public abstract class BaseEndGameScrenUI : KrakenUIElement
    {
        private int _maxNight = 0;
        private int _night = 0;
        [SerializeField] protected Transform _maxScoreObject;
        [SerializeField] protected Transform _currentScoreObject;
        [SerializeField] protected GameObject[] _spritesPrefab;
        protected virtual void HandleShowScreenUI(BytesData data)
        {
            GameManager.ToggleCursor(true);
            EventManager.Dispatch(EventNames.HideHUD, null);
            SetVisible(true);

            _maxNight = PlayerPrefs.GetInt(Config.GAME_MAX_NIGHT_KEY, 0);
            _night = PlayerPrefs.GetInt(Config.GAME_NIGHT_KEY, 0);

            if(_night > _maxNight)
            {
                _maxNight = _night;
                PlayerPrefs.SetInt(Config.GAME_MAX_NIGHT_KEY, _maxNight);
            }

            string n = _night.ToString();
            string mn = _maxNight.ToString();

            ScoreToUI(_currentScoreObject, n);
            ScoreToUI(_maxScoreObject, mn);
        }

        private void ScoreToUI(Transform parent, string number)
        {
            for (int i = 0; i < number.Length; ++i)
            {
                int index = (int)System.Char.GetNumericValue(number[i]);
                Instantiate(_spritesPrefab[index], parent);
            }

            //layout group stupid
            parent.GetComponent<RectTransform>().sizeDelta = new Vector2(number.Length * 48, 60);
        }

        public virtual void Btn_BackToTitle()
        {
            EventManager.Dispatch(EventNames.LeaveGame, null);
        }
    }
}
