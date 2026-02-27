using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [Networked] public NetworkBool isGameStarted { get; set; } = false;

    [SerializeField] private SpriteRenderer ballSR;
    [SerializeField] private Color myColor;
    [SerializeField] private Color opponentColor;

    [Space]
    [SerializeField] private Rigidbody2D ballRb;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float jumpForce = 5f;

    [Networked] public PlayerRef Winner { get; set; } = PlayerRef.None;

    private UIUpdates uiUpdates;

    public void InjectUI(UIUpdates ui)
    {
        uiUpdates = ui;
    }

    void Awake()
    {
        if (ballSR == null)
        {
            if (TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRebd)) ballSR = spriteRebd;
            else enabled = false; return;
        }

        if (ballRb == null)
        {
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) ballRb = rb;
            else enabled = false; return;
        }
    }

    public override void Render()
    {
        if (Object.HasInputAuthority)
        {
            ballSR.color = myColor;
        }
        else
        {
            ballSR.color = opponentColor;
        }

        // Find if any ball win
        if (Winner != PlayerRef.None)
        {
            bool amIWinner = (Winner == Runner.LocalPlayer);
            FindObjectOfType<UIUpdates>().ShowGameOverUI(amIWinner);

            // Disable movement after win
            isGameStarted = false;
        }

        // Hide the ball if game not started or ball won
        ballSR.enabled = isGameStarted;
    }

    public override void Spawned()
    {
        var lobby = FindObjectOfType<LobbySystem>();
        if (lobby != null)
        {
            InjectUI(lobby.GetUIUpdates());
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Stop physics if game not started or someone won
        if (!isGameStarted || IsMatchOver())
        {
            ballRb.velocity = Vector2.zero;
            return;
        }

        if (GetInput<NetworkInputData>(out var input))
        {
            if (Object.HasStateAuthority)
            {
                float targetSpeed = input.movementInput.x * maxSpeed;
                float speedDifference = targetSpeed - ballRb.velocity.x;

                float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

                float movement = accelRate * speedDifference;

                ballRb.AddForce(Vector2.right * movement);

                if (input.jumpInput)
                {
                    input.jumpInput = false;

                    // Prevents infinite double-jumping in the air
                    if (Mathf.Abs(ballRb.velocity.y) < 0.001f)
                    {
                        ballRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    }
                }

                if (Mathf.Abs(ballRb.velocity.x) > maxSpeed)
                {
                    ballRb.velocity = new Vector2(Mathf.Sign(ballRb.velocity.x) * maxSpeed, ballRb.velocity.y);
                }
            }
        }
    }

    public bool IsMatchOver()
    {
        foreach (var player in Runner.ActivePlayers)
        {
            var playerObj = Runner.GetPlayerObject(player);
            if (playerObj != null && playerObj.TryGetComponent<Ball>(out var b))
            {
                if (b.Winner != PlayerRef.None) return true;
            }
        }
        return false;
    }
}
