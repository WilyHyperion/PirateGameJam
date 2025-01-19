
using UnityEngine;

using UnityEngine.InputSystem;
/// <summary>
/// represents something that the player can take control of
/// </summary>
public abstract class Controllable : MonoBehaviour
{
    public void RandomizeData()
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
    GameObject lineObject;
    InputAction movement = null;
    InputAction transfer = null;
    public Controllable? target = null;
    float swapProgress = -1;
    public float swapTime = 1;
    void Update()
    {
        float h = movement.ReadValue<Vector2>().x;
        float v = movement.ReadValue<Vector2>().y;


        var rb = GetComponent<Rigidbody2D>();
        if (isControlled)
        {
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
                    var hits = Physics2D.OverlapCircleAll(transform.position, 400);
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
                    Debug.Log(swapProgress);
                if(swapProgress > swapTime)
                {
                    swapProgress = -1;
                    this.OnEject();
                    this.isControlled = false;
                    target.OnControl();
                    target.isControlled = true;
                    target = null;
                    Destroy(lineObject);
                    lineObject = null;
                }
            }
            else if (swapProgress > -1)
            {
                swapProgress = -1;
                Destroy(lineObject);
                Debug.Log("failed");
                lineObject = null;
                target = null;
            }

                ControlledUpdate();
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            UncontrolledUpdate();
        }
        if (new Vector2(h, v).magnitude == 0)
        {
            rb.AddForce(-rb.linearVelocity * friction);
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 400);
    }
}
