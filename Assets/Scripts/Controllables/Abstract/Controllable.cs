
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
/// <summary>
/// represents something that the player can take control of
/// </summary>
public abstract class Controllable : MonoBehaviour
{
    public GameObject inv;  
    public SpriteRenderer spriteRender;
    public int Suspicion = 0;
    public static Controllable Current = null;
    public List<DialogModifier> Dialogs = new List<DialogModifier>();
    public void RandomizeData()
    {
        fullname = "John Test";//todo name generation
        age = Random.Range(18, 100);
        height = Random.Range(1.5f, 2.5f);
        weight = Random.Range(50, 100);
        baseSpeed = Random.Range(5, 6);
        maxSpeed = baseSpeed;
    }
    public bool isControlled = false;
    public int sanity = 100;
    public float friction = 0.9f;
    public string fullname = "John Test";
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
    /// <summary>
    /// Called while the player is in range of this body and holding swap
    /// </summary>
    /// <returns>If this body should count as a vaild option</returns>
    public virtual bool CanBeControlled()
    {
        return true;
    }
    /// <summary>
    /// called when the player takes control of this body
    /// </summary>
    public virtual void OnControl()
    {
    }
    /// <summary>
    /// Called when the player leaves this body
    /// </summary>
    public virtual void OnEject()
    {
    }
    public float CalcSpeed()
    {
        return baseSpeed;
    }
    public HotbarSlot[] hotbarSlots = null;
    public void Start()
    {   
        spriteRender = GetComponent<SpriteRenderer>();
        movement = InputSystem.actions.FindAction("Move");
        transfer = InputSystem.actions.FindAction("Transfer");
        inv = GameObject.Find("Inventory");
        hotbarSlots = new HotbarSlot[inv.transform.childCount];
        for(var i = 0; i < inv.transform.childCount; i++)
        {
            var slot = inv.transform.GetChild(i).GetComponent<HotbarSlot>();
            hotbarSlots[i] = slot;
        }   
    }
    GameObject lineObject;
    InputAction movement = null;
    InputAction transfer = null;
    /// <summary>
    /// Currently channeling a swap to
    /// </summary>
    public Controllable? target = null;
    float swapProgress = -1;
    /// <summary>
    /// Time in seconds minus 1 to swap to another controllable
    /// </summary>
    public float swapTime = 1;
    /// <summary>
    /// The radius to search for other controllables to swap to
    /// </summary>
    public float DialogSearchRadius = 20f;
    public float ControlRange = 20f;
    public  int Clearance;
    public  int currentZoneClearance;

    void Update()
    {
        float h = movement.ReadValue<Vector2>().x;
        float v = movement.ReadValue<Vector2>().y;


        var rb = GetComponent<Rigidbody2D>();
        if (isControlled)
        {
            Current = this;
            GetComponent<SpriteRenderer>().color = Color.red;
            if (sanity <= 0)
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
            if (transfer.IsPressed())
            {
                if (target == null)
                {
                    var hits = Physics2D.OverlapCircleAll(transform.position, ControlRange);
                    float nearest = float.MaxValue;
                    Controllable? nearestControllable = null;
                    foreach (var hit in hits)
                    {
                        var controllable = hit.GetComponent<Controllable>();
                        if (controllable != null && controllable != this && controllable.CanBeControlled())
                        {
                            var dist = (controllable.transform.position - transform.position).magnitude;
                            if (dist < nearest)
                            {
                                nearest = dist;
                                nearestControllable = controllable;
                            }
                        }
                    }
                    if (nearestControllable != null)
                    {
                        target = nearestControllable;
                    }
                }
                if (lineObject == null)
                {
                    lineObject = new GameObject("DynamicLine");
                    var lR = lineObject.AddComponent<LineRenderer>();
                    lR.startWidth = 0.1f;
                    lR.endWidth = 0.5f;
                    lR.material = new Material(Shader.Find("Sprites/Default"));
                    lR.positionCount = 2;

                }
                Vector3[] posisitions = new Vector3[] { transform.position, target.transform.position };
                LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
                lineRenderer.SetPositions(posisitions);
                lineRenderer.endWidth = (swapProgress/(swapTime + 1));
                swapProgress += Time.deltaTime;
                if(swapProgress > swapTime)
                {
                    CameraScript.StartSmoothMove(target.transform.position, 1f);
                    swapProgress = -1;
                    this.OnEject();
                    this.isControlled = false;
                    target.OnControl();
                    target.isControlled = true;
                    target = null;
                    Destroy(lineObject);
                    lineObject = null;
                    Current = target;
                }
            }
            else if (swapProgress > -1)
            {
                swapProgress = -1;
                Destroy(lineObject);
                lineObject = null;
                target = null;
            }
            if (new Vector2(h, v).magnitude == 0)
            {
                rb.AddForce(-rb.linearVelocity * friction);
            }
            ControlledUpdate();
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            UncontrolledUpdate();
        }
        if (GetComponent<Rigidbody2D>().linearVelocityX > 0)
        {
            spriteRender.flipX = false;
        }
        else if (GetComponent<Rigidbody2D>().linearVelocityX < 0) {
            spriteRender.flipX = true;
        }

    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 400);
    }
    public bool inIllegalZone
    {
        get
        {
            return this.Clearance < this.currentZoneClearance;
        }
    }
    public bool onSpotByCamera()
    {
        return true;
    }

    public void onEnterIllegalZone()
    {
    }
    public void onExitIllegalZone()
    {

    }

    public bool onSpotByGuard()
    {
        return true;
    }

    internal bool canOpenDoor(LockedDoor lockedDoor)
    {
        return true;
    }
    public Item[] items = new Item[3];
    public bool AddItem(Item item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                hotbarSlots[i].Item = item;
                return true;
            }
        }
        return false;
    }
}
