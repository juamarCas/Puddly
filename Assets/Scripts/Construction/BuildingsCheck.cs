
using UnityEngine;

public class BuildingsCheck : MonoBehaviour
{

    public bool canBuild = true;
    public int size;
    public int test = 0;

    private void OnTriggerStay(Collider other)
    {  
        if (other.tag == "Building" || other.tag == "TownCenter" || other.tag == "Villager")
        {
            test = 1;
            canBuild = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Resource" || other.tag == "Building" || other.tag == "Decoration" || other.tag == "Villager" || other.tag == "TownCenter")
        {
            canBuild = false; 
        }    
    }

    private void OnTriggerExit(Collider other)
    {     
        if (other.tag == "Building" || other.tag == "TownCenter" || other.tag == "Villager" || other.tag == "Decoration" || other.tag == "Resource")
        {
            canBuild = true; 
        }
    }


}
