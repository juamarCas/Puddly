using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class Puddly : Unit
{
    private NavMeshAgent agent; 

    [Header("Attributes")]
    public float velocity;
    public float angularSpeed;
    public float spawnTime = 5.0f; 
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocity;
        agent.angularSpeed = angularSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveDestination(Vector3 dest)
    {
        agent.SetDestination(dest); 
    }
}
