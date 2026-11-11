using System;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MainMenu : MonoBehaviour
{
    GameObject currentButton;
    GameObject prevButton;

    [SerializeField] Color selectedColor;
    [SerializeField] Color unselectedColor;


    public void GoGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        Vector3 buttonGoHere = Vector3.one;
        Vector3 buttonStartPos;

        bool buttonIsCurrent = false;

        if (currentButton == null)
        {
            currentButton = EventSystem.current.currentSelectedGameObject;
        }

        if (currentButton != EventSystem.current.currentSelectedGameObject)
        {
            prevButton = currentButton;
            buttonIsCurrent = false;
        }
        else if (buttonIsCurrent == false)
        {
            currentButton = EventSystem.current.currentSelectedGameObject;
            buttonStartPos = currentButton.transform.position;
            buttonGoHere = currentButton.transform.position + new Vector3(30, 0, 0);
            buttonIsCurrent = true;
        }

        currentButton.GetComponentInChildren<TMP_Text>().color = selectedColor;
        prevButton.GetComponentInChildren<TMP_Text>().color = unselectedColor;

        while (currentButton.transform.position != buttonGoHere)
        {
            Vector3.Lerp(currentButton.transform.position, buttonGoHere, 5);
            if (Vector3.Distance(currentButton.transform.position, buttonGoHere) < 0.1f)
            {
                currentButton.transform.position = buttonGoHere;
            }
        }
    }
}
