using System.Collections;
using UnityEngine;

public class Bullet : AMoveable
{
    [SerializeField] private float _offsedFromTarget = 10f;
    [SerializeField] private TrailRenderer _trailRenderer;

    private Coroutine _moveToTargetCoroutine;
    public void SetTarget(Cube cube)
    {
        if (_moveToTargetCoroutine == null)
        {
            _moveToTargetCoroutine = StartCoroutine(StartMoveToTargetCoroutine(cube));
        }
    }

    private IEnumerator StartMoveToTargetCoroutine(Cube target)
    {
        yield return StartCoroutine(MoveToTargetCoroutine(target.GetTransform(), null, _offsedFromTarget));

        Debug.Log("Bullet reached target");

        if (transform.position.x > target.GetTransform().position.x)
        {
            target.SetTurnLeft();
        }
        else
        {
            target.SetTurnRight();
        }

        //target.SetDeadState();
        _moveToTargetCoroutine = null;
        gameObject.SetActive(false);
        TowerBase.Instance.ReleaseBullet(this);
    }

    public void ClearTrail()
    {
        _trailRenderer.Clear();
    }
}