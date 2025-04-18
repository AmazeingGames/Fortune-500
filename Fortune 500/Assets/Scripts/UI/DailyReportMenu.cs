using UnityEngine;

public class DailyReportMenu : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI employeesHiredCount;
    [SerializeField] TMPro.TextMeshProUGUI employeesRejectedCount;
    [SerializeField] TMPro.TextMeshProUGUI mistakesMadeCount;

    private void OnEnable()
    {
        UIManager.MenuChangeEventHandler += HandleMenuChange;
    }

    private void OnDisable()
    {
        UIManager.MenuChangeEventHandler -= HandleMenuChange;
    }

    void HandleMenuChange(object sender, MenuChangeEventArgs e)
    {
        switch (e.newMenuType)
        {
            case UIManager.MenuType.DailyReport:
                if (e.isEnablingMenu)
                {
                    employeesRejectedCount.text = DayManager.EmployeesRejectedToday.ToString();
                    mistakesMadeCount.text = DayManager.MistakesMadeToday.ToString();
                    employeesHiredCount.text = DayManager.EmployeesHiredToday.ToString();

                    Debug.Log("Updated daily report text");
                }
            break;
        }
    }

}
