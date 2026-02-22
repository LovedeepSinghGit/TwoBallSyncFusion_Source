using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour, IInputProvider
{
    private UserInputAction userInputAction;

    public event Action<Vector2> OnMove;

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
        }
    }

    void OnDisable()
    {
        if (userInputAction != null)
        {
            userInputAction.Ball.Movement.performed -= BallMovementCallBack;
            userInputAction.Ball.Movement.canceled -= BallMovementCallBack;

            userInputAction.Ball.Disable();
        }
    }

    private void BallMovementCallBack(InputAction.CallbackContext callBack)
    {
        OnMove?.Invoke(callBack.ReadValue<Vector2>());
    }
}
