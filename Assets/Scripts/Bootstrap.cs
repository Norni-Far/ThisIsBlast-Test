using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _textFPS;
    [SerializeField] private GameProcessController _gameProcessController;

    [SerializeField] private int _startLevelNumber = 1;

    private void Awake()
    {

    }

    private void Start()
    {
        CreatePoolObjects();

        // Start Game
        _gameProcessController.StartGame(_startLevelNumber);
    }

    private void CreatePoolObjects()
    {
        LinesController.Instance.CreatePoolObjects();
        TowerBase.Instance.CreatePoolObjects();
    }

    private float _deltaTime = 0.0f;

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _textFPS.text = $"FPS: {Mathf.Ceil(fps)}";
    }


}
