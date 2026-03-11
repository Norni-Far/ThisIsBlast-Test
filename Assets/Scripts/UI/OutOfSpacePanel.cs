using UnityEngine;
using UnityEngine.UI;

public class OutOfSpacePanel : MonoBehaviour
{
    [SerializeField] private Button _buttonRestart;

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
        GameProcessController.Instance.RestartGame();
    }
}