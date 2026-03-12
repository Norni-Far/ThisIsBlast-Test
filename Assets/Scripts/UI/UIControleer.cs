using System.Collections.Generic;
using System.Runtime.InteropServices;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class UIControleer : MonoBehaviour
{
    public const int COUNT_COINS_FROM_ANIM = 4;
    public const string ANIMATOR_COUNT_COINS_PARAM_NAME = "countCoins";

    [SerializeField] private ResultsPanel _resultsPanel;
    [SerializeField] private OutOfSpacePanel _outOfSpacePanel;
    [SerializeField] private LoadingPanel _loadingPanel;
    [SerializeField] private TMPro.TextMeshProUGUI _textCountCoins;
    [SerializeField] private Animator _animatorCountCoins;
    [SerializeField] private Transform _transformGameCompleted;
    [SerializeField] private Button _buttonRestartApp;

    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI _textProgressLevel;
    [SerializeField] private Slider _progressLevelSlider;

    private void OnEnable()
    {
        _buttonRestartApp.onClick.AddListener(RestartApp);
    }

    private void OnDisable()
    {
        _buttonRestartApp.onClick.RemoveListener(RestartApp);
    }

    public void SetCurrentLevelIndex(int levelIndex)
    {
        _textProgressLevel.text = $"Level {levelIndex}";
        UpdateTextCountCoins();
    }

    public void UpdateProgressLevel(int maxCountCubes, int currentCountCubes)
    {
        _progressLevelSlider.value = 1 - (float)currentCountCubes / maxCountCubes;
    }

    public void ShowResultsPanel(ResultsPanel.ResultData resultData)
    {
        _resultsPanel.SetResult(resultData);
        _animatorCountCoins.SetTrigger(ANIMATOR_COUNT_COINS_PARAM_NAME);
    }

    private void UpdateTextCountCoins()
    {
        _textCountCoins.text = GameProcessController.Instance.GetCountCoins().ToString();
    }

    public void UpdateCountCoinsFromAnim()
    {
        AudioController.Instance.PlayAudio(AudioController.AudioType.CoinCollect);
        GameProcessController.Instance.AddCountCoins(COUNT_COINS_FROM_ANIM);
        UpdateTextCountCoins();
    }

    public void ShowOutOfSpacePanel()
    {
        _outOfSpacePanel.Show();
    }

    public void HidePanels()
    {
        _resultsPanel.Hide();
        _outOfSpacePanel.Hide();
    }

    public void ShowLoadingPanel()
    {
        _loadingPanel.Show();
    }

    public void SetLoading(float loading)
    {
        _loadingPanel.SetLoading(loading);
    }

    public void SignalFinishLoading()
    {
        _loadingPanel.SignalFinish();
    }

    public void ShowGameCompleted()
    {
        _transformGameCompleted.gameObject.SetActive(true);
    }

    public void RestartApp()
    {
        GameProcessController.Instance.RestartApplication();
    }

    public void HideLoadingPanel()
    {
        _loadingPanel.Hide();
    }
}