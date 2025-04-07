using static UIManager;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
class Menu
{
    [field: SerializeField] public Canvas Canvas { get; private set; } = new();
    [field: SerializeField] public List<GameObject> EnableOnReady { get; private set; } = new();
    [field: SerializeField] public List<GameObject> DisableOnReady { get; private set; } = new();
    [field: SerializeField] public ScreenTransitions.OthogonalDirection SlideDirection { get; private set; }
    [field: SerializeField] public bool CanSeePaper { get; private set; }
    public Transform CanvasElements { get; private set; }
    public MenuType MenuType { get; private set; }

    bool isReady = false;

    public static EventHandler<SetCanvasEventArgs> SetCanvasEventHandler;

    public enum CanvasAction { StartSet, FinishSet }

    public void SetCanvas(bool setActive, bool needsToMoveOutOfFrame = false, bool wasNested = false)
    {
        if (setActive == isReady)
        {
            // Debug.Log($"Trying to {(setActive ? "enable" : "disable")} {MenuType} screen, when canvas is already {(isReady ? "enabled" : "disabled")}");
            return;
        }

        isReady = setActive;

        // Debug.Log($"Starting transition to {(setActive ? "enable" : "disable")} {MenuType} canvas. | Slide direction : {SlideDirection} | needsToMoveOutOfFrame : {needsToMoveOutOfFrame} | wasNested : {wasNested}");

        UIManager.Instance.StartCoroutine(SetObjectsAndCanvas(setActive, needsToMoveOutOfFrame, wasNested));

        if (setActive)
        {
            string disabledMenus = string.Empty;
            foreach (GameObject obj in DisableOnReady)
            {
                if (obj.TryGetComponent(out Menu menu))
                {
                    disabledMenus += menu.Canvas.name + ", ";
                    menu.SetCanvas(false);
                }
                else
                    obj.SetActive(false);

            }
            // Debug.Log($"Disabled the following menus on ready: {(disabledMenus == string.Empty ? "none" : disabledMenus[..^2])}");
        }
    }

    System.Collections.IEnumerator SetObjectsAndCanvas(bool ready, bool needsToMoveOutOfFrame, bool wasNested)
    {
        OnCanvasAction(ready, needsToMoveOutOfFrame, wasNested, CanvasAction.StartSet);

        // Enables the canvas before it starts moving into frame
        if (ready)
        {
            OnCanvasAction(ready, needsToMoveOutOfFrame, wasNested, CanvasAction.FinishSet);
            foreach (GameObject obj in EnableOnReady)
                obj.SetActive(ready);
        }

        while (ScreenTransitions.IsPlayingTransitionAnimation)
            yield return null;

        // Waits until the canvas moves out of frame before disabling it
        if (!ready)
        {
            OnCanvasAction(ready, needsToMoveOutOfFrame, wasNested, CanvasAction.FinishSet);
            foreach (GameObject obj in EnableOnReady)
                obj.SetActive(ready);
        }
    }

    void OnCanvasAction(bool setActive, bool moveOutOfFrame, bool wasNested, CanvasAction mySetCanvasAction)
    {
        if (Canvas == null)
            return;

        if (mySetCanvasAction == CanvasAction.FinishSet)
            Canvas.gameObject.SetActive(setActive);

        // Debug.Log($"Setting {Canvas.name} canvas {(setActive ? "active" : "disabled")}");
        SetCanvasEventHandler?.Invoke(this, new(this, setActive, CanvasElements, SlideDirection, moveOutOfFrame, wasNested, mySetCanvasAction));

    }

    public void Init(MenuType menuType)
    {
        if (Canvas == null)
        {
            Debug.Log("Could not initialize null canvas");
            return;
        }

        for (int i = 0; i < Canvas.transform.childCount; i++)
        {
            var child = Canvas.transform.GetChild(i);

            if (child.name == "Elements")
                CanvasElements = child;
        }

        this.MenuType = menuType;
    }

    public class SetCanvasEventArgs : EventArgs
    {
        public readonly Menu menu;
        public readonly bool setActive;

        public readonly Transform canvasElements;
        public readonly ScreenTransitions.OthogonalDirection slideDirection;
        public readonly bool needsToMoveOutOfFrame;
        public readonly bool wasNested;
        public readonly CanvasAction mySetAction;

        public SetCanvasEventArgs(Menu menu, bool setActive, Transform canvasElements, ScreenTransitions.OthogonalDirection slideDirection, bool needsToMoveOutOfFrame, bool wasNested, CanvasAction mySetAction)
        {
            this.menu = menu;
            this.setActive = setActive;
            this.canvasElements = canvasElements;
            this.slideDirection = slideDirection;
            this.needsToMoveOutOfFrame = needsToMoveOutOfFrame;
            this.wasNested = wasNested;
            this.mySetAction = mySetAction;
        }
    }
}

public class MenuChangeEventArgs
{
    public readonly MenuType newMenuType;
    public readonly MenuType previousMenuType;
    public readonly bool isEnablingMenu;
    public MenuChangeEventArgs(MenuType newMenuType, MenuType previousMenuType, bool isEnablingMenu)
    {
        this.newMenuType = newMenuType;
        this.previousMenuType = previousMenuType;
        this.isEnablingMenu = isEnablingMenu;
    }
}