using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHandling : MonoBehaviour
{
    public List<GameObject> selectedObjects;
    GameObject hoveredObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // LMB click to select
        if(Input.GetButton("Fire1"))
        {
            if(hoveredObj)
            {
                this.select_toggle();
            }
            else
            {
                this.unselectAll();
            }
        }

        // RMB click for action
        if(Input.GetButton("Fire2"))
        {
            if(selectedObjects.Count == 0)
            {

                // no units selected, nothin to do here
                return;
            }
            foreach(GameObject obj in selectedObjects)
            {
                Vector3? target = EnvironmentPositionUnderCursor();
                if(target != null)
                    obj.GetComponent<AIComponent>().currentCommand = new AIMoveCommand(obj, (Vector3)target);
            }
        }
    }

    public void select_toggle()
    {
        if(selectedObjects.Contains(hoveredObj))
        {
            selectedObjects.Remove(hoveredObj);
        }
        else
        {
            selectedObjects.Add(hoveredObj);
        }
    }

    public void unselectAll()
    {
        foreach(GameObject obj in selectedObjects)
        {
            Renderer r = obj.GetComponent<Renderer>();
            r.material.color = Color.white;
        }
        selectedObjects.Clear();
    }

    public void hover(GameObject obj)
    {
        hoveredObj = obj;
        Renderer r = hoveredObj.GetComponent<Renderer>();
        r.material.color = Color.green;
    }

    public void unhover()
    {
        Renderer r = hoveredObj.GetComponent<Renderer>();
        if(selectedObjects.Contains(hoveredObj))
        {
            r.material.color = Color.red;
        }
        else
        {
        r.material.color = Color.white;
        }
        hoveredObj = null;
    }

    GameObject ObjectUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            if(hitObject.GetComponent<SelectableObject>())
            {
                return hitObject;
            }
        }
        return null;
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
