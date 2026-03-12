namespace Blast.Core
{
    using UnityEngine;

    public interface ICubeState
    {
        void Enter(Cube cube);
        void Update();
        void Exit();
    }
}