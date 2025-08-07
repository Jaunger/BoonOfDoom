using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrolPath : MonoBehaviour
{
    public int patrolPathID = 0;
    public List<Vector3> patrolPoints = new();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
           patrolPoints.Add(transform.GetChild(i).position);
        }

        WorldAIManager.instance.AddPatrolPathToList(this);
    }
}
