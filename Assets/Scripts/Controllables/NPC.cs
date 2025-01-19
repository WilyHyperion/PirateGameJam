using UnityEngine;

public class TestNPC : Controllable
{


    public override void ControlledUpdate()
    {
        base.ControlledUpdate();
    }
    public override void UncontrolledUpdate()
    {
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }
}
