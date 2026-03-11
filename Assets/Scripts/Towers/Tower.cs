using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Tower : AMoveable
{
    [SerializeField] private TMPro.TextMeshProUGUI _countBulletsText;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private SOGameData _soGameData;
    [SerializeField] private float _attackInterval = 1f;
    [SerializeField] private float _waitIntervalBeforeAttack = 1f;
    [SerializeField] private float _bulletOffsetZ = 1f;

    [Space]
    [SerializeField][ReadOnly] private int IndexPositionOnRoad;
    [SerializeField][ReadOnly] private int CountBullets;
    [SerializeField][ReadOnly] private TowerData _towerData;
    [SerializeField][ReadOnly] private int _lastTargetLineIndex = 0;

    private TowerPlacePoint _emptyTowerPlacePoint;
    private Coroutine _moveToNewPositionCoroutine;
    private Coroutine _attackCoroutine;

    public void SetData(TowerData towerData)
    {
        _towerData = towerData;
        IndexPositionOnRoad = _towerData.StartIndexPositionOnRoad;
        CountBullets = _towerData.CountBullets;
        UpdateMaterial();
        UpdateCountBulletsText();
    }

    private void UpdateMaterial()
    {
        _meshRenderer.material = _soGameData.CubeColorsData.Find(x => x.CubeColor == _towerData.CubeColor).Material;
    }

    private void UpdateCountBulletsText()
    {
        _countBulletsText.text = CountBullets.ToString();
    }

    [Button]
    public void UpdatePosition()
    {
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
                targetPosition = emptyTowerPlacePoint.Transform;
                _towerData.Road.RemoveTower(this);
                _towerData.Road.UpdateAllTowers();
            }
        }
        else
        {
            emptyTowerPlacePoint = null;
            IndexPositionOnRoad--;
            targetPosition = _towerData.Road.GetTransformByIndex(IndexPositionOnRoad);
        }

        // 
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
            emptyTowerPlacePoint.Tower = this;
        }
        float z = transform.position.z;

        yield return StartCoroutine(MoveToTargetCoroutine(targetTransform));

        transform.SetParent(targetTransform);
        transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, z);
        transform.localRotation = Quaternion.identity;

        if (emptyTowerPlacePoint != null)
        {
            if (_attackCoroutine != null)
            {
                Debug.LogError("Attack coroutine is already started");
            }
            else
            {
                _attackCoroutine = StartCoroutine(AttackCoroutine());
            }
        }

        _moveToNewPositionCoroutine = null;
    }

    private IEnumerator AttackCoroutine()
    {
        Debug.Log("<color=yellow>Attack coroutine started</color>");

        while (CountBullets > 0)
        {
            Cube targetCube = LinesController.Instance.GetNearestCube(_towerData.CubeColor, ref _lastTargetLineIndex);

            if (targetCube == null)
            {
                Debug.Log("<color=red>Target cube not found</color>");
                yield return new WaitForSeconds(_waitIntervalBeforeAttack);
                continue;
            }
            else
            {
                Attack(targetCube);
                UpdateCountBulletsText();
            }

            yield return new WaitForSeconds(_attackInterval);
        }

        Debug.Log("<color=green>Attack coroutine finished</color>");
        _attackCoroutine = null;

        StartCoroutine(LeaveCoroutine());
    }

    private void Attack(Cube target)
    {
        Bullet bullet = TowerBase.Instance.GetBullet();
        bullet.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + _bulletOffsetZ);
        bullet.SetTarget(target);
        RotateTowardsTarget(transform.position, target.GetTransform().position);
        CountBullets--;
        target.SetIsLiveFalse();
    }

    private IEnumerator LeaveCoroutine()
    {
        int targetXPosition = transform.position.x > 0 ? 4 : -4;

        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);

        yield return StartCoroutine(MoveToTargetCoroutine(null, targetPosition));

        targetPosition = new Vector3(transform.position.x + targetXPosition, transform.position.y, transform.position.z);

        yield return StartCoroutine(MoveToTargetCoroutine(null, targetPosition));

        ReleaseTower();
    }

    public bool IsLastPositionOnRoad()
    {
        return IndexPositionOnRoad == 0;
    }

    private void ReleaseTower()
    {
        _emptyTowerPlacePoint.Release();
        _emptyTowerPlacePoint = null;
        _moveToNewPositionCoroutine = null;
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