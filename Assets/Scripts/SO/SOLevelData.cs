namespace Blast.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using NaughtyAttributes;

    [CreateAssetMenu(fileName = "LevelSO", menuName = "LevelData")]
    public class SOLevelData : ScriptableObject
    {
        public List<LevelData> LevelData;

        [Header("Generation Settings")]
        [MinMaxSlider(2, 10)] public Vector2Int RoadsRange = new Vector2Int(2, 5);
        [MinMaxSlider(2, 10)] public Vector2Int TowersRange = new Vector2Int(2, 5);
        [MinMaxSlider(1, 25)] public Vector2Int StagesRange = new Vector2Int(6, 12);
        [MinMaxSlider(1, 20)] public Vector2Int LinesRange = new Vector2Int(2, 10);
        [MinMaxSlider(1, 50)] public Vector2Int CubesPerLineRange = new Vector2Int(1, 20);

        [Button("Generate Levels 4-20")]
        private void GenerateLevels()
        {
            if (LevelData == null) LevelData = new List<LevelData>();

            // Сохраняем первые 3 уровня, если они есть
            List<LevelData> newLevels = new List<LevelData>();
            for (int i = 0; i < Math.Min(LevelData.Count, 3); i++)
            {
                newLevels.Add(LevelData[i]);
            }

            CubeData.CubeColor[] allColors = (CubeData.CubeColor[])Enum.GetValues(typeof(CubeData.CubeColor));

            for (int i = 4; i <= 20; i++)
            {
                LevelData level = new LevelData();
                level.LevelNumber = i;

                level.RoadsCount = UnityEngine.Random.Range(RoadsRange.x, RoadsRange.y + 1);

                int minTowers = Mathf.Max(level.RoadsCount, TowersRange.x);
                level.TowersPointCount = UnityEngine.Random.Range(minTowers, TowersRange.y + 1);

                level.LineFillData = new List<LineFillData>();

                float progress = (float)(i - 4) / (20 - 4);
                int stagesCount = Mathf.RoundToInt(Mathf.Lerp(StagesRange.x, StagesRange.y, progress));

                int availableColorsCount = Mathf.Clamp(4 + (i - 4) / 2, 4, allColors.Length);

                for (int s = 0; s < stagesCount; s++)
                {
                    LineFillData stage = new LineFillData();
                    stage.CubeData = new CubeData();
                    stage.CubeData.CubeType = allColors[UnityEngine.Random.Range(0, availableColorsCount)];

                    stage.LinesCount = UnityEngine.Random.Range(LinesRange.x, LinesRange.y + 1);

                    int maxCubes = CubesPerLineRange.y;
                    if (stage.LinesCount >= 5)
                    {
                        maxCubes = Mathf.Min(maxCubes, 9);
                    }

                    int minCubes = Mathf.Min(CubesPerLineRange.x, maxCubes);
                    stage.CubesCountOnLine = UnityEngine.Random.Range(minCubes, maxCubes + 1);

                    level.LineFillData.Add(stage);
                }

                newLevels.Add(level);
            }

            LevelData = newLevels;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
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
}