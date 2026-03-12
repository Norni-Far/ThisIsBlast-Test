namespace Blast.Core
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.Pool;
    using NaughtyAttributes;

    public class LinesController : MonoBehaviour
    {
        [SerializeField] private List<Line> _lines;

        [Space]
        [Header("Pool Objects")]
        [SerializeField] private Transform _cubesPoolParent;
        [SerializeField] private Cube _cubePrefab;
        [SerializeField] private int _poolSize;
        [SerializeField] private int _countLinesTowerCanAttack = 3;

        [Space]
        [SerializeField][ReadOnly] private int _lastLineIndex = -1;
        [SerializeField][ReadOnly] private int _maxLevelCubesCount = 0;
        [SerializeField][ReadOnly] private int _currentLevelCubesCount = 0;

        private GameProcessController _gameProcessController;
        private ObjectPool<Cube> _cubePoolObjects;
        private bool _isEndGame = false;

        public void SetDependencies(GameProcessController gameProcessController)
        {
            _gameProcessController = gameProcessController;

            foreach (var line in _lines)
            {
                line.SetDependencies(this);
            }
        }

        public void CreatePoolObjects()
        {
            _cubePoolObjects = PoolCreator.CreatePool(_cubePrefab, _cubesPoolParent, _poolSize, _poolSize, OnGetCube);
        }

        #region Control Pool Objects

        private void OnGetCube(Cube cube)
        {
            cube.gameObject.SetActive(true);
        }

        #endregion

        public void ReleaseCube(Cube cube)
        {
            _currentLevelCubesCount--;

            _cubePoolObjects.Release(cube);
            cube.transform.SetParent(_cubesPoolParent);
            cube.transform.position = Vector3.zero;
            cube.gameObject.SetActive(false);

            CheckEndGame();
            _gameProcessController.OnSetProgressLevel(_maxLevelCubesCount, _currentLevelCubesCount);
        }

        private void CheckEndGame()
        {
            foreach (var line in _lines)
            {
                if (line.GetCubeByIndex(0) != null)
                {
                    return;
                }
            }

            if (_isEndGame)
            {
                return;
            }
            _isEndGame = true;

            _gameProcessController.OnEndLevel();
        }

        public void FillLinesData(List<LineFillData> lineFillData)
        {
            _maxLevelCubesCount = 0;
            _currentLevelCubesCount = 0;

            foreach (var lineFill in lineFillData)
            {
                FillLine(lineFill);
            }
        }

        private void FillLine(LineFillData lineFillData)
        {
            for (int i = 0; i < lineFillData.LinesCount; i++)
            {
                var line = GetNextLine();
                for (int j = 0; j < lineFillData.CubesCountOnLine; j++)
                {
                    var cube = _cubePoolObjects.Get();
                    cube.SetDependencies(this);
                    cube.SetCubeData(lineFillData.CubeData);
                    line.AddCube(cube);
                    _maxLevelCubesCount++;
                }
            }

            _currentLevelCubesCount = _maxLevelCubesCount;
            _gameProcessController.OnSetProgressLevel(_maxLevelCubesCount, _currentLevelCubesCount);
        }

        public Cube GetNearestCube(CubeData.CubeColor color, ref int lastLineIndex)
        {

            for (int x = 0; x < _countLinesTowerCanAttack; x++)
            {
                for (int i = lastLineIndex; i < _lines.Count; i++)
                {
                    var cube = _lines[i].GetCubeByIndex(x);

                    if (_lines[i].GetCubeByIndex(0) != null && _lines[i].GetCubeByIndex(0).GetCubeColor() != color)
                    {
                        continue;
                    }

                    if (HasLiveCubeAbove(_lines[i], x))
                    {
                        continue;
                    }

                    if (CheckCube(cube, color) != null)
                    {
                        if (x < 1)
                        {
                            lastLineIndex = i;
                        }
                        else
                        {
                            lastLineIndex = 0;
                        }

                        return cube;
                    }
                }

                lastLineIndex = 0;
            }

            return null;
        }

        private bool HasLiveCubeAbove(Line line, int cubeIndex)
        {
            if (cubeIndex <= 0)
            {
                return false;
            }

            for (int i = 0; i < cubeIndex; i++)
            {
                Cube cubeAbove = line.GetCubeByIndex(i);
                if (cubeAbove != null && cubeAbove.IsLive)
                {
                    return true;
                }
            }

            return false;
        }

        private Cube CheckCube(Cube cube, CubeData.CubeColor color)
        {
            if (cube == null)
            {
                return null;
            }

            if (cube.GetCubeColor() == color && cube.IsLive)
            {
                return cube;
            }

            return null;
        }

        private Line GetNextLine()
        {
            _lastLineIndex++;
            if (_lastLineIndex >= _lines.Count)
            {
                _lastLineIndex = 0;
            }
            return _lines[_lastLineIndex];
        }

        public Line GetLine(int index)
        {
            if (index < 0 || index >= _lines.Count)
            {
                Debug.LogError($"Line with index {index} not found");
                return null;
            }

            return _lines[index];
        }

        public void ReleaseLines()
        {
            _isEndGame = false;
            _lastLineIndex = -1;
            _maxLevelCubesCount = 0;
            _currentLevelCubesCount = 0;

            foreach (var line in _lines)
            {
                line.Release();
            }
        }
    }
}
