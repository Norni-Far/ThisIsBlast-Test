using UnityEngine;
using static FTUEController;

public interface IFTUEState
{
    void Enter(FTUEController ftueController, FTUEData ftueData);
    void Update();
    void Exit();

    protected static Vector3 WorldToUIPosition(Transform worldTarget, Transform uiTarget, Camera worldCamera = null)
    {
        if (worldTarget == null || uiTarget == null)
        {
            return Vector3.zero;
        }

        RectTransform uiRectTransform = uiTarget as RectTransform;
        if (uiRectTransform == null)
        {
            return worldTarget.position;
        }

        Canvas canvas = uiRectTransform.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return worldTarget.position;
        }

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        worldCamera ??= Camera.main != null ? Camera.main : Camera.current;
        if (worldCamera == null)
        {
            return worldTarget.position;
        }

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, worldTarget.position);
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
                uiRectTransform,
                screenPoint,
                uiCamera,
                out Vector3 worldPointOnUiRect))
        {
            return worldTarget.position;
        }

        return worldPointOnUiRect;
    }
}