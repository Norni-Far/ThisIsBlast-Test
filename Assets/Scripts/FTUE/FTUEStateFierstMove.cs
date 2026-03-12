
namespace Blast.Core
{
    using UnityEngine;

    public class FTUEStateFierstMove : IFTUEState
    {
        public const string TEXT_FTUE = "Tap to Select";

        private Transform _roundLightTransform;
        private Transform _armShowerTransform;

        private FTUEController _ftueController;

        public void Enter(FTUEController ftueController, FTUEController.FTUEData ftueData)
        {
            _ftueController = ftueController;

            Debug.Log("<color=purple>FTUEStateFierstMove: Enter</color>");

            _ftueController.SetTextFTUE(TEXT_FTUE);
            _roundLightTransform = _ftueController.GetRoundLightTransform();
            _armShowerTransform = _ftueController.GetArmShowerTransform();

            Transform towerPos = TowerBase.Instance.GetRoadWithMinCountTowers().GetTransformByIndex(0);

            Vector3 uiPosition = IFTUEState.WorldToUIPosition(towerPos, _roundLightTransform);
            _roundLightTransform.position = uiPosition;
            _armShowerTransform.position = uiPosition;

            _roundLightTransform.gameObject.SetActive(true);
            _armShowerTransform.gameObject.SetActive(true);
        }

        public void Exit()
        {
            _roundLightTransform.gameObject.SetActive(false);
            _armShowerTransform.gameObject.SetActive(false);
            _ftueController.SetTextFTUE("");
        }

        public void Update()
        {

        }
    }
}