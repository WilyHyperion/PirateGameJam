
using UnityEngine;

using UnityEngine.InputSystem;
/// <summary>
/// represents something that the player can take control of
/// </summary>
public abstract class Controllable : MonoBehaviour
{
    public  void RandomizeData()
    {
        name = "John Test";//todo name generation
        age = Random.Range(18, 100);
        height = Random.Range(1.5f, 2.5f);
        weight = Random.Range(50, 100);
        baseSpeed = Random.Range(5, 6);
        maxSpeed = baseSpeed;
    }
    public bool isControlled = false;
    public int sanity = 100;
    public float friction = 0.9f;
    public string name = "John Test";
    public int age = 20;
    public float height = 2.5f;
    public float weight = 180;
    //really an acceleration value
    public float baseSpeed = 5f;
    public float maxSpeed = 5f;
    /// <summary>
    /// Called ALONGSIDE(after) an update to match the player's input. Change velocity etc here
    /// </summary>
    public virtual void ControlledUpdate()
    {
    }
    /// <summary>
    /// Should handle all behavior when not controlled by the player
    /// </summary>
    public virtual void UncontrolledUpdate()
    {
    }

    public virtual bool CanBeControlled()
    {
        return true;
    }
    public virtual void OnControl()
    {
    }
    public virtual void OnEject()
    {
    }
    public float CalcSpeed()
    {
        return baseSpeed;
    }
    void Start()
    {
        movement = InputSystem.actions.FindAction("Move");
        transfer = InputSystem.actions.FindAction("Transfer");
    }
    InputAction movement= null;
    InputAction transfer = null;
    void Update()
    {
        float h = movement.ReadValue<Vector2>().x;
        float v = movement.ReadValue<Vector2>().y;

        Debug.Log(h+"."+ v);
        var rb = GetComponent<Rigidbody2D>();
        if (isControlled)
        {
            if(sanity <= 0)
            {
                //TODO eject
                isControlled = false;
            }
            if (new Vector2(h, v).magnitude != 0)
            {
                
                rb.AddForce(new Vector2(h, v).normalized * CalcSpeed());
                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }
            }
            ControlledUpdate();
        }
        else
        {
            UncontrolledUpdate();
        }
        if(new Vector2(h,v).magnitude == 0)
        {
            rb.AddForce(-rb.linearVelocity * friction);
        }
    }
}
