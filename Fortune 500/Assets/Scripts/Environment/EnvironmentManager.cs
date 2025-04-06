using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] TextMeshPro revenue_TMP;
    [SerializeField] TextMeshPro strikesLeft_TMP;
    [SerializeField] TextMeshPro day_TMP;
    void OnEnable()
    {
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
        GameFlowManager.PerformGameActionEventHandler += HandlePerformGameAction;
        ScoreKeeper.FluctuateIncomeEventHandler += HandleFluctuateIncome;
    }

    void OnDisable()
    {
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
        GameFlowManager.PerformGameActionEventHandler -= HandlePerformGameAction;
        ScoreKeeper.FluctuateIncomeEventHandler -= HandleFluctuateIncome;
    }

    void HandleFluctuateIncome(object sender, EventArgs e)
        => UpdateText();

    void HandlePerformGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameFlowManager.GameAction.PlayGame:
                UpdateText();
            break;

            case GameFlowManager.GameAction.None:
            case GameFlowManager.GameAction.EnterMainMenu:
            case GameFlowManager.GameAction.PauseGame:
            case GameFlowManager.GameAction.ResumeGame:
            case GameFlowManager.GameAction.LoseGame:
            break;
        }
    }

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayManager.DayState.StartWork:
                UpdateText();
            break;

            case DayManager.DayState.None:
            case DayManager.DayState.StartDay:
            case DayManager.DayState.EndWork:
            case DayManager.DayState.EndDay:
            break;
        }
    }

    void UpdateText()
    {
        revenue_TMP.text = $"Revenue: {ScoreKeeper.Revenue} bn";
        day_TMP.text = $"{DayManager.CurrentDay + 100}";

        strikesLeft_TMP.text = "";
        for (int i = 0; i < ScoreKeeper.StrikesLeft; i++)
            strikesLeft_TMP.text += "I";
    }
}
