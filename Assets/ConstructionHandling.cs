using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionHandling : MonoBehaviour
{
    [SerializeField]
    GameObject beingPlaced;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(beingPlaced == null) return;

        Vector3? cursorPos = EnvironmentPositionUnderCursor();
        if(cursorPos.HasValue)
            beingPlaced.transform.position = cursorPos.Value;

        if(Input.GetButton("Fire1"))
        {
            // place Building
            beingPlaced = null;

            this.gameObject.GetComponent<SelectionHandling>().enabled = true; // release control over mouse
        }
    }

    public void AddNewConstruct(GameObject prefab)
    {
        Debug.Log("new construct");
        this.gameObject.GetComponent<SelectionHandling>().enabled = false; // get control over mouse
        this.gameObject.GetComponent<SelectionHandling>().reset();
        beingPlaced = Instantiate(prefab );
    }

    Vector3? EnvironmentPositionUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        int environLayerMask = 1 << 8;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, environLayerMask))
        {
            return hit.point;
        }
        return null;
    }
}
