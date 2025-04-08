using UnityEngine;
using UnityEngine.UI;

public class ResumeButton : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMPro.TextMeshProUGUI text_TMP;
    [SerializeField] Button button;

    bool shouldBeActiveStation;

    private void OnEnable()
    {
        GameFlowManager.PerformGameActionEventHandler += HandlePerformGameAction;
        UIManager.MenuChangeEventHandler += HandleMenuChange;
        FocusStation.InterfaceConnectedEventHandler += HandleInterfaceConnected;
    }

    private void OnDisable()
    {
        GameFlowManager.PerformGameActionEventHandler -= HandlePerformGameAction;
        UIManager.MenuChangeEventHandler -= HandleMenuChange;
        FocusStation.InterfaceConnectedEventHandler -= HandleInterfaceConnected;
    }

    private void Update()
    {
        if (GameFlowManager.Instance.MyCurrentState == GameFlowManager.GameState.Running && shouldBeActiveStation && !isMenuOpen && CandidateHandler.Instance.currentCandidateData != null)
        {
            image.enabled = true;
            text_TMP.enabled = true;
            button.enabled = true;
        }
        else
        {
            image.enabled = false;
            text_TMP.enabled = false;
            button.enabled = false;
        }
    }

    bool isMenuOpen = true;
    void HandleMenuChange(object sender, MenuChangeEventArgs e)
    {
        switch (e.newMenuType)
        {
            case UIManager.MenuType.None:
                break;
            case UIManager.MenuType.Previous:
            case UIManager.MenuType.MainMenu:
            case UIManager.MenuType.Credits:
            case UIManager.MenuType.Pause:
            case UIManager.MenuType.Settings:
            case UIManager.MenuType.LevelSelect:
            case UIManager.MenuType.DailyReport:
            case UIManager.MenuType.GameOverScreen:
                isMenuOpen = e.isEnablingMenu;
            break;

            case UIManager.MenuType.Empty:
                isMenuOpen = true;
                break;
        }

    }

    void HandlePerformGameAction(object sender, PerformGameActionEventArgs e)
    {

    }

    void HandleInterfaceConnected(object sender, InterfaceConnectedEventArgs e)
    {
        switch (e.myStation)
        {
            case PlayerFocus.Station.Nothing:
                break;
            case PlayerFocus.Station.Computer:
                break;
            case PlayerFocus.Station.Resume:
                if (e.myInteractionType == FocusStation.InteractionType.Connect)
                    shouldBeActiveStation = true;
                else if (e.myInteractionType == FocusStation.InteractionType.Disconnect)
                    shouldBeActiveStation = false;
                break;
            case PlayerFocus.Station.Slots:
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}
