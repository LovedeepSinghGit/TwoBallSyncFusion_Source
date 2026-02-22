using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;

public class LobbyHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private TMP_Text debugText;

    [Space]
    [SerializeField] private NetworkRunner networkRunnerObject;
    [SerializeField] private int maxPlayerCount;
    [SerializeField] private GameObject ballPrefab;

    [Space]
    [SerializeField] private UIUpdates uiUpdates;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private UIInputHandler uIInputHandler;
    [SerializeField] private CameraFollow cameraFollow;


    public void JoinRoom()
    {
        JoinRoomAsync();
    }

    private async void JoinRoomAsync()
    {
        // Update UI
        debugText.text = "UI Update";

        if (uiUpdates != null)
        {
            uiUpdates.MatchMakingUIUpdate(true);
            uiUpdates.JoinButtonUIUpdate(false);
        }

        debugText.text = "Runner Instantiating";
        NetworkRunner runner = Instantiate(networkRunnerObject);
        runner.AddCallbacks(this);

        debugText.text = "Making result";
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "FirstSession",
            PlayerCount = maxPlayerCount
        }
        );

        if (!result.Ok)
        {
            debugText.text = "Result not okey";
            uiUpdates.MatchMakingUIUpdate(false);
            uiUpdates.JoinButtonUIUpdate(true);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRefr)
    {
        debugText.text = $"Succesfully joined an room {runner.SessionInfo.Name} : {runner.SessionInfo.Region}";

        if (runner.LocalPlayer == playerRefr)
        {
            debugText.text = "Creating ball";
            NetworkObject newNetworkObject = runner.Spawn(ballPrefab, Vector2.zero, Quaternion.identity, playerRefr);
            if (newNetworkObject.TryGetComponent<Ball>(out Ball script))
            {
                List<IInputProvider> inputProviders = new();
                if(uIInputHandler.TryGetComponent<IInputProvider>(out IInputProvider provider)) inputProviders.Add(provider);

                if(inputSystem.TryGetComponent<IInputProvider>(out IInputProvider input)) inputProviders.Add(input);
                
                script.InjectInputProvider(inputProviders);
                debugText.text = "script attached";
            }
            else
            {
                debugText.text = "No script";
            }

            cameraFollow.InjectTarget(newNetworkObject.transform);
        }

        if (runner.SessionInfo.PlayerCount == maxPlayerCount)
        {
            foreach (var playerBall in runner.GetAllNetworkObjects())
            {
                if (playerBall.TryGetComponent<Ball>(out Ball b))
                {
                    if (b.Object.HasStateAuthority) b.isGameStarted = true;
                }
            }

            debugText.text = "Lets play";
            uiUpdates.TryAgainButtonUIUpdate(false);
            uiUpdates.BallControllsUIUpdate(true);
            if (uiUpdates != null) uiUpdates.MatchMakingUIUpdate(false);
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
