using UnityEngine;

public class UIControleer : MonoBehaviour
{
    [SerializeField] private ResultsPanel _resultsPanel;
    [SerializeField] private OutOfSpacePanel _outOfSpacePanel;

    public void ShowResultsPanel(ResultsPanel.ResultData resultData)
    {
        _resultsPanel.SetResult(resultData);
    }

    public void ShowOutOfSpacePanel()
    {
        _outOfSpacePanel.Show();
    }

    public void HidePanels()
    {
        _resultsPanel.Hide();
        _outOfSpacePanel.Hide();
    }
}