namespace Blast.Core
{
    using HighlightPlus;
    using UnityEngine;

    public class TouchTriggerRaycaster : MonoBehaviour
    {
        public static TouchTriggerRaycaster Instance;

        [SerializeField] private Camera _camera;
        [SerializeField] private float _maxDistance = 100f;
        [SerializeField] private LayerMask _layerMask = ~0;

        public bool IsEnabled { get; set; } = true;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (IsEnabled == false)
            {
                return;
            }

            Vector2 screenPosition;
            bool hasInput = false;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                screenPosition = Input.GetTouch(0).position;
                hasInput = true;
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            else if (Input.GetMouseButtonDown(0))
            {
                screenPosition = Input.mousePosition;
                hasInput = true;
            }
#endif
            else
            {
                return;
            }

            if (!hasInput)
            {
                return;
            }

            Ray ray = _camera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _layerMask, QueryTriggerInteraction.Collide))
            {
                return;
            }

            if (hit.collider.TryGetComponent(out Tower tower))
            {
                if (tower.IsLastPositionOnRoad())
                {
                    tower.UpdatePosition();
                }
            }
        }
    }
}
