using System;
using UnityEngine;
public enum GuardState
{
    Patrol,
    Investigate,
    Combat,
    ReturnToPatrol,
    Search
}
public class Guard : Controllable
{
    public Spotter Spotter = new Spotter();
    public GuardState state = GuardState.Patrol;
    public override void UncontrolledUpdate()
    {
        Spotter.position = transform.position;
        Spotter.Update();
        switch (state)
        {
            case GuardState.Patrol:
                Patrol();
                break;
            case GuardState.Investigate:
                Investigate();
                break;
            case GuardState.Combat:
                Combat();
                break;
            case GuardState.ReturnToPatrol:
                ReturnToPatrol();
                break;
            case GuardState.Search:
                Search();
                break;  
        }
    }
    public float SearchTime = 3;
    public float CurrentSearchTime = 3;
    private void Search()
    {
        CurrentSearchTime -= Time.deltaTime;
        if (CurrentSearchTime <= 0)
        {
            foreach(var item in Spotter.GetAllInSight())
            {
                if (item.onSpotByGuard())
                {
                    state = GuardState.Combat;
                    return;
                }
            }   
            state = GuardState.ReturnToPatrol;
            CurrentSearchTime = 3;
        }
       
    }

    private void ReturnToPatrol()
    {
        var player = Controllable.Current;
        if (player == null)
        {
            state = GuardState.Patrol;
            return;
        }
        if (CurrentlyPathFinding)
        {
            return;
        }
        if (path == null || path.Length == 0 || currentIndex > path.Length || FindNewPath)
        {
            StartCoroutine(PathFinding.FindPath(this.transform.position, NormalPatrolPath[0], this));
            FindNewPath = false;
            return;
        }
        if (Vector2.Distance(transform.position, path[currentIndex]) < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= path.Length)
            {
                state = GuardState.Patrol;
                path = null;
                currentIndex = 0;
                Debug.Log("moved to player");
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().linearVelocity = (path[currentIndex] - (Vector2)transform.position).normalized * 2;
        }
    }
    public void NoticeDisturbance(Vector2 pos)
    {
        SearchLocation = pos;
        state = GuardState.Investigate;
        FindNewPath = true;
    }
    public Vector2[] NormalPatrolPath;
    public Vector2[] path;
    public Vector2 SearchLocation;
    public int currentIndex;
    public bool FindNewPath;
    public bool CurrentlyPathFinding;
    private void Combat()
    {
        var player = Controllable.Current;
        if (player == null)
        {
            state = GuardState.Patrol;
            return;
        }
        if (CurrentlyPathFinding)
        {
            return;
        }
        if (path == null || path.Length == 0 || currentIndex > path.Length || FindNewPath)
        {
            StartCoroutine(PathFinding.FindPath(this.transform.position, player.transform.position, this));
            FindNewPath = false;
            return;
        }
        if (Vector2.Distance(transform.position, path[currentIndex]) < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= path.Length)
            {
                state = GuardState.Patrol;
                path = null;
                currentIndex = 0;
                Debug.Log("moved to player");
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().linearVelocity = (path[currentIndex] - (Vector2)transform.position).normalized * 2;
        }

    }

    private void Investigate()
    {
        
        if (CurrentlyPathFinding)
        {
            return;
        }
        if (path == null || path.Length == 0 || currentIndex > path.Length || FindNewPath)
        {
            StartCoroutine(PathFinding.FindPath(this.transform.position, SearchLocation, this));
            FindNewPath = false;
            return;
        }
        if (Vector2.Distance(transform.position, path[currentIndex]) < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= path.Length)
            {
                state = GuardState.Search;
                path = null;
                currentIndex = 0;
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().linearVelocity = (path[currentIndex] - (Vector2)transform.position).normalized * 2;
        }
    }

    private void Patrol()
    {
        if (NormalPatrolPath== null || NormalPatrolPath.Length == 0 || currentIndex >= NormalPatrolPath.Length )
        {
            currentIndex = 0;
            return;

        }
        if (Vector2.Distance(transform.position, NormalPatrolPath[currentIndex]) < 0.1f)
        {
            currentIndex++;
           
        }
        else
        {
            GetComponent<Rigidbody2D>().linearVelocity = (NormalPatrolPath[currentIndex] - (Vector2)transform.position).normalized * 2;
        }
        CheckForPlayers();
    }

    private void CheckForPlayers()
    {
        Vector2 Forward = GetComponent<Rigidbody2D>().linearVelocity.normalized;
        Spotter.SetRotation(Forward);
        foreach (var item in Spotter.GetAllInSight())
        {
            Debug.Log("Player Spotted");
            if (item.onSpotByGuard())
            {
                state = GuardState.Combat;
                return;
            }
        }
    }
    public void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (path != null)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.red);
            }
        }
        if(state == GuardState.Investigate)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(SearchLocation, 0.5f);
        }
        if(NormalPatrolPath.Length > 0)
        {
            for (int i = 0; i < NormalPatrolPath.Length - 1; i++)
            {
                Debug.DrawLine(NormalPatrolPath[i], NormalPatrolPath[i + 1], Color.blue);
            }
        }
        //Spotter.DrawGizmo();

    }
}
    