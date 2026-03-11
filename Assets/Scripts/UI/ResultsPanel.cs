using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;


public class ResultsPanel : MonoBehaviour
{
    public const string TEXT_WIN = "Victory!";
    public const string TEXT_LOSE = "Defeat!";
    public const string TEXT_NUM_LEVEL = "Level {0}";

    public const string TEXT_BUTTON_WIN = "Continue";
    public const string TEXT_BUTTON_LOSE = "Restart";

    [SerializeField] private TMPro.TextMeshProUGUI _textResult;
    [SerializeField] private TMPro.TextMeshProUGUI _textNumLevel;
    [SerializeField] private TMPro.TextMeshProUGUI _textCountCoins;

    [Space]
    [SerializeField] private Button _buttonLevel;
    [SerializeField] private TMPro.TextMeshProUGUI _textButtonLevel;

    [SerializeField][ReadOnly] private ResultData _resultData;

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
        _textResult.text = resultData.IsWin ? TEXT_WIN : TEXT_LOSE;
        _textNumLevel.text = string.Format(TEXT_NUM_LEVEL, resultData.NumLevel);
        _textCountCoins.text = resultData.CountCoins.ToString();

        _textButtonLevel.text = resultData.IsWin ? TEXT_BUTTON_WIN : TEXT_BUTTON_LOSE;

        gameObject.SetActive(true);
    }

    private void OnButtonNextLevelClick()
    {
        if (_resultData.IsWin)
        {
            GameProcessController.Instance.StartNextLevel();
        }
        else
        {
            GameProcessController.Instance.RestartGame();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    [Serializable]
    public class ResultData
    {
        public bool IsWin;
        public int NumLevel;
        public int CountCoins;
    }

}