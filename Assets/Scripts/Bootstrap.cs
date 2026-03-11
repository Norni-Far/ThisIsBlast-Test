using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameProcessController _gameProcessController;

    private void Awake()
    {

    }

    private void Start()
    {
        CreatePoolObjects();

        // Start Game
        _gameProcessController.StartGame();
    }

    private void CreatePoolObjects()
    {
        LinesController.Instance.CreatePoolObjects();
        TowerBase.Instance.CreatePoolObjects();
    }


}
