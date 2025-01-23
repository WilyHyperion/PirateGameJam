using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    void Start()
    {
        Instance = this;
    }
    public static Vector3 Offset = new Vector3 (0, 0, 0);
    public Vector2? goal = null;
    public Vector2? start = null;
    public float progress = 0f;
    public float totalTime = 0f;
    void Update()
    {

        if (goal != null) {
            progress += Time.deltaTime;
            if (progress >= totalTime)
            {
                transform.position = new Vector3(goal.Value.x, goal.Value.y, -10) + Offset;
                goal = null;
            }
            else
            {
                float t = progress / totalTime;
                transform.position = new Vector3(Mathf.Lerp(start.Value.x, goal.Value.x, t), Mathf.Lerp(start.Value.y, goal.Value.y, t), -10) + Offset;
            }
        }
        else
        {
            Transform target = Controllable.Current?.transform;
            if (target != null)
            {
                transform.position = new Vector3(target.position.x, target.position.y, -10) + Offset;
            }
        }
    }
    public static CameraScript Instance;
    public static void StartSmoothMove(Vector3 position, float v)
    {
        Instance.progress = 0;
        Debug.Log(position + " " +  v);
        Instance.start = Instance.transform.position;
        Instance.totalTime = v;
        Instance.goal = position;
    }
}
