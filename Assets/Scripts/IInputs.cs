using System;
using UnityEngine;

public interface IInputProvider
{
    public event Action<Vector2> OnMove; 
}
