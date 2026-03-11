using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameProcessController : MonoBehaviour
{
    [SerializeField] private SOLevelData _soLevelData;
    [SerializeField] private LinesController _linesController;
    [SerializeField] private TowerBase _towerBase;

    public void StartGame(int levelIndex)
    {
        _linesController.ReleaseLines();
        _linesController.FillLinesData(_soLevelData.LevelData[levelIndex].LineFillData);

        _towerBase.Release();
        _towerBase.PrepareLevelData(_soLevelData.LevelData[levelIndex]);
    }

    [Button]
    public void RestartGame()
    {
        StartGame(0);
    }
}