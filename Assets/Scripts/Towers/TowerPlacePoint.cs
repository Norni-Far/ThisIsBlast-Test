using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class TowerPlacePoint : MonoBehaviour
{
    public Transform Transform;
    [ReadOnly] public Tower Tower;

    public void Release()
    {
        Tower = null;
    }
}