using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputDelegator : MonoBehaviour, InputActions.IGameplayActions
{
    public static event Action<Vector2> OnMovementChanged;
    public static event Action OnGrabItemPressed;
    
    public static event Action<bool> OnLeftClick;
    public static event Action<bool> OnRightClick;

    private Vector2 _currentInput;
    
    //============================================================================================================//\

    private void OnEnable()
    {
        Inputs.input.Gameplay.Enable();
        Inputs.input.Gameplay.AddCallbacks(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    private void OnDisable()
    {
        Inputs.input.Gameplay.Disable();
        Inputs.input.Gameplay.RemoveCallbacks(null);
    }

    //============================================================================================================//

    public void OnHorizontalMovement(InputAction.CallbackContext context)
    {

        var x = context.ReadValue<float>();

        _currentInput.x = x;
        OnMovementChanged?.Invoke(_currentInput);

    }

    public void OnVerticalMovement(InputAction.CallbackContext context)
    {
        var y = context.ReadValue<float>();

        _currentInput.y = y;
        OnMovementChanged?.Invoke(_currentInput);
    }

    public void OnGrabItem(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton() == false)
            return;
        
        OnGrabItemPressed?.Invoke();
    }

    public void OnMouseLeftClick(InputAction.CallbackContext context)
    {
        var pressed = context.ReadValueAsButton();
        OnLeftClick?.Invoke(pressed);
    }

    public void OnMouseRightClick(InputAction.CallbackContext context)
    {
        var pressed = context.ReadValueAsButton();
        OnRightClick?.Invoke(pressed);
    }

    //============================================================================================================//
}
