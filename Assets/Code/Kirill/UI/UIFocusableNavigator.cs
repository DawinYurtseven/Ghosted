using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// This code is deprecated
public class UIFocusableNavigator : MonoBehaviour
{
    public UIDocument uiDocument;
    public InputActionAsset inputActions;
    public List<string> focusableElemntsToChooseInOrder;

    private List<VisualElement> focusableElements = new();
    private int currentIndex = 0;

    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction adjustAction;

    void Start()
    {
        // Get actions
        //navigateAction = inputActions.FindAction("Navigate");
        //submitAction = inputActions.FindAction("Submit");
        //adjustAction = inputActions.FindAction("AdjustValue");

        //navigateAction.performed += OnNavigate;
        //submitAction.performed += OnSubmit;
        //adjustAction.performed += OnAdjustValue;

        //navigateAction.Enable();
        //submitAction.Enable();
        //adjustAction.Enable();

        // Collect all focusable elements
        focusableElements.Clear();
        var root = uiDocument.rootVisualElement;

        foreach (string name in focusableElemntsToChooseInOrder)
        {
            var el = root.Q<VisualElement>(name);
            if (el.focusable)
                focusableElements.Add(el);
        }

        /*foreach (var el in root.Query<VisualElement>().ToList())
        {
            if (el.focusable)
                focusableElements.Add(el);
        }*/

        if (focusableElements.Count > 0)
            focusableElements[currentIndex].Focus();
    }

    void OnNavigate(InputAction.CallbackContext ctx)
    {
        Debug.Log("I am navigating in the menu");
        Vector2 dir = ctx.ReadValue<Vector2>();

        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y > 0)
                MoveFocus(-1);
            else if (dir.y < 0)
                MoveFocus(1);
        }
    }

    void MoveFocus(int direction)
    {
        Debug.Log("My index befor: " + currentIndex);
        currentIndex = (currentIndex + direction + focusableElements.Count) % focusableElements.Count;
        Debug.Log("My index after: " + currentIndex);
        focusableElements[currentIndex].Focus();
    }

    void OnSubmit(InputAction.CallbackContext ctx)
    {
        var current = focusableElements[currentIndex];
        Debug.Log("I submit something: " + current.name);

        if (current is Button btn)
            Debug.Log("I clicked"); //btn.clickable?.Invoke();
        else if (current is Toggle toggle)
            toggle.value = !toggle.value;
        else if (current is DropdownField dropdown)
            CycleDropdown(dropdown, 1);
    }

    void OnAdjustValue(InputAction.CallbackContext ctx)
    {
        var current = focusableElements[currentIndex];
        float value = ctx.ReadValue<Vector2>().x;

        Debug.Log("I am adjusting focusableElement: " + current);
        Debug.Log("It has an id: " + currentIndex);
        Debug.Log("I am adjusting value: " + value);

        if (current is Slider slider)
        {
            Debug.Log("I am adjusting slider: " + slider.name);
            float delta = (slider.highValue - slider.lowValue) * 0.05f;
            slider.value += Mathf.Sign(value) * delta;
            Debug.Log("Slider was effected");
        }
        else if (current is DropdownField dropdown)
        {
            CycleDropdown(dropdown, (int)Mathf.Sign(value));
            Debug.Log("Dropdown was effected");
        }
    }

    void CycleDropdown(DropdownField dropdown, int direction)
    {
        int index = dropdown.choices.IndexOf(dropdown.value);
        index = (index + direction + dropdown.choices.Count) % dropdown.choices.Count;
        dropdown.value = dropdown.choices[index];
    }

    void OnDisable()
    {
        navigateAction.performed -= OnNavigate;
        submitAction.performed -= OnSubmit;
        adjustAction.performed -= OnAdjustValue;

        navigateAction.Disable();
        submitAction.Disable();
        adjustAction.Disable();
    }
}
