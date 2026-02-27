using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputHandler : MonoBehaviour
{
    [SerializeField] private LobbySystem lobbySystem;

    public void JoinButtonPressed()
    {
        lobbySystem.JoinGame();
    }
}
