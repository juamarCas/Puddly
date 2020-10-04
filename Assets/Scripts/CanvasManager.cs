using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    private enum Units {Villager = 0}; 
    [SerializeField] God god;

    // Start is called before the first frame update
    public TextMeshProUGUI nameText;

    [Header("Buttons")]
    [SerializeField] List<Unit> units = new List<Unit>(); //all unit selected
    void Start()
    {
        if (nameText != null)
        {
            nameText.text = " ";
        }
    }
    //create villager

    public void CreateVillager()
    {
        CreateUnit((int)Units.Villager); 
    }

    private void CreateUnit(int index)
    {
        units = god.GetUnits();
        if (units.Count > 0)
        {
            for (int i = 0; i < units.Count; i++)
            {
                Building building = units[i].gameObject.GetComponent<Building>();
                if (building != null)
                {
                    building.CreateUnit(index);
                }
            }

        }
    }
    
}
