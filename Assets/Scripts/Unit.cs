using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Unit : MonoBehaviour
{
    /*
     * This is script is to show info of the unit or building and options
     */
    private List<Tiles> tiles;
    [Header("Options")]
    [SerializeField] private GameObject optionsPanel; 

    // Start is called before the first frame update
    [Header("Characteristics")]
    public string unitName;
    public float health; 

   
    


   

    private void Awake()
    {
        tiles = new List<Tiles>(); 
    }

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
       
    }


    //add tiles that are under the unit
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Tile")
        {
            tiles.Add(other.gameObject.GetComponent<Tiles>()); 
        }
    }

    //well, it actually doesn't destroy anything, just reset the tiles states
    public void Destroy() 
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].state = Tiles.States.free;
            tiles.Remove(tiles[i]);
        }

        this.gameObject.SetActive(false); 
    }


}
