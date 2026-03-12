namespace Blast.Core
{
    using UnityEngine;

    public class SignalsFromAnimation : MonoBehaviour
    {
        [SerializeField] private Cube _cube;

        public void OnRemoveCubeFromLine()
        {
            _cube.RemoveFromLine();
        }

        public void OnDestroyCubeFromAnimation()
        {
            _cube.Release();
        }

        public void OnTurnLeftEnd()
        {
            _cube.SetDeadState();
        }

        public void OnTurnRightEnd()
        {
            _cube.SetDeadState();
        }
    }
}
