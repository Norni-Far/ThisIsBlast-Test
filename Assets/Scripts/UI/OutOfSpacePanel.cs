namespace Blast.Core
{
    using UnityEngine;
    using UnityEngine.UI;

    public class OutOfSpacePanel : MonoBehaviour
    {
        [SerializeField] private Button _buttonRestart;

        private GameProcessController _gameProcessController;

        public void SetDependencies(GameProcessController gameProcessController)
        {
            _gameProcessController = gameProcessController;
        }

        private void OnEnable()
        {
            _buttonRestart.onClick.AddListener(OnButtonRestartClick);
        }

        private void OnDisable()
        {
            _buttonRestart.onClick.RemoveListener(OnButtonRestartClick);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnButtonRestartClick()
        {
            _gameProcessController.RestartGame();
        }
    }
}