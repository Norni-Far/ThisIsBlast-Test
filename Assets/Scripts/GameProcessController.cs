using System.Collections.Generic;
using UnityEngine;

public class GameProcessController : MonoBehaviour
{
    [SerializeField] private SOLevelData _soLevelData;
    [SerializeField] private LinesController _linesController;
    [SerializeField] private TowerBase _towerBase;

    public void StartGame()
    {
        _linesController.ReleaseLines();
        _linesController.FillLinesData(_soLevelData.LevelData[0].LineFillData);

        _towerBase.Release();
        _towerBase.PrepareLevelData(_soLevelData.LevelData[0]);
    }
}