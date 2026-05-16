using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameControls controls;
    private Dictionary<string, InputAction> actionMap;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        controls = new GameControls();
        controls.Enable();

        // Заполняем словарь действий из карты Gameplay
        actionMap = new Dictionary<string, InputAction>();
        foreach (var action in controls.Gameplay.Get())
        {
            actionMap[action.name] = action;
        }
    }

    public void SetActionEnabled(string actionName, bool enabled)
    {
        if (!actionMap.TryGetValue(actionName, out InputAction action))
        {
            Debug.LogError($"Action '{actionName}' не найдена в карте Gameplay!");
            return;
        }

        if (enabled)
            action.Enable();
        else
            action.Disable();
    }

    public InputAction GetAction(string actionName)
    {
        actionMap.TryGetValue(actionName, out InputAction action);
        return action;
    }
}