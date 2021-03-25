
using UnityEngine;

public class RenderVisible : MonoBehaviour
{
    public bool isVisible = false;

    private void OnBecameVisible()
    {
        isVisible = true; 
    }

    private void OnBecameInvisible()
    {
        isVisible = false; 
    }
}
