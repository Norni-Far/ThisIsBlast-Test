using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FTUEController : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _textFTUE;
    [SerializeField] private Transform _roundLightTransform;
    [SerializeField] private Transform _rectangleTransform;
    [SerializeField] private Transform _armShowerTransform;
    [SerializeField] private Transform _mergePanelTransform;
    [SerializeField] private Transform _endPanelTransform;
    [SerializeField] private List<Button> _skipButtons;

    [Space]
    [SerializeField] private List<FTUEData> _ftueData;

    private IFTUEState _currentState;

    private void OnEnable()
    {
        foreach (var skipButton in _skipButtons)
        {
            skipButton.onClick.AddListener(OnSkipButtonClick);
        }
    }

    private void OnDisable()
    {
        foreach (var skipButton in _skipButtons)
        {
            skipButton.onClick.RemoveListener(OnSkipButtonClick);
        }
    }

    private readonly Dictionary<int, IFTUEState> _states = new Dictionary<int, IFTUEState>()
    {
        {1, new FTUEStateFierstMove()},
        {2, new FTUEStateMergeShow()},
        {3, new FTUEStateEndPanel()},
    };

    public void CheckIfNeedToShowFTUE(int levelIndex)
    {
        if (_ftueData == null || _ftueData.Count == 0)
        {
            Debug.LogError("FTUE data not found");
            return;
        }

        FTUEData ftueData = _ftueData.Find(x => x.LevelIndex == levelIndex);
        if (ftueData == null)
        {
            return;
        }

        ftueData.State = GetStateByNumber(ftueData.IndexState);

        SetState(ftueData.State, ftueData);
    }

    public void SetState(IFTUEState state, FTUEData ftueData)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState?.Enter(this, ftueData);
    }

    public void UpdateState()
    {
        _currentState?.Update();
    }

    public void ExitState()
    {
        _currentState?.Exit();
        _currentState = null;
    }

    private void OnSkipButtonClick()
    {
        ExitState();
    }

    public IFTUEState GetStateByNumber(int indexState)
    {
        if (_states.TryGetValue(indexState, out var state))
        {
            return state;
        }

        Debug.LogError($"State with index {indexState} not found");
        return null;
    }

    public void SetTextFTUE(string text)
    {
        _textFTUE.text = text;

        if (string.IsNullOrEmpty(text))
        {
            _textFTUE.gameObject.SetActive(false);
        }
        else
        {
            _textFTUE.gameObject.SetActive(true);
        }
    }

    public Transform GetRoundLightTransform()
    {
        return _roundLightTransform;
    }

    public Transform GetRectangleTransform()
    {
        return _rectangleTransform;
    }

    public Transform GetMergePanelTransform()
    {
        return _mergePanelTransform;
    }

    public Transform GetArmShowerTransform()
    {
        return _armShowerTransform;
    }

    public Transform GetEndPanelTransform()
    {
        return _endPanelTransform;
    }

    [Serializable]
    public class FTUEData
    {
        public int LevelIndex;
        public int IndexState;
        public IFTUEState State;
    }
}