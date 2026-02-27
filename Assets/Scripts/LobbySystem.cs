using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbySystem : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private TMP_Text debugTxt;
    [SerializeField] private TMP_Text debugTxt2;
    private NetworkRunner runner;

    [SerializeField] private int playerCount = 2;

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;
    [SerializeField] private GameObject enemyPrefab;

    [Space]
    [SerializeField] private UIUpdates uiUpdates;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private CameraFollow cameraFollow;


    public void JoinGame()
    {
        debugTxt.text = "Join Game Pressed";
        JoinGameAsync();
    }

    async void JoinGameAsync()
    {
        debugTxt.text = "Join Game Async";

        if (uiUpdates != null)
        {
            uiUpdates.JoinButtonUIUpdate(false);
            uiUpdates.MatchMakingUIUpdate(true);
        }

        debugTxt.text = "Runner Instantiating";

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        var runnerSimulatePhysics2D = gameObject.AddComponent<RunnerSimulatePhysics2D>();
        runnerSimulatePhysics2D.ClientPhysicsSimulation = ClientPhysicsSimulation.SimulateAlways;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        debugTxt.text = "Lobby";
        var resultA = await runner.JoinSessionLobby(SessionLobby.Shared);

        if (!resultA.Ok)
        {
            debugTxt.text = "Lobby result not okey";
            uiUpdates.JoinButtonUIUpdate(true);
            uiUpdates.MatchMakingUIUpdate(false);
            return;
        }

        debugTxt.text = "Start game args";
        var resultB = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TwoBall_Sync_Room",
            PlayerCount = playerCount,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        debugTxt.text = $"Session Name {runner.SessionInfo.Name}";

        if (!resultB.Ok)
        {
            debugTxt.text = $"Session Name {runner.SessionInfo.Name} :: Game result not okey: {resultB.ShutdownReason}";
            uiUpdates.JoinButtonUIUpdate(true);
            uiUpdates.MatchMakingUIUpdate(false);
            return;
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        debugTxt.text = "JOin";

        if (runner.IsServer)
        {
            debugTxt.text = $"Player {player} joined. Spawning ball... {runner.SessionInfo.Region}";

            NetworkObject networkBallObject = runner.Spawn(ballPrefab, spawnPosition, Quaternion.identity, player);
        }

        if (runner.SessionInfo.PlayerCount == playerCount)
        {
            debugTxt.text = "All Player join";
            StartCoroutine(BeginMatchRoutine());
            uiUpdates.MatchMakingUIUpdate(false);
            SpawnLevelHazards();
        }
    }

    private IEnumerator BeginMatchRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        var allBalls = FindObjectsOfType<Ball>();
        foreach (var ball in allBalls)
        {
            ball.isGameStarted = true;
            if (ball.Object.HasInputAuthority) cameraFollow.InjectTarget(ball.transform);
        }

        uiUpdates.MatchMakingUIUpdate(false);
    }

    public UIUpdates GetUIUpdates()
    {
        return uiUpdates;
    }

    public void SpawnLevelHazards()
    {
        if (runner.IsServer) // Only the Host spawns global objects
        {
            runner.Spawn(enemyPrefab, new Vector3(30f, -1.8f, -10), Quaternion.identity);
            runner.Spawn(enemyPrefab, new Vector3(60f, -1.8f, -10), Quaternion.identity);
        }
    }


    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData();
        data.movementInput = inputSystem.GetCurrentInput();
        data.jumpInput = inputSystem.Jump();

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        debugTxt2.text = $"Shutdown reason: {shutdownReason}";
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        debugTxt.text = $"Connect Failed: {reason}";
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }
}
