using UnityEngine;

public class ClearanceZone : MonoBehaviour
{
    public int clearanceLevel = 0;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Controllable>() is Controllable c )
        {

            c.currentZoneClearance = clearanceLevel;
            if (c.isControlled && c.Clearance < this.clearanceLevel)
            {
                c.onEnterIllegalZone();
            }
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Controllable>() is Controllable c )
        {
            Debug.Log("left");
            c.currentZoneClearance = 0;
            if(c.isControlled && c.Clearance > this.clearanceLevel)
            {
                c.onExitIllegalZone();
            }
        }
    }
}
