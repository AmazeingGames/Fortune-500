using UnityEngine;

public class DailyReportMenu : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI employeesHiredCount;
    [SerializeField] TMPro.TextMeshProUGUI employeesRejectedCount;
    [SerializeField] TMPro.TextMeshProUGUI mistakesMadeCount;

    private void OnEnable()
    {
        CandidateHandler.ReviewedCandidateEventHandler += HandleReviewedCandidate;
    }

    private void OnDisable()
    {
        CandidateHandler.ReviewedCandidateEventHandler -= HandleReviewedCandidate;
    }

    void HandleReviewedCandidate(object sender, ReviewedCandidateEventArgs e)
    {
        employeesHiredCount.text = DayManager.Instance.EmployeesHiredToday.ToString();
        employeesRejectedCount.text = DayManager.Instance.EmployeesRejectedToday.ToString();
        mistakesMadeCount.text = DayManager.Instance.MistakesMadeToday.ToString();
    }

}
