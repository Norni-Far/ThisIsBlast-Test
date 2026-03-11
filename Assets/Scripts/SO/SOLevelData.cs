using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "LevelData")]
public class SOLevelData : ScriptableObject
{
    public List<LevelData> LevelData;
}

[Serializable]
public class LevelData
{
    public int LevelNumber;
    public int TowersPointCount;
    public int RoadsCount;

    public List<LineFillData> LineFillData;
}

[Serializable]
public class LineFillData
{
    public CubeData CubeData;
    public int LinesCount;
    public int CubesCountOnLine;
}