using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Pool;

public class TowerBase : MonoBehaviour
{
    public static TowerBase Instance;

    [SerializeField] private List<TowerPlacePoint> _towerPlacePointsAll;
    [SerializeField] private List<Road> _roadsAll;

    [Space]
    [SerializeField][ReadOnly] private List<TowerPlacePoint> _towerPlacePointsActive;
    [SerializeField][ReadOnly] private List<Road> _roadsActive;

    [Space]
    [SerializeField] private ChildrenCenterAligner _towerPlacePointsAligner;
    [SerializeField] private ChildrenCenterAligner _roadsAligner;


    [Header("Pool Objects")]
    [SerializeField] private Tower _towerPrefab;
    [SerializeField] private Transform _towerPoolParent;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletsPoolParent;
    [SerializeField] private int _poolTowerSize;
    [SerializeField] private int _poolBulletSize;

    private ObjectPool<Bullet> _bulletPool;
    private ObjectPool<Tower> _towerPool;

    private void Awake()
    {
        Instance = this;
    }

    public void CreatePoolObjects()
    {
        _towerPool = PoolCreator.CreatePool(_towerPrefab, _towerPoolParent, _poolTowerSize, _poolTowerSize, OnGetTower);
        _bulletPool = PoolCreator.CreatePool(_bulletPrefab, _bulletsPoolParent, _poolBulletSize, _poolBulletSize, OnGetBullet);
    }

    #region Control Pool Objects

    private void OnGetTower(Tower tower)
    {
        tower.gameObject.SetActive(true);
    }

    private void OnGetBullet(Bullet bullet)
    {
        bullet.ClearTrail();
        bullet.gameObject.SetActive(true);
    }

    #endregion

    public void PrepareLevelData(LevelData levelData)
    {
        if (levelData.TowersPointCount > _towerPlacePointsAll.Count)
        {
            Debug.LogError($"Towers point count {levelData.TowersPointCount} is greater than tower place points count {_towerPlacePointsAll.Count}, level data: {levelData.LevelNumber}");
            return;
        }

        if (levelData.RoadsCount > _roadsAll.Count)
        {
            Debug.LogError($"Roads count {levelData.RoadsCount} is greater than roads count {_roadsAll.Count}, level data: {levelData.LevelNumber}");
            return;
        }

        for (int i = 0; i < levelData.TowersPointCount; i++)
        {
            Transform towerPlacePoint = _towerPlacePointsAll[i].Transform;
            towerPlacePoint.gameObject.SetActive(true);
            _towerPlacePointsActive.Add(_towerPlacePointsAll[i]);
        }

        for (int i = 0; i < levelData.RoadsCount; i++)
        {
            Road road = _roadsAll[i];
            road.gameObject.SetActive(true);
            _roadsActive.Add(road);
        }

        _towerPlacePointsAligner.AlignChildren();
        _roadsAligner.AlignChildren();

        SetTowersOnRoads(levelData);
    }

    private void SetTowersOnRoads(LevelData levelData)
    {
        foreach (var lineFill in levelData.LineFillData)
        {
            TowerData towerData = GetTowerData(lineFill);
            Tower tower = _towerPool.Get();

            Transform towersParent = towerData.Road.GetTransformByIndex(towerData.StartIndexPositionOnRoad);

            tower.SetData(towerData);
            tower.transform.position = towersParent.position;
            tower.transform.SetParent(towersParent);
            towerData.Road.AddTower(tower);
        }
    }

    private TowerData GetTowerData(LineFillData lineFillData)
    {
        Road road = GetRoadWithMinCountTowers();

        return new TowerData()
        {
            StartIndexPositionOnRoad = road.GetCountTowersOnRoad(),
            CubeColor = lineFillData.CubeData.CubeType,
            CountBullets = lineFillData.CubesCountOnLine * lineFillData.LinesCount,
            Road = road,
        };
    }

    public TowerPlacePoint GetEmptyTowerPlacePoint()
    {
        return _towerPlacePointsActive.FirstOrDefault(x => x.Tower == null);
    }

    private Road GetRoadWithMinCountTowers()
    {
        return _roadsActive.OrderBy(x => x.GetCountTowersOnRoad()).First();
    }

    public Bullet GetBullet()
    {
        return _bulletPool.Get();
    }

    public void ReleaseBullet(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    public void ReleaseTower(Tower tower)
    {
        _towerPool.Release(tower);
        tower.gameObject.SetActive(false);
        tower.transform.SetParent(_towerPoolParent);
        tower.transform.position = Vector3.zero;
        tower.transform.localRotation = Quaternion.identity;
    }

    public void Release()
    {
        foreach (var tower in _towerPlacePointsAll)
        {
            tower.Release();
            tower.Transform.gameObject.SetActive(false);
        }
        foreach (var road in _roadsAll)
        {
            road.Release();
            road.gameObject.SetActive(false);
        }
    }


}
