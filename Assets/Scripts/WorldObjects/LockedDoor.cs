using UnityEngine;

public enum DoorType
{
    Key,
    //More if needed
}
public class LockedDoor : MonoBehaviour
{
    string keyName = "";
    public DoorType doorType = DoorType.Key;
    void Start()
    {
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");
        var c = collision.gameObject.GetComponent<Controllable>();
        if (c)
        {
            switch (doorType)
            {
                case DoorType.Key:
                    if (c.canOpenDoor(this))
                    {
                        Destroy(this.gameObject);
                    }
                    break;
            }
        }
    }
    void Update()
    {
        
    }
}
