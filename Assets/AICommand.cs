using UnityEngine;

public abstract class AICommand
{
    public bool finished = false;
    public abstract void Update(float deltaTime);
}