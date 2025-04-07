using NUnit.Framework.Internal.Execution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static GameFlowManager;
using static UIManager;
using static Menu;
using UnityEngine.Assertions;

/// <summary>
/// 
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [Header("Menus")]
    [SerializeField] Menu mainMenu;
    [SerializeField] Menu pauseMenu;
    [SerializeField] Menu settingsMenu;
    [SerializeField] Menu levelSelectMenu;
    [SerializeField] Menu dailyReportMenu;
    [SerializeField] Menu creditsScreen;
    [SerializeField] Menu gameEndScreen;

    [Header("Cameras")]
    [SerializeField] Camera userInterfaceCamera;
    [SerializeField] LayerMask uiOnlyCullingMask;
    [SerializeField] LayerMask allSeeingCullingMask;

    [field: Header("Button Properties")]
    [field: SerializeField] public float RegularScale { get; private set; } = 1.0f;
    [field: SerializeField] public float HoverScale { get; private set; } = 1.1f;

    [field: Range(0, 1)][field: SerializeField] public float RegularOpacity { get; private set; } = .66f;
    [field: Range(0, 1)][field: SerializeField] public float HoverOpacity { get; private set; } = 1;

    [field: Header("Button Lerp Properties")]
    [field: SerializeField] public float ButtonLerpSpeed { get; private set; } = 8;
    [field: SerializeField] public float UnderlineLerpSpeed { get; private set; } = 8;
    [field: SerializeField] public AnimationCurve ButtonLerpCurve { get; private set; }
    [field: SerializeField] public AnimationCurve UnderlineLerpCurve { get; private set; }

    public enum MenuType { None, Previous, MainMenu, Credits, Pause, Settings, LevelSelect, DailyReport, GameOverScreen, Empty }

    public static event EventHandler<MenuChangeEventArgs> MenuChangeEventHandler;

    MenuType nextInQueue;

    Menu currentMenu;
    Menu previousMenu;

    public static MenuType CurrentMenuType { get; private set; }
    MenuType previousMenuType;

    readonly Menu emptyMenu = new();
    readonly List<Menu> menuHistory = new();
    readonly List<Menu> nestedMenuHistory = new();
    int currentHistoryIndex = -1;

    Dictionary<MenuType, Menu> MenuTypeToMenu;
    Dictionary<Menu, MenuType> MenuToMenuType;
    readonly List<Menu> menus = new();

    private void Awake()
    {
        base.Awake();

        MenuTypeToMenu = new()
        {
            { MenuType.MainMenu,        mainMenu },
            { MenuType.Pause,           pauseMenu},
            { MenuType.GameOverScreen,  gameEndScreen},
            { MenuType.Settings,        settingsMenu},
            { MenuType.DailyReport,     dailyReportMenu},
            { MenuType.LevelSelect,     levelSelectMenu},
            { MenuType.Credits,         creditsScreen},
        };

        MenuToMenuType = MenuTypeToMenu.ToDictionary(x => x.Value, x => x.Key);
        menus.AddRange(MenuTypeToMenu.Values);
    }

    private void Start()
    {
        // Initializes each menu
        // Makes sure only the main menu is open on start
        foreach (Menu menu in menus)
        {
            if (!MenuToMenuType.TryGetValue(menu, out MenuType menuType))
                menuType = MenuType.None;
            menu.Init(menuType);

            if (menu.Canvas == null)
                throw new NullReferenceException("Menu canvas should not be null.");

            if (menu.MenuType != MenuType.MainMenu)
                menu.Canvas.gameObject.SetActive(false);
        }

        // Ignores following MenuTypes: 'Previous', 'None', 'Empty'
        if (MenuTypeToMenu.Count < Enum.GetNames(typeof(MenuType)).Length - 3)
            throw new Exception("Not all enums are counted for in the MenuTypeToMenu dictionary");

        // Debug.Log($"Game Manager's Last Game Action: {GameFlowManager.Instance.MyLastGameAction}");
        //UpdateMenusToGameAction(GameFlowManager.Instance.MyLastGameAction);
    }

    private void Update()
    {
        if (nextInQueue != MenuType.None && !ScreenTransitions.IsPlayingTransitionAnimation)
        {
            LoadMenu(nextInQueue);
            nextInQueue = MenuType.None;
        }
    }

    void OnEnable()
    {
        UIButton.UIInteractEventHandler += HandleUIButtonInteract;
        Menu.SetCanvasEventHandler += HandleSetCanvas;
        DayManager.DayStateChangeEventHandler += HandleDayStateChange;
        GameFlowManager.PerformGameActionEventHandler += HandleGameAction;
    }

    void OnDisable()
    {
        UIButton.UIInteractEventHandler -= HandleUIButtonInteract;
        GameFlowManager.PerformGameActionEventHandler -= HandleGameAction;
        Menu.SetCanvasEventHandler -= HandleSetCanvas;
        DayManager.DayStateChangeEventHandler -= HandleDayStateChange;
    }

    void HandleDayStateChange(object sender, DayStateChangeEventArgs e)
    {
        var menuToLoad = e.myDayState switch
        {
            DayManager.DayState.EndWork => MenuType.DailyReport,
            DayManager.DayState.StartDay => MenuType.Empty,
            _ => MenuType.None,
        };
        LoadMenu(menuToLoad);
    }

    void HandleGameAction(object sender, PerformGameActionEventArgs e)
        => UpdateMenusToGameAction(e.myGameAction);

    bool IsAMenuEnabled()
    {
        bool isAMenuEnabled = false;
        string activeCanvases = string.Empty;

        // We could save performance by skipping the loop on (ready == true)
        foreach (Menu menu in menus)
        {
            if (menu.Canvas.gameObject.activeInHierarchy)
            {
                activeCanvases += menu.Canvas.name;
                isAMenuEnabled = true;
            }
        }
        // Debug.Log($"The following canvases are currently active: {(activeCanvases == string.Empty ? "no active canvases" : activeCanvases[..^2])}");
        return isAMenuEnabled;
    }

    /// <summary> Sets UI and level camera active based on if there's currently an active canvas in the scene. </summary>
    /// <param name="setActive"> The SetActive() property the canvas was set to. </param>
    void HandleSetCanvas(object sender, Menu.SetCanvasEventArgs e)
    {
        bool isAMenuEnabled = IsAMenuEnabled();
        userInterfaceCamera.gameObject.SetActive(isAMenuEnabled);
        OnMenuChange(CurrentMenuType, previousMenuType, isEnablingMenu: e.setActive);
    }
    void OnMenuChange(MenuType newMenuType, MenuType previousMenuType, bool isEnablingMenu)
        => MenuChangeEventHandler?.Invoke(this, new(newMenuType, previousMenuType, isEnablingMenu));

    /// <summary> Loads a menu appropraite to the current game action. </summary>
    void UpdateMenusToGameAction(GameFlowManager.GameAction action)
    {
        MenuType menuToLoad = action switch
        {
            GameAction.EnterMainMenu => MenuType.MainMenu,
            GameAction.PauseGame => MenuType.Pause,
            GameAction.LoseGame => MenuType.GameOverScreen,
            _ => MenuType.Empty,
        };

        // Debug.Log($"Menu Manager: Handled Game Action {action} and loaded menu of type: {menuToLoad}");

        LoadMenu(menuToLoad);
    }

    /// <summary> Loads the appropraite menu when we click a ui button </summary>
    void HandleUIButtonInteract(object sender, UIButton.UIInteractEventArgs e)
    {
        if (e.myButtonType != UIButton.ButtonType.UI)
            return;

        if (e.myInteractionType != UIButton.UIInteractionTypes.Click)
            return;

        LoadMenu(e.myMenuToOpen);
    }

    void LoadMenu(MenuType myMenuType, bool addToHistory = true, bool addToQueue = true)
    {
        // In the future I would like the game to smoothly switch between screen transitions
        if (ScreenTransitions.IsPlayingTransitionAnimation)
        {
            // Debug.LogWarning("Can not change menus during screen transition.");
            if (addToQueue)
                nextInQueue = myMenuType;
            return;
        }

        Assert.IsNotNull(MenuTypeToMenu, "MenuTypeToMenu should not be null. Initialize on Awake.");

        if (MenuTypeToMenu.TryGetValue(myMenuType, out Menu menuToLoad))
            LoadMenu(menuToLoad, addToHistory);

        switch (myMenuType)
        {
            case MenuType.Previous:
                LoadPreviousMenu();
            break;

            case MenuType.Empty:
                foreach (Menu menuToUnload in menus)
                    menuToUnload.SetCanvas(false, needsToMoveOutOfFrame: true);
                nestedMenuHistory.Clear();

                CurrentMenuType = MenuType.None;
                break;

            case MenuType.None:
                break;

            default:
                Assert.IsNotNull(menuToLoad, $"Add {myMenuType} to MenuTypeToMenu dictionary or above switch expression. ");
            break;
        }
    }

    /// <summary> Loads a menu while unloading the previous menu </summary>
    /// <param name="menu"> Menu to load. </param>
    /// <param name="addToHistory"> If we are entering a nested menu.  </param>
    void LoadMenu(Menu menu, bool addToHistory = true)
    {
        // We also need to ensure that the menu we just loaded is at the end of the list
        bool transitioningToMenuUnderStack = false;
        if (nestedMenuHistory.Contains(menu))
        {
            transitioningToMenuUnderStack = true;
            nestedMenuHistory.RemoveAt(nestedMenuHistory.Count() - 1);
        }
        else
            nestedMenuHistory.Add(menu);

        if (addToHistory)
        {
            menuHistory.Add(menu);
            currentHistoryIndex++;
        }

        if (menu == currentMenu)
            Debug.LogWarning("Menu Manager: Should not be trying to load an already loaded menu");

        previousMenu = currentMenu;
        currentMenu = menu;

        if (currentMenu != null && MenuToMenuType.TryGetValue(currentMenu, out MenuType currentType))
            CurrentMenuType = currentType;
        if (previousMenu != null && MenuToMenuType.TryGetValue(previousMenu, out MenuType previousType))
            previousMenuType = previousType;

        previousMenu?.SetCanvas(false, false, !transitioningToMenuUnderStack);
        menu.SetCanvas(true, false, transitioningToMenuUnderStack);


        if (menu.CanSeePaper)
            userInterfaceCamera.cullingMask = allSeeingCullingMask;
        else
            userInterfaceCamera.cullingMask = uiOnlyCullingMask;
    }

    /// <summary> Loads the last loaded menu. </summary>
    void LoadPreviousMenu()
    {
        if (currentHistoryIndex == 0)
            return;

        if (currentHistoryIndex >= menuHistory.Count)
            throw new Exception("Menu history index exceeds the menu history list");

        LoadMenu(menuHistory[--currentHistoryIndex], false);
        menuHistory.RemoveAt(currentHistoryIndex + 1);
    }
}

