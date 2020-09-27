
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems; 
using UnityEngine;


public class God : MonoBehaviour
{


    //Selected tile and ghostBuilding
    private GameObject hoveredTile;
    private GameObject ghostBuilding;

    //index for selected building
    private int selectedBuilding; 

    //index of hovered building
    private int buildingIndex;


    //is selecting place to construct? 
    private bool isConstructing; 

    // saves the gameobject you interact with
    [SerializeField] GameObject unit;
    

    //by now, not important, is for UI check
    private int FingerID = -1; 

    private bool buildCooled = true; 
    
    
    // states 
    public enum States {Free = 0, HasVillager = 1};
    public enum BuildingSel {Nothing = -1,House = 0}

    [Header("Components")]
    public float buildCoolDown;
    public Color canBuildHere;
    public Color cantBuildHere;

    /*
           Color: 
            Red: (0.9, 0.4, 0.5, 1)
            green: (0.5, 0.9, 0.5, 1)
    */

    [Header("Buildings Lists")]
    //building ghost is a preview of the building i want to build and building is the actual building i want to build
    public List<GameObject> Building_ghost;
    public List<GameObject> Buildings;

    [Header("UI Elements")]
    [SerializeField] private CanvasManager canvas; 

    [Header("Masks")]
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask selectableUnitMask; 

    [Header("others")]
    public States state;
    public BuildingSel buildSel; 
    void Start()
    {
        state = States.Free;
        buildSel = BuildingSel.Nothing; 
        #if !UNITY_EDITOR
        fingerID = 0; 
        #endif

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            isConstructing = true; 
            buildSel = BuildingSel.House;
            selectedBuilding = (int)BuildingSel.House; 
        }else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(buildSel != BuildingSel.Nothing)
                SetFree(); 
            if(unit != null)
            {
                UnSelectUnit(); 
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            //select a unit this includes: buildings, villagers, warriors
                         
            
            if (!isConstructing)
            {
                //selecting a unit   
                unit = SelectObject(selectableUnitMask);
                if (unit != null)
                {
                    canvas.nameText.text = unit.GetComponent<Unit>().unitName;
                    if (unit.gameObject.tag == "Villager")
                    {
                        state = States.HasVillager;
                    }
                    else
                    {
                        state = States.Free; 
                    }                  
                }           
            }
        }else if (Input.GetMouseButtonDown(1) && state == States.HasVillager)
        {

        }
        if (buildSel == BuildingSel.House)
        {
            hoverTile(); 
        }
    }
    #region Interaction

    void hoverTile()
    {
        //select a tile
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask))
        {
            hoveredTile = hit.transform.gameObject;
            Vector3 spawnPos = new Vector3(hoveredTile.transform.position.x, hoveredTile.transform.position.y+1, hoveredTile.transform.position.z);
            Building_ghost[selectedBuilding].transform.position = spawnPos;
            Building_ghost[selectedBuilding].SetActive(true);
            if (Building_ghost[selectedBuilding].GetComponent<BuildingsCheck>().canBuild)
            {
                Building_ghost[selectedBuilding].GetComponent<Renderer>().material.SetColor("_Color", canBuildHere);
            }
            else
            {
                Building_ghost[selectedBuilding].GetComponent<Renderer>().material.SetColor("_Color", cantBuildHere);
            }

             
            if (Input.GetMouseButtonDown(0) && Building_ghost[selectedBuilding].GetComponent<BuildingsCheck>().canBuild && buildCooled)
            {
                SetBuilding(); 
            }
        }
    }

 
    GameObject SelectObject(LayerMask desiredLayer)
    {
        if (!EventSystem.current.IsPointerOverGameObject()) //if the mouse is over the UI, dont execute this function
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, desiredLayer))
            { 
                return hit.transform.gameObject;
            }
            else
            { 
                //if click any othe place, just unselect
                UnSelectUnit();
            }
        }
        
        return null; 
    }

    void UnSelectUnit()
    {
        if (unit != null)
        {
            canvas.nameText.text = " ";
            unit = null;
            state = States.Free; 
        }
       
    }

    void SetFree()
    {
        //free the user by reseting the state and returning the ghost object to this object place
        state = States.Free;
        buildSel = BuildingSel.Nothing;
        if (Building_ghost[selectedBuilding].activeSelf) Building_ghost[selectedBuilding].SetActive(false);
        Building_ghost[selectedBuilding].transform.position = this.transform.position;
        isConstructing = false; 
    }
    #endregion

    void SetBuilding()
    {
        //build the vuilding
        Instantiate(Buildings[selectedBuilding], Building_ghost[selectedBuilding].transform.position, Building_ghost[selectedBuilding].transform.rotation);
        Building_ghost[selectedBuilding].transform.position = this.transform.position;
        Building_ghost[selectedBuilding].SetActive(false); 
    }

    

    private IEnumerator BuildingCoolDown()
    {
        yield return new WaitForSeconds(buildCoolDown);
        buildCooled = true; 
    }

}
