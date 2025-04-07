using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UIButton;

public class Settings : MonoBehaviour
{
    public enum LinkedSetting { None, ToggleCalls, ToggleTips, ToggleMusic }


    public static Dictionary<LinkedSetting, bool> LinkedSettingToSetting;

    public static EventHandler<UpdateSettingsEventArgs> UpdateSettingsEventHandler;

    private void OnEnable()
    {
        UIButton.UIInteractEventHandler += HandleUIInteract;

    }
    private void OnDisable()
    {
        UIButton.UIInteractEventHandler -= HandleUIInteract;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        LinkedSettingToSetting = new Dictionary<LinkedSetting, bool>()
        {
            { LinkedSetting.None,       false },
            { LinkedSetting.ToggleCalls,  false },
            { LinkedSetting.ToggleTips,   false },
            { LinkedSetting.ToggleMusic,  false},
        };

        Assert.AreEqual(LinkedSettingToSetting.Count, Enum.GetNames(typeof(LinkedSetting)).Length, "Not all LinkedSettings keys are covered within dictionary.");
    }

    void HandleUIInteract(object sender, UIInteractEventArgs e)
    {
        if (e.myButtonType != ButtonType.Setting || e.myInteractionType != UIInteractionTypes.Click)
            return;

        OnUpdateSettings(e.myLinkedSetting);
    }

    void OnUpdateSettings(LinkedSetting mySettingToToggle)
    {
        LinkedSettingToSetting[mySettingToToggle] = !LinkedSettingToSetting[mySettingToToggle];
        UpdateSettingsEventHandler?.Invoke(this, new(mySettingToToggle));
    }
}

public class UpdateSettingsEventArgs : EventArgs
{
    public readonly Settings.LinkedSetting myLinkedSetting;
    public UpdateSettingsEventArgs(Settings.LinkedSetting myLinkedSetting)
    {
        this.myLinkedSetting = myLinkedSetting;
    }
}

