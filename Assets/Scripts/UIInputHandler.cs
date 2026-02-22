using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIInputHandler : MonoBehaviour, IInputProvider
{
    [SerializeField] private LobbyHandler lobbyHandler;
    public event Action<Vector2> OnMove;

    public void OnJoinButtonPressed()
    {
        if(lobbyHandler != null) lobbyHandler.JoinRoom();
    }

    public void TryAgainButtonPressed()
    {
        SceneManager.LoadScene(0);
    }

    public void LeftButtonDown()
    {
        OnMove?.Invoke(Vector2.left);
    }

    public void LeftRigthButtonUp()
    {
        OnMove?.Invoke(Vector2.zero);
    }

    public void RightButtonDown()
    {
        OnMove?.Invoke(Vector2.right);
    }
}
