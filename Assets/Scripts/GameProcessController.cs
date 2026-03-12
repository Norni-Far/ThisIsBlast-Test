using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProcessController : MonoBehaviour
{
    public static GameProcessController Instance;

    public const int COUNT_COINS_FROM_WIN = 12;

    [SerializeField] private TouchTriggerRaycaster _touchTriggerRaycaster;
    [SerializeField] private UIControleer _uiControleer;
    [SerializeField] private FTUEController _ftueController;
    [SerializeField] private ParticleSystem _particleSystemWin;

    [Space]
    [SerializeField] private SOLevelData _soLevelData;
    [SerializeField] private LinesController _linesController;
    [SerializeField] private TowerBase _towerBase;

    [Space]
    [SerializeField][ReadOnly] private int _currentLevelNum = 0;
    [SerializeField][ReadOnly] private int _countCoins = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AudioController.Instance.PlayAudio(AudioController.AudioType.Click);
    }

    public void StartGame(int levelNum)
    {
        _uiControleer.ShowLoadingPanel();
        SetActivePlayerInput(false);

        _currentLevelNum = levelNum;

        LevelData levelData = _soLevelData.LevelData.Find(x => x.LevelNumber == levelNum);

        if (levelData == null)
        {
            Debug.LogError($"Level data not found for level number {levelNum}");
            return;
        }

        _linesController.ReleaseLines();
        _linesController.FillLinesData(levelData.LineFillData);

        _towerBase.Release();
        _towerBase.PrepareLevelData(levelData);

        _uiControleer.SetCurrentLevelIndex(levelNum);

        _ftueController.CheckIfNeedToShowFTUE(levelNum);

        _uiControleer.HidePanels();

        SetActivePlayerInput(true);
        _uiControleer.SignalFinishLoading();
    }

    public void StartNextLevel()
    {
        AudioController.Instance.PlayAudio(AudioController.AudioType.Click);
        if (_currentLevelNum >= 20)
        {
            _uiControleer.ShowGameCompleted();
            return;
        }

        StartGame(_currentLevelNum + 1);
    }

    public void OnEndLevel()
    {
        Debug.Log("<color=green> OnEndLevel</color>");

        _particleSystemWin.Play();
        AudioController.Instance.PlayAudio(AudioController.AudioType.Win);
        SetActivePlayerInput(false);
        _uiControleer.ShowResultsPanel(new ResultsPanel.ResultData()
        {
            NumLevel = _currentLevelNum,
            CountCoins = COUNT_COINS_FROM_WIN
        });

    }

    public void OnSetProgressLevel(int maxCountCubes, int currentCountCubes)
    {
        _uiControleer.UpdateProgressLevel(maxCountCubes, currentCountCubes);
    }

    public void SetActivePlayerInput(bool isActive)
    {
        _touchTriggerRaycaster.IsEnabled = isActive;
    }

    public void OnOutOfSpace()
    {
        SetActivePlayerInput(false);
        _uiControleer.ShowOutOfSpacePanel();
    }

    public void AddCountCoins(int countCoins)
    {
        _countCoins += countCoins;
    }

    public int GetCountCoins()
    {
        return _countCoins;
    }

    [Button]
    public void RestartGame()
    {
        AudioController.Instance.PlayAudio(AudioController.AudioType.Click);
        StartGame(_currentLevelNum);
    }

    public void RestartApplication()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}