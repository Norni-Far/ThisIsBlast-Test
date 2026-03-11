using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameProcessController : MonoBehaviour
{
    public static GameProcessController Instance;

    [SerializeField] private UIControleer _uiControleer;

    [Space]
    [SerializeField] private SOLevelData _soLevelData;
    [SerializeField] private LinesController _linesController;
    [SerializeField] private TowerBase _towerBase;

    [Space]
    [SerializeField][ReadOnly] private int _currentLevelIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame(int levelIndex)
    {
        _currentLevelIndex = levelIndex;

        _linesController.ReleaseLines();
        _linesController.FillLinesData(_soLevelData.LevelData[levelIndex].LineFillData);

        _towerBase.Release();
        _towerBase.PrepareLevelData(_soLevelData.LevelData[levelIndex]);
        _uiControleer.HidePanels();
    }

    public void StartNextLevel()
    {
        StartGame(_currentLevelIndex + 1);
    }

    public void OnEndLevel(bool isWin)
    {
        _uiControleer.ShowResultsPanel(new ResultsPanel.ResultData()
        {
            IsWin = isWin,
            NumLevel = _currentLevelIndex,
            CountCoins = isWin ? 12 : 0
        });
    }

    public void OnOutOfSpace()
    {
        _uiControleer.ShowOutOfSpacePanel();
    }

    [Button]
    public void RestartGame()
    {
        StartGame(_currentLevelIndex);
    }
}