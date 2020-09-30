using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems; 
using UnityEngine;

public class God : MonoBehaviour
{

    private Camera cam; 
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
    [SerializeField] List<Unit> units = new List<Unit>();

    // units you have created
    [SerializeField] List<Unit> createdUnits = new List<Unit>(); 
    

    //by now, not important, is for UI check
    private int FingerID = -1; 

    private bool buildCooled = true; /*by now, this varuable is not important, used to cool dow a little bit the constructions*/

    //variables for the selection box
    private Vector2 startPos;
    private Vector2 endPos; 
    
    // states 
    public enum States {Free = 0, HasVillager = 1, HasWarrior = 2, HasMixed = 3, HasBuilding = 4 }; //mixed means has warriors and villagers
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
    public RectTransform selectionBox;


    [Header("Masks")]
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask selectableUnitMask; 

    [Header("others")]
    public States state;
    public BuildingSel buildSel; 
    void Start()
    {
        cam = Camera.main; 
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
            if(units.Count > 0)
                UnSelectUnit(); 
            
        }
        #region one one unit selection
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
        {
            GameObject unit = SelectObject(selectableUnitMask);
            units.Add(unit.GetComponent<Unit>());
            unit = null; 
        } else if (Input.GetMouseButtonDown(0))
        {
            //select a unit this includes: buildings, villagers, warriors
            startPos = Input.mousePosition;             
            
            if (!isConstructing)
            {
                //selecting a unit   
                GameObject unit = SelectObject(selectableUnitMask);
                if (unit != null)
                {
                    units.Clear(); 
                    canvas.nameText.text = unit.GetComponent<Unit>().unitName;
                    if (unit.gameObject.tag == "Villager")
                    {               
                        state = States.HasVillager;                      
                        units.Add(unit.GetComponent<Unit>()); 
                    }
                    else
                    {
                        state = States.Free; 
                    }                  
                }           
            }
        }else if (Input.GetMouseButtonDown(1))
        {
            //make an action with villager, warrior or building
            if(state == States.HasVillager)
            {
                //get villager properties
                if(units.Count > 0)
                {
                    for(int i = 0; i < units.Count; i++)
                    {
                        if(units[i].transform.tag == "Villager")
                        {
                            units[i].gameObject.GetComponent<Puddly>().MoveDestination(DestinationPoint()); 
                        }
                    }
                }
            }
        }
        #endregion

        #region multiple selectiom
        //end box selection
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox(); 
        }

        //start box selection
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition); 
        }
        #endregion


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

    Vector3 DestinationPoint()
    {
        
        if(!EventSystem.current.IsPointerOverGameObject()){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask))
            {             
                if (hit.transform.gameObject.GetComponent<Tiles>().state == Tiles.States.free)                   
                    return hit.transform.position;
            }
        }
        return this.transform.position; 
    }

    void UnSelectUnit()
    {
        if (units.Count > 0)
        {
            canvas.nameText.text = "";
            units.Clear(); 
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

    void UpdateSelectionBox(Vector2 currentMousePos)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        float width = currentMousePos.x - startPos.x;
        float height = currentMousePos.y - startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = startPos +  new Vector2(width / 2 , height / 2 ); 
    }

    void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2); 

        foreach(Unit puddly in createdUnits)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(puddly.transform.position); 

            if(screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
            {
                units.Add(puddly);          
            }
        }

        if(units.Count > 0)
        {
            state = States.HasVillager; 
        }
    }
    #endregion

    void SetBuilding()
    {
        //build the building
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
