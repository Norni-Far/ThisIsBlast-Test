namespace Blast.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using NaughtyAttributes;
    using UnityEngine;

    public class Tower : AMoveable
    {
        [SerializeField] private HighlightPlus.HighlightEffect _highlightEffect;
        [SerializeField] private TMPro.TextMeshProUGUI _countBulletsText;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private SOGameData _soGameData;
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _particleSystemMerge;
        [SerializeField] private ParticleSystem _particleSystemClow;

        [Space]
        [SerializeField] private float _attackInterval = 1f;
        [SerializeField] private float _waitIntervalBeforeAttack = 1f;
        [SerializeField] private float _bulletOffsetZ = 1f;
        [SerializeField] private float _bulletOffsetY = 1f;
        [SerializeField] private int _maxCountMissAttack = 4;

        [Space]
        [SerializeField] private int IndexPositionOnRoad;
        [SerializeField][ReadOnly] private int CountBullets;
        [SerializeField][ReadOnly] private TowerData _towerData;
        [SerializeField][ReadOnly] private int _lastTargetLineIndex = 0;
        [SerializeField][ReadOnly] private int _countMissAttack = 0;
        [SerializeField][ReadOnly] private bool _iCantAttack = true;

        private LinesController _linesController;
        private TowerPlacePoint _emptyTowerPlacePoint;
        private Coroutine _moveToNewPositionCoroutine;
        private Coroutine _attackCoroutine;

        public void SetDependencies(LinesController linesController)
        {
            _linesController = linesController;
        }

        private void Start()
        {
            StartCoroutine(EnableAnimatorCoroutine());
        }

        private IEnumerator EnableAnimatorCoroutine()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
            _animator.enabled = true;
        }

        public void SetData(TowerData towerData)
        {
            _towerData = towerData;
            IndexPositionOnRoad = _towerData.StartIndexPositionOnRoad;
            CountBullets = _towerData.CountBullets;
            UpdateMaterial();
            UpdateCountBulletsText();
            UpdateHighlightEffect();
        }

        private void UpdateMaterial()
        {
            _meshRenderer.material = _soGameData.CubeColorsData.Find(x => x.CubeColor == _towerData.CubeColor).Material;
        }

        private void UpdateCountBulletsText()
        {
            _countBulletsText.text = CountBullets.ToString();
        }

        private void UpdateHighlightEffect()
        {
            if (IndexPositionOnRoad == 0)
            {
                _highlightEffect.SetHighlighted(true);
            }
            else
            {
                _highlightEffect.SetHighlighted(false);
            }
        }

        public void AddBullets(int countBullets)
        {
            CountBullets += countBullets;
            UpdateCountBulletsText();
        }

        [Button]
        public void UpdatePosition()
        {
            if (_moveToNewPositionCoroutine != null)
            {
                return;
            }

            Transform targetPosition;
            TowerPlacePoint emptyTowerPlacePoint = TowerBase.Instance.GetEmptyTowerPlacePoint();

            if (IndexPositionOnRoad == 0)
            {
                if (emptyTowerPlacePoint == null)
                {
                    Debug.Log(" <color=red>Empty tower place point not found</color>");
                    return;
                }
                else
                {
                    _emptyTowerPlacePoint = emptyTowerPlacePoint;
                    targetPosition = emptyTowerPlacePoint.GetTransform();
                    _towerData.Road.RemoveTower(this);
                    _towerData.Road.UpdateAllTowers();
                    IndexPositionOnRoad--;
                }
            }
            else
            {
                emptyTowerPlacePoint = null;
                IndexPositionOnRoad--;
                targetPosition = _towerData.Road.GetTransformByIndex(IndexPositionOnRoad);
            }

            AudioController.Instance.PlayAudio(AudioController.AudioType.Click);

            if (_moveToNewPositionCoroutine == null)
            {
                _moveToNewPositionCoroutine = StartCoroutine(MoveToNewPositionCoroutine(targetPosition, emptyTowerPlacePoint));
            }
        }

        private IEnumerator MoveToNewPositionCoroutine(Transform targetTransform, TowerPlacePoint emptyTowerPlacePoint)
        {
            if (targetTransform == null)
            {
                Debug.LogError(" <color=red>Target transform is null</color>");
                _moveToNewPositionCoroutine = null;
                yield break;
            }

            if (emptyTowerPlacePoint != null)
            {
                emptyTowerPlacePoint.SetTower(this);
            }
            float z = transform.position.z;

            yield return StartCoroutine(MoveToTargetCoroutine(targetTransform));

            transform.SetParent(targetTransform);
            transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, z);
            transform.localRotation = Quaternion.identity;

            UpdateHighlightEffect();

            if (emptyTowerPlacePoint != null)
            {
                if (_attackCoroutine != null)
                {
                    StopCoroutine(_attackCoroutine);
                }

                _attackCoroutine = StartCoroutine(AttackCoroutine());
            }

            _moveToNewPositionCoroutine = null;
        }

        private IEnumerator AttackCoroutine()
        {
            while (CountBullets > 0)
            {
                Cube targetCube = _linesController.GetNearestCube(_towerData.CubeColor, ref _lastTargetLineIndex);

                if (targetCube == null)
                {
                    _countMissAttack++;

                    if (_countMissAttack >= _maxCountMissAttack)
                    {
                        _countMissAttack = _maxCountMissAttack;
                        _iCantAttack = false;
                    }

                    RotateTowardsTarget(transform.position, transform.position + new Vector3(0, 1, 0));
                    yield return new WaitForSeconds(_waitIntervalBeforeAttack);
                    continue;
                }
                else
                {
                    _countMissAttack = 0;
                    _iCantAttack = true;
                    Attack(targetCube);
                    UpdateCountBulletsText();
                }

                yield return new WaitForSeconds(_attackInterval);
            }

            _attackCoroutine = null;

            yield return new WaitForSeconds(0.3f);

            StartCoroutine(LeaveCoroutine());
        }

        private void Attack(Cube target)
        {
            AudioController.Instance.PlayAudio(AudioController.AudioType.Attack);
            Bullet bullet = TowerBase.Instance.GetBullet();
            bullet.transform.position = new Vector3(transform.position.x, transform.position.y + _bulletOffsetY, transform.position.z + _bulletOffsetZ);
            bullet.SetTarget(target);
            RotateTowardsTarget(transform.position, target.GetTransform().position);
            CountBullets--;
            target.SetIsLiveFalse();
        }

        private IEnumerator LeaveCoroutine()
        {
            int targetXPosition = transform.position.x > 0 ? 4 : -4;

            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);

            yield return StartCoroutine(MoveToTargetCoroutine(null, true, targetPosition));

            _emptyTowerPlacePoint.Release();

            targetPosition = new Vector3(transform.position.x + targetXPosition, transform.position.y, transform.position.z);

            yield return StartCoroutine(MoveToTargetCoroutine(null, true, targetPosition));

            ReleaseTower();
        }

        public bool IsLastPositionOnRoad()
        {
            return IndexPositionOnRoad == 0;
        }

        public bool IsICantAttack()
        {
            return _iCantAttack;
        }

        public CubeData.CubeColor GetColor()
        {
            return _towerData.CubeColor;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public int GetCountBullets()
        {
            return CountBullets;
        }

        public Road GetRoad()
        {
            return _towerData.Road;
        }

        public void SetLastPositionOnTheRoad()
        {
            IndexPositionOnRoad = _towerData.Road.GetCountTowersOnRoad();
        }

        public void PlayMergeParticles()
        {
            _particleSystemClow.startColor = _soGameData.CubeColorsData.Find(x => x.CubeColor == _towerData.CubeColor).Material.color;
            _particleSystemMerge.Play();
        }

        public void ReleaseTower()
        {
            _countMissAttack = 0;
            _iCantAttack = true;
            _emptyTowerPlacePoint = null;
            _moveToNewPositionCoroutine = null;

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }
            _attackCoroutine = null;

            TowerBase.Instance.ReleaseTower(this);
        }
    }

    [Serializable]
    public class TowerData
    {
        public int StartIndexPositionOnRoad;
        public CubeData.CubeColor CubeColor;
        public int CountBullets;
        public Road Road;
    }

}