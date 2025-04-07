using System;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    public void CheckForClicks(Action onClick)
    {
        //Check for mouse click 
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("mouse click");
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 10000f, LayerMask.GetMask("Click")))
            {
                if (raycastHit.transform != null)
                {
                    Debug.Log("clicked something");
                    CurrentClickedGameObject(raycastHit.transform.gameObject, onClick);
                }
            }
        }
    }

    public void CurrentClickedGameObject(GameObject gameObject, Action onClick)
    {
        if (gameObject == this.gameObject)
        {
            Debug.Log("We were clicked!");
            onClick?.Invoke();
        }
    }
}
