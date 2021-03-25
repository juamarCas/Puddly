using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems; 
using UnityEngine;
using Utils.Raycasting; 

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

    private RaycastHelper rayHelper; 

    // saves the gameobject you interact with, the list of units o the unit
    [SerializeField] private List<Unit> units = new List<Unit>(); 

    // units you have created
    [SerializeField] private List<Unit> createdUnits = new List<Unit>(); //optimize this list, only the units in the camera view can enter this list


    //by now, not important, is for UI check
    private int FingerID = -1; 

    private bool buildCooled = true; /*by now, this varuable is not important, used to cool dow a little bit the constructions*/

    //variables for the selection box
    private Vector2 startPos;
    private Vector2 endPos; 
    
    // states 
    public enum States {Free = 0, HasVillager = 1, HasWarrior = 2, HasMixed = 3, HasTownCenter = 4 }; //mixed means has warriors and villagers
    public enum BuildingSel {Nothing = -1,TownCenter = 0, Armery = 1, Archers = 2}; 

    [Header("Components")]
    public float buildCoolDown;
    public Color canBuildHere;
    public Color cantBuildHere;
    public int maxPoblation = 100; 

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
    public GameObject UIHolder;
    public GameObject TownCenterOptions;
    public GameObject VillagerOptions; 


    [Header("Masks")]
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask selectableUnitMask; 

    [Header("others")]
    public States state;
    public BuildingSel buildSel; 
    void Start()
    {
        cam = Camera.main;
        rayHelper = new RaycastHelper(); 
        state = States.Free;
        UIHolder.gameObject.SetActive(false);
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
            buildSel = BuildingSel.TownCenter;
            selectedBuilding = (int)BuildingSel.TownCenter; 
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
            GameObject unit = rayHelper.RayTo(selectableUnitMask);
            if (unit != null)
                units.Add(unit.GetComponent<Unit>());
            else
                UnSelectUnit();

            Debug.Log(unit); 
                 
            unit = null; 
        } else if (Input.GetMouseButtonDown(0))
        {
            //select a unit this includes: buildings, villagers, warriors
            startPos = Input.mousePosition;             
            
            if (!isConstructing)
            {
                //selecting a unit   
                GameObject unit = rayHelper.RayTo(selectableUnitMask, tileMask);                                             
                if (unit != null)
                {
                    if (unit.tag == "Tile")
                    {
                        UnSelectUnit();
                        return;
                    }
                    Unit u = unit.GetComponent<Unit>();
                    UIHolder.gameObject.SetActive(true);
                    SetOffUI();
                    units.Clear();
                    canvas.nameText.text = u.unitName;
                    switch (unit.gameObject.tag)
                    {
                        case "Villager":
                            state = States.HasVillager;
                            units.Add(u);
                            break;
                        case "TownCenter":
                            state = States.HasTownCenter;
                            TownCenterOptions.gameObject.SetActive(true);
                            units.Add(u);
                            break;
                        default:
                            state = States.Free;
                            break;

                    }

                }
                
            }
        }else if (Input.GetMouseButtonDown(1))
        {
            //make an action with villager, warrior or building     
                //get villager properties
                if(units.Count > 0)
                {
                    foreach(Unit u in units)
                    {
                        if(u.GetType() == typeof(Puddly)) //ask if is a puddly
                        {
                            u.gameObject.GetComponent<Puddly>().MoveDestination(rayHelper.RayTo(tileMask, this.transform)); 
                        }
                    }                 
                }
            
        }
        #endregion

        #region multiple selectiom
        //end box selection
        if (Input.GetMouseButtonUp(0))
        {
            if(!isConstructing)
                ReleaseSelectionBox(); 
        }

        //start box selection
        if (Input.GetMouseButton(0))
        {
            if(!Input.GetKey(KeyCode.LeftShift))
                UpdateSelectionBox(Input.mousePosition); 
        }
        #endregion


        if (buildSel == BuildingSel.TownCenter)
        {
            hoverTile(); 
        }
    }
    #region Interaction

    void hoverTile()
    {
        //select a tile
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); 
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask))
        {
            hoveredTile = hit.transform.gameObject;
            Vector3 spawnPos = new Vector3(hoveredTile.transform.position.x, hoveredTile.transform.position.y+1.91f, hoveredTile.transform.position.z);
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
   
    void UnSelectUnit()
    {
        if (units.Count > 0)
        {           
            if (TownCenterOptions.gameObject.activeInHierarchy)
            {
                TownCenterOptions.SetActive(false); 
            }
            UIHolder.gameObject.SetActive(false);
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
        Building_ghost[selectedBuilding].transform.position = transform.position;
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

        if(createdUnits.Count > 0)
        {
            foreach (Unit puddly in createdUnits)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(puddly.transform.position);

                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    units.Add(puddly);
                }
            }
        }
       

        if(units.Count > 0)
        {
            state = States.HasVillager; 
        }
    }

    void SetOffUI()
    {
        TownCenterOptions.SetActive(false);
        VillagerOptions.SetActive(false); 
    }

    void SetBuilding()
    {
        //build the building
        Instantiate(Buildings[selectedBuilding], Building_ghost[selectedBuilding].transform.position, Building_ghost[selectedBuilding].transform.rotation);
        Building_ghost[selectedBuilding].transform.position = this.transform.position;
        Building_ghost[selectedBuilding].SetActive(false); 
    }
    #endregion


    private IEnumerator BuildingCoolDown()
    {
        yield return new WaitForSeconds(buildCoolDown);
        buildCooled = true; 
    }

    public List<Unit> GetUnits()
    {
        if(units.Count > 0 && state != States.HasMixed)
            return units;

        return null; 
    }

    public List<Unit> GetPuddlyCreatedList() { return createdUnits; }

}
