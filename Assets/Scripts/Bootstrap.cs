namespace Blast.Core
{
    using UnityEngine;

    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _textFPS;
        [SerializeField] private GameProcessController _gameProcessController;

        [SerializeField] private int _startLevelNumber = 1;

        private void Awake()
        {
            _gameProcessController.SetDependencies();
        }

        private void Start()
        {
            CreatePoolObjects();

            // Start Game
            _gameProcessController.StartGame(_startLevelNumber);
        }

        private void CreatePoolObjects()
        {
            _gameProcessController.CreatePoolObjects();
        }
    }
}