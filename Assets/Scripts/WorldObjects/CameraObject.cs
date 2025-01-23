using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
/// <summary>
/// Not a camera like the Unity camera, but a camera object that can be placed in the world.
/// </summary>
public class CameraObject : MonoBehaviour
{
    public LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentRotation = StartRotation;
    }
    /// <summary>
    /// In deg
    /// </summary>
    public float StartRotation = 0;
    bool goingRight = true;
    
    public float currentRotation;
    /// <summary>
    /// In deg, doubled if mirror is true
    /// </summary>
    public float RotationRange = 45;
    /// <summary>
    /// Max see distance
    /// </summary>
    public float Range = 10;
    /// <float>
    /// In deg/sec
    /// </summary>
    public float Speed = 15;
    /// <summary>
    /// If the camera should be mirrored(ie range is added to both sides)
    /// </summary>
    public bool Mirror = false;
    /// <summary>
    /// The width of the camera
    /// </summary>
    public float Width = 15;
    public bool Tripped = false;
    /// <summary>
    /// If the camera should be continous - will negate the rotation range
    /// </summary>
    public bool Continous = false;
    public List<Controllable> GetAllInRange()
    {
        var rad = currentRotation * Math.PI / 180;
        Vector2 pos = this.transform.position;
        Vector2 offset = new Vector2(1, 0);
        Vector2 left = pos + offset.RotateBy((float)rad) * (float)Range;
        Vector2 right = pos + offset.RotateBy((float)(rad + (Width * ((float)Math.PI) / 180f))) * Range;
        var allInRange = Physics2D.OverlapCircleAll(this.transform.position, Range);
        List<Controllable> returnval = new List<Controllable>();
        foreach(var item in allInRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, pos.DirectionTo(item.transform.position.ToVector2()), this.Range);
            if(hit.collider != null  && hit.collider != item) {
                continue;
            }
            var controllable = item.GetComponent<Controllable>();
            if (controllable != null && item.transform.position.ToVector2().IsInTriangle(pos, left, right) )
            {
                returnval.Add(controllable);
            }
        }
        return returnval;
    }

    /// <summary>
    /// Only here for debugging should alawys be this
    /// </summary>
    const float RotationOffset = 90;

    void Update()
    {
        Physics2D.queriesHitTriggers = false;
        var rad = currentRotation * Math.PI / 180;
        this.transform.eulerAngles = Vector3.forward * (currentRotation + RotationOffset);
        lineRenderer.positionCount = 0;
        currentRotation += Time.deltaTime *( goingRight ? -1 : 1) * Speed;
        Vector2 pos = this.transform.position;
        Vector2 offset = new Vector2(1, 0);
        Vector2 left = pos + offset.RotateBy((float)rad) * (float)Range;
        RaycastHit2D hit = Physics2D.Raycast(pos, pos.DirectionTo(left), this.Range);
        if (hit)
        {
            left = hit.point;
        }
        /*float mag = Math.Abs((hit.point - pos).magnitude);
        if(mag < left.magnitude)
        {
            left *= mag / left.magnitude;
        }*/
        Vector2 right = pos + offset.RotateBy((float)(rad + (Width * ((float)Math.PI) / 180f))) * Range;
        hit = Physics2D.Raycast(pos, pos.DirectionTo(right), this.Range);
        if (hit)
        {
            right = hit.point;
        }
        /*  mag = Math.Abs((hit.point - pos).magnitude);
         if (mag < right.magnitude)
         {
             right *= mag / right.magnitude;
         }*/
        foreach (Controllable c in GetAllInRange())
        {
            if (c.onSpotByCamera())
            {
                this.Tripped = true;
            }
        }
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.startColor = Tripped ? Color.red : Color.green;
        lineRenderer.endColor = Tripped ? Color.red : Color.green;
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, left);
        lineRenderer.SetPosition(1, pos);
        lineRenderer.SetPosition(2, right);
        
        if (Math.Abs(currentRotation) > RotationRange && !Continous)
        {
            goingRight = !goingRight;
        }
        Physics2D.queriesHitTriggers = true;
        this.Tripped = false;
    }
    public void OnDrawGizmos()
    {
        var rad = currentRotation * Math.PI / 180;
        Vector2 pos = this.transform.position;
        Vector2 offset = new Vector2(1, 0);
        Vector2 left = pos + offset.RotateBy((float)rad) * (float)Range;
        Vector2 right = pos + offset.RotateBy((float)(rad + (Width * ((float)Math.PI) / 180f))) * Range;
        Gizmos.DrawLine(pos, left);
        Gizmos.DrawLine(pos, right);
        Gizmos.DrawLine(left, right);
    }
}
