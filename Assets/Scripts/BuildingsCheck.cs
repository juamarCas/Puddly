using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsCheck : MonoBehaviour
{

    public bool canBuild = true;
    public int size;
    public int test = 0;

    public List<Tiles> tiles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTriggerStay(Collider other)
    {  
        if (other.tag == "Building")
        {
            Debug.Log(other.name);
            test = 1;
            canBuild = false;
        }

        if (tiles.Count < size)
        {
            canBuild = false;
        }
        else if (tiles.Count == size && test == 0)
        {
            canBuild = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            //Checking if the building can fit in that space
            if (other.gameObject.GetComponent<Tiles>().state == Tiles.States.free)
                tiles.Add(other.gameObject.GetComponent<Tiles>());  
            else if(other.gameObject.GetComponent<Tiles>().state == Tiles.States.hasConstruction)
            {
                tiles.Remove(other.gameObject.GetComponent<Tiles>()); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {     
        if (other.tag == "Building")
        {
            test = 0;
            canBuild = true; 
        }

        if(other.tag == "Tile")
        {
            if (other.gameObject.GetComponent<Tiles>().state == Tiles.States.free)
                tiles.Remove(other.gameObject.GetComponent<Tiles>());
        }
    }


}
