using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Color selectedColor;
    [SerializeField] Color unselectedColor;

    GameObject currentButton;

    int goHere = -4;
    int buttonStartPos = -5;

    private Dictionary<RectTransform, Coroutine> buttonAnimations = new Dictionary<RectTransform, Coroutine>();

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
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

        if (selectedButton == currentButton) return;

        if (currentButton != null) //go back (currentbutton is currently not selected)
        {
            SetButtonColor(currentButton, unselectedColor);
            Larp(buttonStartPos, currentButton);
        }

        currentButton = selectedButton;

        if (currentButton != null) //go forward (currentbutton is currently selected)
        {
            SetButtonColor(currentButton, selectedColor);
            Larp(goHere, currentButton);
        }
    }

    private void SetButtonColor(GameObject button, Color color)
    {
        TMP_Text text = button.GetComponentInChildren<TMP_Text>();

        if (text != null) text.color = color;
    }

    private void Larp(float targetX, GameObject button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();

        if (buttonAnimations.TryGetValue(rect, out Coroutine existingAnimation))
        {
            StopCoroutine(existingAnimation);
            buttonAnimations.Remove(rect);
        }

        if (Mathf.Abs(rect.position.x - targetX) <= 0.01f)
        {
            Vector3 position = rect.position;
            position.x = targetX;
            rect.position = position;
            return;
        }

        buttonAnimations[rect] = StartCoroutine(MoveButton(rect, targetX));
    }

    private IEnumerator MoveButton(RectTransform rect, float targetX)
    {
        while (rect != null && Mathf.Abs(rect.position.x - targetX) > 0.01f)
        {
            Vector3 position = rect.position;

            position.x = Mathf.Lerp(position.x, targetX, 10f * Time.unscaledDeltaTime);

            rect.position = position;

            yield return null;
        }

        if (rect != null)
        {
            Vector3 position = rect.position;
            position.x = targetX;
            rect.position = position;

            buttonAnimations.Remove(rect);
        }
    }
}