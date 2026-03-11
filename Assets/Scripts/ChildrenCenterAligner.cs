using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ChildrenCenterAligner : MonoBehaviour
{
    public enum LayoutDirection
    {
        Horizontal,
        Vertical
    }

    [SerializeField] private LayoutDirection _direction = LayoutDirection.Horizontal;
    [SerializeField] private float _spacing = 1f;
    [SerializeField] private bool _autoUpdateInEditor = true;

    private void Start()
    {
        AlignChildren();
    }

    private void OnValidate()
    {
        if (_autoUpdateInEditor)
        {
            AlignChildren();
        }
    }

    [Button("Align Children")]
    public void AlignChildren()
    {
        List<Transform> activeChildren = new List<Transform>();
        int allChildrenCount = transform.childCount;
        for (int i = 0; i < allChildrenCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                activeChildren.Add(child);
            }
        }

        int activeCount = activeChildren.Count;
        if (activeCount == 0)
        {
            return;
        }

        float totalLength = (activeCount - 1) * _spacing;
        float startOffset = -totalLength * 0.5f;

        for (int i = 0; i < activeCount; i++)
        {
            Transform child = activeChildren[i];
            Vector3 localPosition = child.localPosition;
            float positionOnAxis = startOffset + i * _spacing;

            if (_direction == LayoutDirection.Horizontal)
            {
                localPosition.x = positionOnAxis;
            }
            else
            {
                localPosition.y = positionOnAxis;
            }

            child.localPosition = localPosition;
        }
    }
}
