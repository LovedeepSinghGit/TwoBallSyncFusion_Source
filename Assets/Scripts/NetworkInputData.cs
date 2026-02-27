using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[System.Serializable]
public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public bool jumpInput;
}
