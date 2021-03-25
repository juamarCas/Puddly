
using UnityEngine;

public class CheckIfIsInScreen : MonoBehaviour
{
    [SerializeField] private God god;
    [SerializeField] GameObject u; 
    Unit thisUnit;
    public bool isVisible = false;
    private void Awake()
    {
        thisUnit = u.gameObject.GetComponent<Unit>(); 
    }

    private void OnBecameInvisible()
    {
        god.GetPuddlyCreatedList().Remove(thisUnit);
    }

    private void OnBecameVisible()
    {
        god.GetPuddlyCreatedList().Add(thisUnit); 
    }
}
