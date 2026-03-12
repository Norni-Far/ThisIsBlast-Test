namespace Blast.Core
{
    using System;
    using System.Collections;
    using NaughtyAttributes;
    using UnityEngine;

    [Serializable]
    public class TowerPlacePoint : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField][ReadOnly] private Tower _tower;

        public Tower GetTower()
        {
            return _tower;
        }

        public void SetTower(Tower tower)
        {
            _tower = tower;
        }

        public bool IsEmpty()
        {
            return _tower == null;
        }

        public Transform GetTransform()
        {
            return _transform;
        }

        public bool IsTowerICantAttackOrNull()
        {
            if (IsEmpty())
            {
                return true;
            }

            return _tower.IsICantAttack();
        }

        public void Release()
        {
            _tower = null;
        }
    }
}