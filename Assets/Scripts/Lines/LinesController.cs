using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
using NaughtyAttributes;

public class LinesController : MonoBehaviour
{
    public static LinesController Instance;

    [SerializeField] private List<Line> _lines;

    [Space]
    [Header("Pool Objects")]
    [SerializeField] private Transform _cubesPoolParent;
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private int _poolSize;

    [Space]
    [SerializeField][ReadOnly] private int _lastLineIndex = -1;

    private ObjectPool<Cube> _poolObjects;

    private void Awake()
    {
        Instance = this;
    }

    public void CreatePoolObjects()
    {
        _poolObjects = PoolCreator.CreatePool(_cubePrefab, _cubesPoolParent, _poolSize, _poolSize, OnGetCube);
    }

    #region Control Pool Objects

    private void OnGetCube(Cube cube)
    {
        cube.gameObject.SetActive(true);
    }

    #endregion

    public void ReleaseCube(Cube cube)
    {
        _poolObjects.Release(cube);
        cube.transform.SetParent(_cubesPoolParent);
        cube.transform.position = Vector3.zero;
        cube.gameObject.SetActive(false);

        CheckEndGame();
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

        GameProcessController.Instance.OnEndLevel(true);
    }

    public void FillLinesData(List<LineFillData> lineFillData)
    {
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
                var cube = _poolObjects.Get();
                cube.SetCubeData(lineFillData.CubeData);
                line.AddCube(cube);
            }
        }
    }

    public Cube GetNearestCube(CubeData.CubeColor color, ref int lastLineIndex)
    {
        int countLinesForAttack = 3;

        for (int x = 0; x < countLinesForAttack; x++)
        {
            for (int i = lastLineIndex; i < _lines.Count; i++)
            {
                var cube = _lines[i].GetCubeByIndex(x);

                // if (x > 0)
                // {
                //     Cube previousCube = _lines[i].GetCubeByIndex(x - 1);
                //     if (previousCube != null)
                //     {
                //         if (previousCube.GetCubeColor() != color)
                //         {
                //             lastLineIndex = 0;
                //             return null;
                //         }
                //     }
                // }

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

    public void ReleaseLines()
    {
        _lastLineIndex = -1;

        foreach (var line in _lines)
        {
            line.Release();
        }
    }

}
