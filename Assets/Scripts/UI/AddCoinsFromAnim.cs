using UnityEngine;

public class AddCoinsFromAnim : MonoBehaviour
{
    [SerializeField] private UIControleer _uiControleer;

    public void OnAddCoins()
    {
        _uiControleer.UpdateCountCoinsFromAnim();
    }
}