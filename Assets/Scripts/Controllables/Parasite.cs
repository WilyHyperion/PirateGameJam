using UnityEngine;

public class Parasite : Controllable
{
    private new void Start()
    {
        base.Start();
        this.Clearance = -1;
    }
}
