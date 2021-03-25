using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.Raycasting
{
   
    public class RaycastHelper
    {
        private Camera cam; 
        public RaycastHelper()
        {
            cam = Camera.main; 
        }

        public GameObject RayTo(LayerMask desiredLayer, LayerMask optional = default)
        {
            if (!EventSystem.current.IsPointerOverGameObject()) //if the mouse is over the UI, dont execute this function
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, desiredLayer))
                {
                    return hit.transform.gameObject;
                }else if(Physics.Raycast(ray, out hit, Mathf.Infinity, optional))
                {
                    return hit.transform.gameObject;
                }
            }

            return null;
        }

        public Vector3 RayTo(LayerMask tileMask, Transform t) //overcharge
        {

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask))
                {
                    if (hit.transform.gameObject.GetComponent<Tiles>().state == Tiles.States.free)
                        return hit.transform.position;
                }
            }
            return t.position;
        }
    }
}