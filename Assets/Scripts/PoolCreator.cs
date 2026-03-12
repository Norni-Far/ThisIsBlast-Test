namespace Blast.Core
{
    using UnityEngine;
    using System;
    using UnityEngine.Pool;

    public static class PoolCreator
    {
        public static ObjectPool<T> CreatePool<T>(
            T prefab,
            Transform parent,
            int defaultCapacity,
            int maxSize,
            Action<T> onGet = null,
            Action<T> onRelease = null) where T : Component
        {
            return new ObjectPool<T>(
                createFunc: () => UnityEngine.Object.Instantiate(prefab, parent),
                actionOnGet: item =>
                {
                    onGet?.Invoke(item);
                },
                actionOnDestroy: item =>
                {
                    if (item != null && item.gameObject != null)
                        UnityEngine.Object.Destroy(item.gameObject);
                },
                defaultCapacity: defaultCapacity,
                maxSize: maxSize);
        }
    }
}