
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{


    [SerializeField] Color selectedColor;
    [SerializeField] Color unselectedColor;

    GameObject currentButton;
    GameObject prevButton;
    Vector3 goHere = Vector3.one;
    Vector3 buttonStartPos;
    bool buttonIsCurrent = false;

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
        if (currentButton == null)
        {
            currentButton = EventSystem.current.currentSelectedGameObject;
        }

        if (currentButton != EventSystem.current.currentSelectedGameObject)
        {
            prevButton = currentButton;
            buttonIsCurrent = false;
            StartCoroutine(Larp(buttonStartPos, prevButton));
        }
        
        if (buttonIsCurrent == false)
        {
            currentButton = EventSystem.current.currentSelectedGameObject;
            buttonStartPos = currentButton.GetComponent<RectTransform>().position;
            goHere = currentButton.GetComponent<RectTransform>().position + new Vector3(1.5f,0,0);
            currentButton.GetComponentInChildren<TMP_Text>().color = selectedColor;
            buttonIsCurrent = true;

            StartCoroutine(Larp(goHere, currentButton));
        }


        if (prevButton != null)
        {
            prevButton.GetComponentInChildren<TMP_Text>().color = unselectedColor;
        }
    }

    public IEnumerator Larp(Vector3 lerpGoHere, GameObject button)
    {
            while (button.GetComponent<RectTransform>().position != lerpGoHere)
            {
                button.GetComponent<RectTransform>().position = Vector3.Lerp(button.GetComponent<RectTransform>().position, lerpGoHere, 0.1f);
                yield return new WaitForSeconds(0.01f);

                if (Vector3.Distance(button.GetComponent<RectTransform>().position, lerpGoHere) < 0.01f)
                {
                    button.GetComponent<RectTransform>().position = lerpGoHere;
                }
            }
    }
}
