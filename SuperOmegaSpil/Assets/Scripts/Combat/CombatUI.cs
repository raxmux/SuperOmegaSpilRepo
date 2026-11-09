using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject selectionPanel;
    [SerializeField] GameObject movesPanel;
    [SerializeField] GameObject consumablesPanel;

    [Header("Selection panel")]
    [SerializeField] Button movesButton;
    [SerializeField] Button consumablesButton;

    [Header("Moves panel")]
    [SerializeField] Button[] moveButtons = new Button[4];

    [Header("Consumables panel")]
    [SerializeField] Button[] consumableButtons = new Button[2];

    [Tooltip("Text shown on a greyed out, empty slot.")]
    [SerializeField] string emptySlotText = "-";

    readonly List<Move> shownMoves = new List<Move>();
    GameObject currentPanel;

    void Awake()
    {
        movesButton.onClick.AddListener(ShowMoves);
        consumablesButton.onClick.AddListener(ShowConsumables);

        for (int i = 0; i < moveButtons.Length; i++)
        {
            int index = i; // copy, so each button remembers its own number
            moveButtons[i].onClick.AddListener(() => OnMoveClicked(index));
        }
    }

    // EnemyEvent turns this object on when a fight starts, so always open on the selection menu.
    void OnEnable()
    {
        if (CombatManager.Instance != null)
            CombatManager.Instance.CombatEnded += OnCombatEnded;

        ShowSelection();
    }

    void OnDisable()
    {
        if (CombatManager.Instance != null)
            CombatManager.Instance.CombatEnded -= OnCombatEnded;
    }

    public void ShowSelection()
    {
        ShowPanel(selectionPanel);
        Select(movesButton);
    }

    public void ShowMoves()
    {
        // The player's moves come from their equipment, max 4 shown.
        shownMoves.Clear();
        var moves = CombatManager.Instance.GetPlayerMoves();
        for (int i = 0; i < moves.Count && i < moveButtons.Length; i++)
            shownMoves.Add(moves[i]);

        for (int i = 0; i < moveButtons.Length; i++)
        {
            bool hasMove = i < shownMoves.Count;
            moveButtons[i].interactable = hasMove; // greys it out, and the stick skips it
            SetLabel(moveButtons[i], hasMove
                ? $"{shownMoves[i].moveName} ({shownMoves[i].timeCost:0.#}s)"
                : emptySlotText);
        }

        ShowPanel(movesPanel);
        Select(FirstInteractable(moveButtons));
    }

    public void ShowConsumables()
    {
        // David er fucking langsom! :c
        foreach (var button in consumableButtons)
        {
            button.interactable = false;
            SetLabel(button, emptySlotText);
        }

        ShowPanel(consumablesPanel);
        Select(FirstInteractable(consumableButtons));
    }
    public void Back(InputAction.CallbackContext context)
    {
        if (!context.performed || !isActiveAndEnabled) return;

        // Back on the selection menu does nothing.
        if (currentPanel != selectionPanel)
            ShowSelection();
    }

    void OnMoveClicked(int index)
    {
        if (index >= shownMoves.Count) return;

        CombatManager.Instance.UseMove(shownMoves[index]);

        // The move might have ended the fight and turned this UI off.
        if (isActiveAndEnabled)
            ShowSelection();
    }

    void OnCombatEnded(bool playerWon)
    {
        if (playerWon)
            EventManager.instance.EndEvent(); // back to the map
        else
            Debug.Log("Player died. No game over screen yet.");
    }

    void ShowPanel(GameObject panel)
    {
        selectionPanel.SetActive(panel == selectionPanel);
        movesPanel.SetActive(panel == movesPanel);
        consumablesPanel.SetActive(panel == consumablesPanel);
        currentPanel = panel;
    }

    static void Select(Button button)
    {
        if (EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(button != null ? button.gameObject : null);
    }

    static Button FirstInteractable(Button[] buttons)
    {
        foreach (var b in buttons)
            if (b != null && b.interactable) return b;
        return null;
    }

    static void SetLabel(Button button, string text)
    {
        var label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.text = text;
    }
}