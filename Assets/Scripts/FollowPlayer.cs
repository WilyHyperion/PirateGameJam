using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 offset = new Vector3(0,0,0);

    void Update()
    {
        if(Controllable.Current == null)
        {
            return;
        }
        transform.position = Controllable.Current.transform.position + offset;
    }
}
