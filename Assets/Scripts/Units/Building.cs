using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Unit
{
    private float spawnTime = 0;
    private bool hasSomething = false; // checking if the queue is not empty
    private bool hasFoundPosition = false;
   
    [SerializeField] private Queue<Puddly> spawningUnitQueue = new Queue<Puddly>();
    

    public enum State { inConstruction = 0, constructed = 1};
    public State state;

    [Header("Units can generate")]
    [SerializeField] private List<Puddly> units = new List<Puddly>(); 
    /*
        List of units you can create with this building 
    */
    [SerializeField] private int maxUnitsCanCreate;

    [Header("Other protperties")]
    [SerializeField] private Transform Checker; 
    [SerializeField] private Transform spawnPos;
    [SerializeField] private List<Transform> spawnPositions = new List<Transform>();

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
        while (spawnTime < spawningUnitQueue.Peek().spawnTime)
        {
            spawnTime += Time.deltaTime;
            yield return null; 
        }
        spawnPos = CheckPosition();
        if(hasFoundPosition)
            Instantiate(spawningUnitQueue.Dequeue().gameObject, spawnPos.transform.position, Quaternion.identity);
         
        if(spawningUnitQueue.Count > 0 && hasFoundPosition)
        {
            StartCoroutine(StartCreatingUnit());
        }else if(spawningUnitQueue.Count == 0)
        {
            hasSomething = false;
        }
    }

    private Transform CheckPosition()
    {
        //Check if that position is avaible
        Transform pos;
        hasFoundPosition = false; 
        for(int i = 0; i < spawnPositions.Count; i++)
        {
            Checker.transform.position = new Vector3(spawnPositions[i].transform.position.x, spawnPositions[i].transform.position.y + 10, spawnPositions[i].transform.position.z); 
            RaycastHit hit;
            Ray ray = new Ray(Checker.transform.position, Vector3.down);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(hit.transform.gameObject.tag != "Tile")
                {
                    hasFoundPosition = false; 
                    continue; 
                }
                hasFoundPosition = true; 
                pos = spawnPositions[i];
                return pos; 
            }
           
        }
        return null; 
    }


     
    
}
