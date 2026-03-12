namespace Blast.Core
{
    using UnityEngine;

    public class FTUEStateEndPanel : IFTUEState
    {
        private FTUEController _ftueController;
        private Transform _endPanelTransform;

        public void Enter(FTUEController ftueController, FTUEController.FTUEData ftueData)
        {
            _ftueController = ftueController;

            _ftueController.GetGameProcessController().SetActivePlayerInput(false);

            _ftueController = ftueController;
            _endPanelTransform = ftueController.GetEndPanelTransform();

            _endPanelTransform.gameObject.SetActive(true);
        }

        public void Exit()
        {
            AudioController.Instance.PlayAudio(AudioController.AudioType.Click);
            _ftueController.GetGameProcessController().SetActivePlayerInput(true);
            _endPanelTransform.gameObject.SetActive(false);
        }

        public void Update()
        {

        }
    }
}