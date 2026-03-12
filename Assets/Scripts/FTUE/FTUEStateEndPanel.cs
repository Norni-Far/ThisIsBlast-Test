using UnityEngine;

public class FTUEStateEndPanel : IFTUEState
{

    private FTUEController _ftueController;
    private Transform _endPanelTransform;

    public void Enter(FTUEController ftueController, FTUEController.FTUEData ftueData)
    {
        GameProcessController.Instance.SetActivePlayerInput(false);

        _ftueController = ftueController;
        _endPanelTransform = ftueController.GetEndPanelTransform();

        _endPanelTransform.gameObject.SetActive(true);
    }

    public void Exit()
    {
        AudioController.Instance.PlayAudio(AudioController.AudioType.Click);
        GameProcessController.Instance.SetActivePlayerInput(true);
        _endPanelTransform.gameObject.SetActive(false);
    }

    public void Update()
    {

    }
}