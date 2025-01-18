using System;
using UnityEngine;
/// <summary>
/// represents something that the player can take control of
/// </summary>
public abstract class Controllable : MonoBehaviour
{
    public ControllableData data ;
    public bool isControlled = false;
    public int sanity = 100;
    public float friction = 0.99f;
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
    void Start()
    {
        this.data = ControllableData.getRandomizedControllableData();
        Debug.Log(data);
    }

    public virtual float GetSpeed()
    {
        //todo add in support for modifiers
        return (data?.CalcBaseSpeed()).GetValueOrDefault();
    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
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
                
                rb.AddForce(new Vector2(h, v).normalized * GetSpeed());
                if (rb.linearVelocity.magnitude > data.maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * data.maxSpeed;
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
