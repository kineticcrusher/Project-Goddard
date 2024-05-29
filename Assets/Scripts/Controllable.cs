using UnityEngine;

public class Controllable : MonoBehaviour, IControllable
{
    public float throttle;

    public void SetThrottle(float throttle)
    {
        this.throttle = throttle;
    }
}
