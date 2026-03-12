namespace Blast.Core
{
    using UnityEngine;

    public class FTUEStateMergeShow : IFTUEState
    {
        private FTUEController _ftueController;
        private Transform _rectangleTransform;
        private Transform _armShowerTransform;

        public void Enter(FTUEController ftueController, FTUEController.FTUEData ftueData)
        {
            SetRightLineTowers();

            _ftueController = ftueController;

            Debug.Log("<color=purple>FTUEStateFierstMove: Enter</color>");

            _ftueController.SetTextFTUE("");
            _rectangleTransform = _ftueController.GetRectangleTransform();
            _armShowerTransform = _ftueController.GetArmShowerTransform();

            Transform towerPos = TowerBase.Instance.GetRoadWithMinCountTowers().GetTransformByIndex(0);

            Vector3 uiPosition = IFTUEState.WorldToUIPosition(towerPos, _rectangleTransform);
            _rectangleTransform.position = uiPosition;
            _armShowerTransform.position = uiPosition;

            _rectangleTransform.gameObject.SetActive(true);
            _armShowerTransform.gameObject.SetActive(true);
            _ftueController.GetMergePanelTransform().gameObject.SetActive(true);
        }

        private void SetRightLineTowers()
        {
            Road road = TowerBase.Instance.GetRoadWithMinCountTowers();
            TowerBase.Instance.ChangePositionIndexForFierstTower(road);
        }

        public void Exit()
        {
            _rectangleTransform.gameObject.SetActive(false);
            _armShowerTransform.gameObject.SetActive(false);
            _ftueController.GetMergePanelTransform().gameObject.SetActive(false);
            _ftueController.SetTextFTUE("");
        }

        public void Update()
        {

        }
    }
}