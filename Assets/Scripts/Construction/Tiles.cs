
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public enum States { free = 0, hasConstruction = 1 }; 
    

    public States state; 
   

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Building")
        {
            //Change State to has construction
            state = States.hasConstruction;
         
        }
    }

    

}
