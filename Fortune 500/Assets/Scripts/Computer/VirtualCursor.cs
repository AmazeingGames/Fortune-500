using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualCursor : MonoBehaviour
{
    public enum MouseType { Virtual, Regular }

    [field: Header("Properties")]
    [field: SerializeField] public MouseType MovementType { get; private set; }

    [field: SerializeField] public EventSystem CursorEventSystem { get; private set; }
    [field: SerializeField] public ScreenClamper CursorClamper { get; private set; }
    [field: SerializeField] public Canvas ParentCanvas { get; private set; }

}
