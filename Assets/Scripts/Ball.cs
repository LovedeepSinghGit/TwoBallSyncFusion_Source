using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [Networked] public NetworkBool isGameStarted { get; set; } = false;

    [SerializeField] private SpriteRenderer ballSR;
    [SerializeField] private Color myColor;
    [SerializeField] private Color opponentColor;

    [Space]
    [SerializeField] private float moveSpeed = 10f;
    private Vector2 currentInput;

    private List<IInputProvider> inputProviders = new();

    void Awake()
    {
        if (ballSR == null)
        {
            if (TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRebd)) ballSR = spriteRebd;
            else enabled = false; return;
        }
    }

    public override void Spawned()
    {
        if (Object.InputAuthority.PlayerId == 1) transform.position = new Vector2(-3, 0);
        else transform.position = new Vector2(3, 0);
    }

    public void InjectInputProvider(List<IInputProvider> providers)
    {
        inputProviders = providers;

        for(int i =0; i< inputProviders.Count; i++)
        {
            inputProviders[i].OnMove += HandleMovement;
        }
    }

    public override void Render()
    {
        if (Object.HasInputAuthority) ballSR.color = myColor;
        ballSR.enabled = isGameStarted;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority || !isGameStarted) return;

        if (currentInput != Vector2.zero)
        {
            Vector2 movement = moveSpeed * Runner.DeltaTime * currentInput;
            transform.Translate(movement);
        }
    }

    private void HandleMovement(Vector2 direction)
    {
        if (!Object.HasInputAuthority) return;
        currentInput = direction;
    }

    private void OnDisable()
    {
        for(int i =0; i< inputProviders.Count; i++)
        {
            inputProviders[i].OnMove -= HandleMovement;
        }
    }
}
