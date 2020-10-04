using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Unit
{
    private float spawnTime = 0;
    private bool hasSomething = false; // checking if the queue is not empty
    [SerializeField] private Queue<GameObject> spawningUnitQueue = new Queue<GameObject>(); 

    public enum State { inConstruction = 0, constructed = 1};
    public State state;

    [Header("Units can generate")]
    [SerializeField] private List<GameObject> units = new List<GameObject>(); 
    /*
        List of units you can create with this building 
    */
    [SerializeField] private int maxUnitsCanCreate;

    [Header("Other protperties")]
    [SerializeField] private Transform spawnPos; 
   

    public void CreateUnit(int unitIndex)
    {   
        spawningUnitQueue.Enqueue(units[unitIndex]);
        if (!hasSomething)
        {
            StartCoroutine(StartCreatingUnit()); 
        }
        hasSomething = true; 
    }

    private IEnumerator StartCreatingUnit()
    {   
        spawnTime = 0; 
        GameObject pudlyToSpawn = spawningUnitQueue.Peek().gameObject; 
        while (spawnTime < pudlyToSpawn.GetComponent<Puddly>().spawnTime)
        {
            spawnTime += Time.deltaTime;
            yield return null; 
        }
        Instantiate(pudlyToSpawn.gameObject, spawnPos.transform.position, Quaternion.identity);
        spawningUnitQueue.Dequeue(); 
        if(spawningUnitQueue.Count > 0)
        {
            StartCoroutine(StartCreatingUnit());
        }else if(spawningUnitQueue.Count == 0)
        {
            hasSomething = false;
        }
    }
     
    
}
