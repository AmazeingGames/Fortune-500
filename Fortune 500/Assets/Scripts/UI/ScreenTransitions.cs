using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.UIElements;

// Contains all code for loading screens and screen transitions
public class ScreenTransitions : MonoBehaviour
{
    [field: Header("Transition Data")]
    [SerializeField] TransitionData transitionInData;
    [SerializeField] TransitionData transitionOutData;
    [SerializeField] TransitionTypes transitionType;
    [SerializeField] float distance;

    public enum TransitionTypes { AlwaysMove, OnlyWhenNecessary, Stacked }
    public enum OthogonalDirection { Up, Down, Left, Right }

    public static event EventHandler<ScreenTransitionsEventArgs> ScreenTransitionEventHandler;

    public static bool IsPlayingTransitionAnimation { get; private set; } = false;

    OthogonalDirection lastDirection;
    Transform lastElements;
    
    bool lastIsReadying;
    bool neededToMoveOutOfFrame;
    bool wasLastNested;

    private void OnEnable()
        => Menu.SetCanvasEventHandler += HandleOnScreenTransition;

    private void OnDisable()
        => Menu.SetCanvasEventHandler -= HandleOnScreenTransition;

    void HandleOnScreenTransition(object sender, Menu.SetCanvasEventArgs e)
    {
        if (e.canvasElements == null)
            return;

        switch (e.mySetAction)
        {
            case Menu.CanvasAction.StartSet:
                StartTransition(e.canvasElements, e.setActive, e.slideDirection, e.needsToMoveOutOfFrame, e.wasNested);
                break;
            case Menu.CanvasAction.FinishSet:
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
# if DEBUG
        if (Input.GetKeyDown(KeyCode.Space) && lastElements != null)
            StartTransition(lastElements, lastIsReadying, lastDirection, neededToMoveOutOfFrame, wasLastNested);
#endif
    }

    OthogonalDirection OppositeDirection(OthogonalDirection direction)
    {
        return direction switch
        {
            OthogonalDirection.Up => OthogonalDirection.Down,
            OthogonalDirection.Down => OthogonalDirection.Up,
            OthogonalDirection.Left => OthogonalDirection.Right,
            OthogonalDirection.Right => OthogonalDirection.Left,
            _ => throw new NotImplementedException(),
        };
    }

    public Coroutine StartTransition(Transform elements, bool isReadying, OthogonalDirection slideInDirection, bool needsToMoveOutOfFrame, bool wasNested)
    {
        lastElements = elements;
        lastIsReadying = isReadying;
        lastDirection = slideInDirection;
        neededToMoveOutOfFrame = needsToMoveOutOfFrame;
        this.wasLastNested = wasNested;

        if (elements == null)
        {
            Debug.Log("Could not transition due to null elements");
            return null;
        }
        var position = elements.localPosition;

        var transitionData = isReadying ? transitionInData : transitionOutData;

        var slideDirection = slideInDirection;
        if (isReadying)
        {
            elements.localPosition = slideInDirection switch
            {
                OthogonalDirection.Up => new(position.x, -distance),
                OthogonalDirection.Down => new(position.x, distance),
                OthogonalDirection.Left => new(distance, position.y),
                OthogonalDirection.Right => new(-distance, position.y),
                _ => throw new NotImplementedException($"Othogonal Direction {slideInDirection} not implemented")
            };
        }
        else
            slideDirection = OppositeDirection(slideDirection);

        return StartCoroutine(SlideElements(elements, transitionData, isReadying, slideDirection, needsToMoveOutOfFrame, wasNested));
    }

    // I can either move the game elements or I can move the game camera
    IEnumerator SlideElements(Transform elements, TransitionData transitionData, bool movingInFrame, OthogonalDirection slideDirection, bool mustMoveOutOfFrame, bool wasNested)
    {
        var position = elements.localPosition;
        Vector2 startingPosition = elements.transform.localPosition;
        Vector2 goalPosition;

        if (movingInFrame)
            goalPosition = Vector2.zero;
        else    
            goalPosition = slideDirection switch
            {
                OthogonalDirection.Up => new(position.x, position.y + distance),
                OthogonalDirection.Down => new(position.x, position.y - distance),
                OthogonalDirection.Left => new(position.x - distance, position.y),
                OthogonalDirection.Right => new(position.x + distance, position.y),
                _ => throw new NotImplementedException($"Othogonal Direction {slideDirection} not implemented")
            };

        float current = 0;

        if (movingInFrame && wasNested && transitionType == TransitionTypes.Stacked)
        {
            Debug.Log("Set current to 1");
            current = 1;
        }

        while (current < 1)
        {
            IsPlayingTransitionAnimation = true;
            current = Mathf.MoveTowards(current, 1, transitionData.Speed * Time.deltaTime);

            switch (transitionType)
            {
                case TransitionTypes.AlwaysMove:
                    LerpElements();
                break;

                case TransitionTypes.Stacked:
                    if (movingInFrame || mustMoveOutOfFrame || (!movingInFrame && !wasNested))
                        LerpElements();
                break;

                case TransitionTypes.OnlyWhenNecessary:
                    if (movingInFrame || mustMoveOutOfFrame)
                        LerpElements();
                break;
            }

            void LerpElements()
                => elements.localPosition = Vector3.Lerp(startingPosition, goalPosition, transitionData.Curve.Evaluate(current));

            yield return null;
        }
        elements.localPosition = goalPosition;
        IsPlayingTransitionAnimation = false;
    }
}

[Serializable]
class TransitionData
{
    [field: SerializeField] public AnimationCurve Curve { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
}

public class ScreenTransitionsEventArgs : EventArgs { }

