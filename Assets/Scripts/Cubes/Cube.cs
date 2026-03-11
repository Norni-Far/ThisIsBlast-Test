using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private SOGameData _soGameData;

    [Space]
    [SerializeField][ReadOnly] private CubeData _cubeData;

    private ICubeState _currentState;
    private Line _line;

    [SerializeField][ReadOnly] private bool _isLive;
    public bool IsLive { get { return _isLive; } private set { _isLive = value; } }

    public void SetCubeData(CubeData cube)
    {
        _meshRenderer.material = _soGameData.CubeColorsData.Find(x => x.CubeColor == cube.CubeType).Material;
        _cubeData = cube;
        SetState(new CubeStateWaiting());
        IsLive = true;
    }

    public void SetState(ICubeState state)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState?.Enter(this);
    }

    public void UpdateState()
    {
        _currentState?.Update();
    }

    public void ExitState()
    {
        _currentState?.Exit();
        _currentState = null;
    }

    public IEnumerator SetAfterDelayStateCoroutine(float delay, ICubeState state)
    {
        yield return new WaitForSeconds(delay);
        SetState(state);
    }

    public void SetLine(Line line)
    {
        _line = line;
    }

    public Animator GetAnimator() => _animator;
    public CubeData.CubeColor GetCubeColor() => _cubeData.CubeType;
    public Transform GetTransform() => transform;

    [Button]
    public void SetTurnLeft()
    {
        //IsLive = false;
        SetState(new CubeStateTurnLeft());
    }

    [Button]
    public void SetTurnRight()
    {
        //IsLive = false;
        SetState(new CubeStateTurnRight());
    }

    [Button]
    public void SetDeadState()
    {
        SetState(new CubeStateDead());
    }

    public void SetIsLiveFalse()
    {
        IsLive = false;
    }

    public void RemoveFromLine()
    {
        _line?.RemoveCube(this);
    }

    public void Release()
    {
        _line = null;
        LinesController.Instance.ReleaseCube(this);
    }

}