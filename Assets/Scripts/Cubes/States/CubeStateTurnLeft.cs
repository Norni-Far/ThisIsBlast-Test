namespace Blast.Core
{
    using UnityEngine;

    public class CubeStateTurnLeft : ICubeState
    {
        private const string TURN_LEFT_ANIMATION_TRIGGER = "leftTurn";

        public void Enter(Cube cube)
        {
            cube.GetAnimator().SetTrigger(TURN_LEFT_ANIMATION_TRIGGER);
        }

        public void Exit()
        {

        }

        public void Update()
        {

        }
    }
}