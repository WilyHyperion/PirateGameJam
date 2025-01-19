using UnityEngine;

public class Camera : MonoBehaviour
{
    public static Transform target;
    void Start()
    {
        
    }
    public static Vector3 Offset = new Vector3 (0, 0, 0);
    void Update()
    {
        if(target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, -10) + Offset;
        }
    }
}
