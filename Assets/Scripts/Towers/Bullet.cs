namespace Blast.Core
{
    using System.Collections;
    using UnityEngine;

    public class Bullet : AMoveable
    {
        [SerializeField] private float _offsedFromTarget = 10f;
        [SerializeField] private TrailRenderer _trailRenderer;

        private Coroutine _moveToTargetCoroutine;
        public void SetTarget(Cube cube)
        {
            ClearTrail();
            if (_moveToTargetCoroutine == null)
            {
                _moveToTargetCoroutine = StartCoroutine(StartMoveToTargetCoroutine(cube));
            }
        }

        private IEnumerator StartMoveToTargetCoroutine(Cube target)
        {
            yield return StartCoroutine(MoveToTargetCoroutine(target.GetTransform(), false, null, _offsedFromTarget));

            Debug.Log("Bullet reached target");

            EffectShower effectShower = TowerBase.Instance.GetEffectShower();
            effectShower.transform.position = new Vector3(target.GetTransform().position.x, target.GetTransform().position.y, target.GetTransform().position.z - 0.4f);
            effectShower.ShowEffect();

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
            ClearTrail();
        }

        private void ClearTrail()
        {
            _trailRenderer.Clear();
        }
    }
}