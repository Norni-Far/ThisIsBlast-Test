namespace Blast.Core
{
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

        private LinesController _linesController;

        public readonly CubeStateWaiting _cubeStateWaiting = new CubeStateWaiting();
        public readonly CubeStateTurnLeft _cubeStateTurnLeft = new CubeStateTurnLeft();
        public readonly CubeStateTurnRight _cubeStateTurnRight = new CubeStateTurnRight();
        public readonly CubeStateDead _cubeStateDead = new CubeStateDead();

        public void SetDependencies(LinesController linesController)
        {
            _linesController = linesController;
        }

        public void SetCubeData(CubeData cube)
        {
            _meshRenderer.material = _soGameData.CubeColorsData.Find(x => x.CubeColor == cube.CubeType).Material;
            _cubeData = cube;
            SetState(_cubeStateWaiting);
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
            SetState(_cubeStateTurnLeft);
        }

        [Button]
        public void SetTurnRight()
        {
            SetState(_cubeStateTurnRight);
        }

        [Button]
        public void SetDeadState()
        {
            SetState(_cubeStateDead);
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
            _linesController.ReleaseCube(this);
        }

    }

}