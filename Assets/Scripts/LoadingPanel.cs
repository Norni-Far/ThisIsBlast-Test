using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private float _slowSpeed = 0.1f;
    [SerializeField] private float _fastSpeed = 2.0f;
    [SerializeField] private float _pauseThreshold = 0.85f;

    private Coroutine _loadingCoroutine;
    private bool _isSignalReceived;

    public void Show()
    {
        gameObject.SetActive(true);
        _isSignalReceived = false;

        if (_loadingCoroutine != null) StopCoroutine(_loadingCoroutine);
        _loadingCoroutine = StartCoroutine(LoadingRoutine());
    }

    public void Hide()
    {
        if (_loadingCoroutine != null) StopCoroutine(_loadingCoroutine);
        gameObject.SetActive(false);
    }

    public void SetLoading(float loading)
    {
        _loadingSlider.value = loading;
    }

    public void SignalFinish()
    {
        _isSignalReceived = true;
    }

    private IEnumerator LoadingRoutine()
    {
        _loadingSlider.value = 0;

        while (_loadingSlider.value < _pauseThreshold)
        {
            _loadingSlider.value += _slowSpeed * Time.deltaTime;
            yield return null;
        }

        while (!_isSignalReceived)
        {
            yield return null;
        }

        while (_loadingSlider.value < 1f)
        {
            _loadingSlider.value += _fastSpeed * Time.deltaTime;
            yield return null;
        }

        _loadingSlider.value = 1f;

        Hide();
    }
}