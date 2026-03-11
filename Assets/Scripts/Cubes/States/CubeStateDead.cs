using UnityEngine;

public class CubeStateDead : ICubeState
{
    private const string DEAD_ANIMATION_TRIGGER = "dead";
    private Cube _cube;

    public void Enter(Cube cube)
    {
        _cube = cube;
        _cube.GetAnimator().SetTrigger(DEAD_ANIMATION_TRIGGER);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}