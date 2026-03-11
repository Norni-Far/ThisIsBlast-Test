using System;
using System.Collections;
using UnityEngine;

public class CubeStateSmallShake : ICubeState
{
    private const string SMALL_SHAKE_ANIMATION_TRIGGER = "smallShake";

    private Cube _cube;
    public void Enter(Cube cube)
    {
        _cube = cube;
        cube.GetAnimator().SetTrigger(SMALL_SHAKE_ANIMATION_TRIGGER);
        _cube.StartCoroutine(_cube.SetAfterDelayStateCoroutine(0.5f, new CubeStateWaiting()));
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}