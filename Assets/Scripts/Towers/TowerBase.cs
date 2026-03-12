namespace Blast.Core
{
    using System;
    using System.Collections;
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
        [SerializeField][ReadOnly] private List<Tower> _towersActive;

        [Space]
        [SerializeField] private ChildrenCenterAligner _towerPlacePointsAligner;
        [SerializeField] private ChildrenCenterAligner _roadsAligner;


        [Header("Pool Objects")]
        [SerializeField] private Tower _towerPrefab;
        [SerializeField] private Transform _towerPoolParent;
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _bulletsPoolParent;
        [SerializeField] private EffectShower _effectShowerPrefab;
        [SerializeField] private Transform _effectShowerPoolParent;
        [SerializeField] private int _poolTowerSize;
        [SerializeField] private int _poolBulletSize;
        [SerializeField] private int _poolEffectShowerSize;

        private LinesController _linesController;
        private GameProcessController _gameProcessController;

        private ObjectPool<Bullet> _bulletPool;
        private ObjectPool<Tower> _towerPool;
        private ObjectPool<EffectShower> _effectShowerPool;

        private Coroutine _checkLoseGameCoroutine;
        private Coroutine _mergeTowersCoroutine;

        private void Awake()
        {
            Instance = this;
        }

        public void SetDependencies(GameProcessController gameProcessController, LinesController linesController)
        {
            _gameProcessController = gameProcessController;
            _linesController = linesController;
        }

        public void CreatePoolObjects()
        {
            _towerPool = PoolCreator.CreatePool(_towerPrefab, _towerPoolParent, _poolTowerSize, _poolTowerSize, OnGetTower);
            _bulletPool = PoolCreator.CreatePool(_bulletPrefab, _bulletsPoolParent, _poolBulletSize, _poolBulletSize, OnGetBullet);
            _effectShowerPool = PoolCreator.CreatePool(_effectShowerPrefab, _effectShowerPoolParent, _poolEffectShowerSize, _poolEffectShowerSize, OnGetEffectShower);
        }

        #region Control Pool Objects

        private void OnGetTower(Tower tower)
        {
            tower.gameObject.SetActive(true);
        }

        private void OnGetBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
        }

        private void OnGetEffectShower(EffectShower effectShower)
        {
            effectShower.gameObject.SetActive(true);
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
                Transform towerPlacePoint = _towerPlacePointsAll[i].GetTransform();
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

            if (_checkLoseGameCoroutine != null)
            {
                StopCoroutine(_checkLoseGameCoroutine);
            }
            _checkLoseGameCoroutine = StartCoroutine(CheckLoseGame());
        }

        private void SetTowersOnRoads(LevelData levelData)
        {
            foreach (var lineFill in levelData.LineFillData)
            {
                TowerData towerData = GetTowerData(lineFill);
                Tower tower = _towerPool.Get();

                Transform towersParent = towerData.Road.GetTransformByIndex(towerData.StartIndexPositionOnRoad);

                tower.SetDependencies(_linesController);
                tower.SetData(towerData);
                tower.transform.position = towersParent.position;
                tower.transform.localRotation = Quaternion.identity;
                tower.transform.SetParent(towersParent);
                towerData.Road.AddTower(tower);

                _towersActive.Add(tower);
            }
        }

        private IEnumerator CheckLoseGame()
        {
            bool isLose = false;

            int countIterations = 0;
            const int maxCountIterations = 4;

            while (isLose == false || countIterations < maxCountIterations)
            {
                isLose = true;

                yield return new WaitForEndOfFrame();

                foreach (var tower in _towerPlacePointsActive)
                {
                    if (tower.IsTowerICantAttackOrNull())
                    {
                        countIterations = 0;
                        isLose = false;
                        break;
                    }
                }

                yield return new WaitForSeconds(0.2f);

                countIterations++;
                CheckHasThreeTowersonFirePlacePoint(ref countIterations);
            }

            yield return new WaitForSeconds(1f);

            _gameProcessController.OnOutOfSpace();
        }

        private void CheckHasThreeTowersonFirePlacePoint(ref int countIterations)
        {
            List<TowerPlacePoint> towersOnPoints = _towerPlacePointsActive.Where(x => x.IsTowerICantAttackOrNull() == false).ToList();

            if (towersOnPoints.Count < 3)
            {
                Debug.Log("<color=yellow> return CheckHasThreeTowersonFirePlacePoint: less than 3 towers</color>");
                return;
            }

            Dictionary<CubeData.CubeColor, int> towersGroupedByColor = new Dictionary<CubeData.CubeColor, int>();

            foreach (var towerPlacePoint in towersOnPoints)
            {
                CubeData.CubeColor color = towerPlacePoint.GetTower().GetColor();
                if (towersGroupedByColor.ContainsKey(color))
                {
                    towersGroupedByColor[color]++;
                }
                else
                {
                    towersGroupedByColor.Add(color, 1);
                }
            }

            Dictionary<CubeData.CubeColor, int> filteredTowers = towersGroupedByColor
                .Where(pair => pair.Value >= 3)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            if (filteredTowers.Count == 0)
            {
                Debug.Log("<color=yellow> return CheckHasThreeTowersonFirePlacePoint: no filtered towers</color>");
                return;
            }
            else
            {
                if (filteredTowers.Count == 1)
                {
                    countIterations = 0;

                    if (_mergeTowersCoroutine == null)
                    {
                        _mergeTowersCoroutine = StartCoroutine(MergeTowers(filteredTowers.Keys.First()));
                    }
                }
                else
                {
                    Debug.LogError("Has three towers on fire place point");
                }
            }
        }

        private IEnumerator MergeTowers(CubeData.CubeColor color)
        {
            _gameProcessController.SetActivePlayerInput(false);

            List<TowerPlacePoint> towersPointToMerge = _towerPlacePointsActive.Where(x => x.GetTower() != null && x.GetTower().GetColor() == color).ToList();

            if (towersPointToMerge.Count < 3)
            {
                Debug.LogError("Less than 3 towers to merge on fire place point");
                yield break;
            }

            float targetOffsetY = 0.3f;

            AudioController.Instance.PlayAudio(AudioController.AudioType.StartMerge);

            foreach (var towerPlacePoint in towersPointToMerge)
            {
                Vector3 targetPosition = new Vector3(
                towerPlacePoint.GetTower().GetTransform().position.x,
                towerPlacePoint.GetTower().GetTransform().position.y + targetOffsetY,
                towerPlacePoint.GetTower().GetTransform().position.z);

                StartCoroutine(towerPlacePoint.GetTower().MoveToTargetCoroutine(null, true, targetPosition));
            }

            yield return new WaitForSeconds(0.4f);

            int countBulletsForAdd = 0;

            AudioController.Instance.PlayAudio(AudioController.AudioType.EndMerge);

            for (int i = 0; i < towersPointToMerge.Count; i++)
            {
                if (i == 1) { continue; }
                Vector3 targetPosition = new Vector3(
                towersPointToMerge[1].GetTower().GetTransform().position.x,
                towersPointToMerge[1].GetTower().GetTransform().position.y,
                towersPointToMerge[1].GetTower().GetTransform().position.z);

                countBulletsForAdd += towersPointToMerge[i].GetTower().GetCountBullets();

                StartCoroutine(towersPointToMerge[i].GetTower().MoveToTargetCoroutine(null, true, targetPosition));
            }

            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < towersPointToMerge.Count; i++)
            {
                if (i == 1)
                {
                    Vector3 targetPosition = new Vector3(
                        towersPointToMerge[1].GetTransform().position.x,
                        towersPointToMerge[1].GetTransform().position.y,
                        towersPointToMerge[1].GetTransform().position.z
                    );

                    StartCoroutine(towersPointToMerge[1].GetTower().MoveToTargetCoroutine(null, false, targetPosition));

                    towersPointToMerge[1].GetTower().AddBullets(countBulletsForAdd);
                    towersPointToMerge[1].GetTower().PlayMergeParticles();
                    continue;
                }

                towersPointToMerge[i].GetTower().ReleaseTower();
                towersPointToMerge[i].Release();
            }

            _gameProcessController.SetActivePlayerInput(true);
            _mergeTowersCoroutine = null;
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
            return _towerPlacePointsActive.FirstOrDefault(x => x.IsEmpty());
        }

        public Road GetRoadWithMinCountTowers()
        {
            return _roadsActive.OrderBy(x => x.GetCountTowersOnRoad()).First();
        }

        public Bullet GetBullet()
        {
            return _bulletPool.Get();
        }

        public EffectShower GetEffectShower()
        {
            return _effectShowerPool.Get();
        }

        public void ReleaseBullet(Bullet bullet)
        {
            _bulletPool.Release(bullet);
        }

        public void ChangePositionIndexForFierstTower(Road road)
        {
            Tower tower = _towersActive.FirstOrDefault(x => x.GetRoad() == road && x.IsLastPositionOnRoad());
            if (tower == null)
            {
                Debug.LogError("Fierst tower not found");
                return;
            }

            tower.SetLastPositionOnTheRoad();
            road.UpdateAllTowers();
        }

        public void ReleaseEffectShower(EffectShower effectShower)
        {
            _effectShowerPool.Release(effectShower);
        }

        public void ReleaseTower(Tower tower)
        {
            _towerPool.Release(tower);
            tower.gameObject.SetActive(false);
            tower.transform.SetParent(_towerPoolParent);
            tower.transform.position = Vector3.zero;
            tower.transform.localRotation = Quaternion.identity;

            _towersActive.Remove(tower);
        }

        public void Release()
        {
            _towerPlacePointsActive.Clear();
            _roadsActive.Clear();

            foreach (var place in _towerPlacePointsAll)
            {
                place.Release();
                place.GetTransform().gameObject.SetActive(false);
            }
            foreach (var road in _roadsAll)
            {
                road.Release();
                road.gameObject.SetActive(false);
            }

            List<Tower> towersToRelease = new List<Tower>(_towersActive);
            for (int i = 0; i < towersToRelease.Count; i++)
            {
                ReleaseTower(towersToRelease[i]);
            }
            towersToRelease.Clear();

            if (_checkLoseGameCoroutine != null)
            {
                StopCoroutine(_checkLoseGameCoroutine);
            }
        }
    }
}