using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class CanvasManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI nameText; 
    void Start()
    {
        if(nameText != null)
        {
            nameText.text = " ";  
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
