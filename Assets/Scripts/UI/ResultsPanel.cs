namespace Blast.Core
{
    using System;
    using NaughtyAttributes;
    using UnityEngine;
    using UnityEngine.UI;


    public class ResultsPanel : MonoBehaviour
    {
        public const string TEXT_WIN = "Victory!";
        public const string TEXT_NUM_LEVEL = "Level {0}";

        public const string TEXT_BUTTON_WIN = "Continue";

        [SerializeField] private TMPro.TextMeshProUGUI _textResult;
        [SerializeField] private TMPro.TextMeshProUGUI _textNumLevel;
        [SerializeField] private TMPro.TextMeshProUGUI _textCountCoins;

        [Space]
        [SerializeField] private Button _buttonLevel;
        [SerializeField] private TMPro.TextMeshProUGUI _textButtonLevel;

        [SerializeField][ReadOnly] private ResultData _resultData;

        private GameProcessController _gameProcessController;

        public void SetDependencies(GameProcessController gameProcessController)
        {
            _gameProcessController = gameProcessController;
        }

        private void OnEnable()
        {
            _buttonLevel.onClick.AddListener(OnButtonNextLevelClick);
        }

        private void OnDisable()
        {
            _buttonLevel.onClick.RemoveListener(OnButtonNextLevelClick);
        }

        public void SetResult(ResultData resultData)
        {
            _textResult.text = TEXT_WIN;
            _textNumLevel.text = string.Format(TEXT_NUM_LEVEL, resultData.NumLevel);
            _textCountCoins.text = resultData.CountCoins.ToString();

            _textButtonLevel.text = TEXT_BUTTON_WIN;

            gameObject.SetActive(true);
        }

        private void OnButtonNextLevelClick()
        {
            _gameProcessController.StartNextLevel();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        [Serializable]
        public class ResultData
        {
            public int NumLevel;
            public int CountCoins;
        }

    }
}