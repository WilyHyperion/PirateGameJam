using UnityEngine;

public class TestNPC : Controllable
{


    public override void ControlledUpdate()
    {
        base.ControlledUpdate();
    }
    public override void UncontrolledUpdate()
    {
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5,5), Random.Range(-5, 5)));  
    }
}
