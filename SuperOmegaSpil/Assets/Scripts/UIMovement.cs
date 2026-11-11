using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMovement : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject eventUI;
    public static UIMovement instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    public void SetSelectedUIElement(GameObject uiElement)
    {
        eventSystem.SetSelectedGameObject(uiElement);
    }
    public void SetRandomSelectedUIElement()
    {
        for (int i = 0; i < eventUI.transform.childCount; i++)
        {
            if(eventUI.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                SetSelectedUIElement(eventUI.transform.GetChild(i).GetComponentInChildren<Button>().gameObject);
                break;
            }
        }
    }
}
