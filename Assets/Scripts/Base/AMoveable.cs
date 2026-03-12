namespace Blast.Core
{
    using System.Collections;
    using UnityEngine;

    public abstract class AMoveable : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _rotateSpeed;

        public IEnumerator MoveToTargetCoroutine(Transform targetTransform, bool isNeedToRotate = true, Vector3? targetPosition = null, float offsetFromTarget = 0.001f)
        {
            float z = transform.position.z;

            Vector2 targetXY = Vector2.zero;

            if (targetTransform == null)
            {
                if (targetPosition == null)
                {
                    Debug.LogError("Target position is null");
                    yield break;
                }
                targetXY = new Vector2(targetPosition.Value.x, targetPosition.Value.y);
            }
            else
            {
                targetXY = new Vector2(targetTransform.position.x, targetTransform.position.y);
            }

            while (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), targetXY) > offsetFromTarget)
            {
                if (targetTransform != null)
                {
                    targetXY = new Vector2(targetTransform.position.x, targetTransform.position.y);
                }

                Vector3 current = transform.position;
                Vector2 currentXY = new Vector2(current.x, current.y);
                Vector2 nextXY = GetNextPositionTowardsTarget(currentXY, targetXY);
                if (isNeedToRotate)
                {
                    RotateTowardsTarget(currentXY, nextXY);
                }

                transform.position = new Vector3(nextXY.x, nextXY.y, z);

                yield return null;
            }
        }

        private Vector2 GetNextPositionTowardsTarget(Vector2 currentXY, Vector2 targetXY)
        {
            return Vector2.MoveTowards(currentXY, targetXY, _moveSpeed * Time.deltaTime);
        }

        public void RotateTowardsTarget(Vector2 currentXY, Vector2 nextXY)
        {
            Vector2 moveDirXY = nextXY - currentXY;
            if (moveDirXY.sqrMagnitude <= 0.000001f)
            {
                return;
            }

            float targetZAngle = Mathf.Atan2(moveDirXY.y, moveDirXY.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }

    }
}