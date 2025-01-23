using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A entity that can spot other entities and rotates a full 360 degrees
/// </summary>
public class Spotter
{
    public Vector2 position = new Vector2(0, 0);
    public float SightRange = 10f;
    public float SightAngle = 45f;
    public float currentAngle = 0f;
    public float DegreesASecond = 90f;
    public void SetRotation(Vector2 rot)
    {
        LookAt(position + rot);
    }
    public void LookAt(Vector2 pos)
    {
        currentAngle = position.DirectionTo(pos).ToRotation() - (SightAngle/2f);
    }
    public void Update()
    {
        currentAngle += Time.deltaTime * (90f * Mathf.PI/180);
    }
    public List<Controllable> GetAllInSight()
    {

        Physics2D.queriesHitTriggers = false;
        var rad = currentAngle * Math.PI / 180;
        Vector2 pos = position;
        Vector2 offset = new Vector2(1, 0);
        Vector2 left = pos + offset.RotateBy((float)rad) * (float)SightRange;
        Vector2 right = pos + offset.RotateBy((float)(rad + (SightAngle * ((float)Math.PI) / 180f))) * SightRange;
        var allInRange = Physics2D.OverlapCircleAll(position, SightRange);
        List<Controllable> returnval = new List<Controllable>();
        foreach (var item in allInRange)
        {
            Debug.Log("item spotted");
            RaycastHit2D hit = Physics2D.Raycast(pos, pos.DirectionTo(item.transform.position.ToVector2()), this.SightRange);
            if (hit.collider != null && hit.collider != item)
            {
                continue;
            }
            var controllable = item.GetComponent<Controllable>();
            if (controllable != null && item.transform.position.ToVector2().IsInTriangle(pos, left, right))
            {
                returnval.Add(controllable);
            }
        }
        Physics2D.queriesHitTriggers = true;
        return returnval;

    }
}
