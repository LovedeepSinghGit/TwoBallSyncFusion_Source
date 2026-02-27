using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    private UserInputAction userInputAction;

    public event Action<Vector2> OnMove;
    private Vector2 moveInput;
    private bool jumpInput;

    void Awake()
    {
        userInputAction = new UserInputAction();
    }

    void OnEnable()
    {
        if (userInputAction != null)
        {
            userInputAction.Ball.Enable();

            userInputAction.Ball.Movement.performed += BallMovementCallBack;
            userInputAction.Ball.Movement.canceled += BallMovementCallBack;

            userInputAction.Ball.Jump.performed += BallJumpCallBack;
            userInputAction.Ball.Jump.canceled += BallJumpCallBack;
        }
    }

    void OnDisable()
    {
        if (userInputAction != null)
        {
            userInputAction.Ball.Movement.performed -= BallMovementCallBack;
            userInputAction.Ball.Movement.canceled -= BallMovementCallBack;

            userInputAction.Ball.Jump.performed -= BallJumpCallBack;
            userInputAction.Ball.Jump.canceled -= BallJumpCallBack;

            userInputAction.Ball.Disable();
        }
    }

    private void BallMovementCallBack(InputAction.CallbackContext callBack)
    {
        moveInput = callBack.ReadValue<Vector2>();
        OnMove?.Invoke(moveInput);
    }

    private void BallJumpCallBack(InputAction.CallbackContext callback)
    {
        jumpInput = callback.ReadValueAsButton();
    }

    public Vector2 GetCurrentInput()
    {
        return moveInput;
    }

    public bool Jump()
    {
        return jumpInput;
    }
}
