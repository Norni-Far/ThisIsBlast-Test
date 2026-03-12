namespace Blast.Core
{
    using System;
    using System.Collections.Generic;
    using NaughtyAttributes;
    using UnityEngine;

    [Serializable]
    public class Road : MonoBehaviour
    {
        [SerializeField] private List<Transform> _towerPlacePoints;

        [Space]
        [SerializeField][ReadOnly] private List<Tower> _towersOnRoad;

        public Transform GetTransformByIndex(int index)
        {
            if (index < 0 || index >= _towerPlacePoints.Count)
            {
                Debug.LogError($"Index {index} is out of range for road {name}");
                return null;
            }

            return _towerPlacePoints[index];
        }

        public int GetCountTowersOnRoad()
        {
            return _towersOnRoad.Count;
        }

        public void AddTower(Tower tower)
        {
            _towersOnRoad.Add(tower);
        }

        public void RemoveTower(Tower tower)
        {
            _towersOnRoad.Remove(tower);
        }

        [Button]
        public void UpdateAllTowers()
        {
            foreach (var tower in _towersOnRoad)
            {
                tower.UpdatePosition();
            }
        }

        public void Release()
        {
            _towersOnRoad.Clear();
        }
    }
}