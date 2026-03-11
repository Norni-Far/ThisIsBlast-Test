using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Line : MonoBehaviour
{

    [SerializeField] private float _spaceBetweenCubes;

    [Space]
    [SerializeField] private Transform _lineTransform;
    [SerializeField] private float _moveLineSpeed;
    [SerializeField] private int _maxShakeCubes;

    [Space]
    [Header("Runtime Data")]
    [SerializeField][ReadOnly] private int _cubesCount;
    [SerializeField][ReadOnly] private List<Cube> _cubes;
    [Space]
    [SerializeField][ReadOnly] private float _startLineYPosition;
    [SerializeField][ReadOnly] private float _currentLineYPosition;

    private Coroutine _moveLineCoroutine;

    private void Start()
    {
        UpdateStartLineYPosition();
    }

    private void UpdateStartLineYPosition()
    {
        _startLineYPosition = _lineTransform.position.y;
        _currentLineYPosition = _startLineYPosition;
    }

    public void AddCube(Cube cube)
    {
        cube.transform.SetParent(_lineTransform);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localPosition = new Vector3(0, _spaceBetweenCubes * _cubesCount, 0);

        _cubesCount++;
        _cubes.Add(cube);
        cube.SetLine(this);
    }

    public void RemoveCube(Cube cube)
    {
        _cubes.Remove(cube);
        _cubesCount--;

        if (_cubesCount < 0)
        {
            Debug.LogError("Cubes count is less than 0");
            _cubesCount = 0;
        }

        _currentLineYPosition -= _spaceBetweenCubes;

        if (_moveLineCoroutine == null)
        {
            _moveLineCoroutine = StartCoroutine(MoveLineToTargetPosition());
        }
    }

    private IEnumerator MoveLineToTargetPosition()
    {
        while (_lineTransform.position.y >= _currentLineYPosition)
        {
            _lineTransform.position = new Vector3(_lineTransform.position.x, _lineTransform.position.y - _moveLineSpeed * Time.deltaTime, _lineTransform.position.z);
            yield return null;
        }

        _lineTransform.position = new Vector3(_lineTransform.position.x, _currentLineYPosition, _lineTransform.position.z);

        SetShakeCubes();
        _moveLineCoroutine = null;
    }

    private void SetShakeCubes()
    {
        int countShakeCubes = 0;
        foreach (var cube in _cubes)
        {
            if (countShakeCubes >= _maxShakeCubes)
            {
                break;
            }
            countShakeCubes++;
            cube.SetState(new CubeStateSmallShake());
        }
    }

    public Cube GetCubeByIndex(int index)
    {
        if (index < 0 || index >= _cubes.Count)
        {
            return null;
        }

        return _cubes[index];
    }

    // public Cube GetFierstCubeOrSecond()
    // {
    //     if (_cubes.Count == 0)
    //     {
    //         return null;
    //     }

    //     return _cubes[0];
    // }

    // public Cube GetSecondCube()
    // {
    //     if (_cubes.Count < 2)
    //     {
    //         return null;
    //     }

    //     return _cubes[1];
    // }



    public void Release()
    {
        foreach (var cube in _cubes)
        {
            cube.Release();
        }
        _cubes.Clear();

        _cubesCount = 0;
        UpdateStartLineYPosition();

        if (_moveLineCoroutine != null)
        {
            StopCoroutine(_moveLineCoroutine);
        }

        _moveLineCoroutine = null;

    }
}
