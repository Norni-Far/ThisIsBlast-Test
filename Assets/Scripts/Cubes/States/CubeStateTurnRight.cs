using UnityEngine;

public class CubeStateTurnRight : ICubeState
{
    private const string TURN_RIGHT_ANIMATION_TRIGGER = "rightTurn";

    public void Enter(Cube cube)
    {
        cube.GetAnimator().SetTrigger(TURN_RIGHT_ANIMATION_TRIGGER);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}