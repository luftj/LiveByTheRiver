using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionHandling : MonoBehaviour
{
    public List<GameObject> selectedObjects;
    GameObject hoveredObj;
    Rect? selectionRect = null;
    GameObject boxUIElement;
    public GameObject selectionHighlight;
    public GameObject canvas;
    List<GameObject> selectionHighlights = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        boxUIElement = GameObject.Find("SelectionBox");
    }

    void updateDrag(Vector3 endCorner)
    {
        // update selection box ui element
        RectTransform rt =  boxUIElement.GetComponent<RectTransform>();
        float width = endCorner.x - selectionRect.Value.position.x;
        float height = selectionRect.Value.position.y - endCorner.y;
        float x = width < 0 ? endCorner.x : selectionRect.Value.position.x; // UI elements can't have negative dimensions
        float y = height < 0 ? endCorner.y : selectionRect.Value.position.y;
        boxUIElement.transform.position = new Vector3(x, y, 0);
        selectionRect = new Rect(selectionRect.Value.position.x,selectionRect.Value.position.y,width,-height);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(height));
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Abs(width));
        rt.ForceUpdateRectTransforms();
    }

    void endDrag()
    {
        deselectAll();

        // find selected objects
        SelectableObject[] selectables = GameObject.FindObjectsOfType<SelectableObject>();
        foreach(var obj in selectables)
        {
            // get screen pos
            Vector3 pos = Camera.main.WorldToScreenPoint(obj.transform.position);

            // check if in box
            if(selectionRect.Value.Contains(pos,allowInverse:true))
            {
                selectObject(obj.gameObject);
            }
        }

        // reset everything else
        selectionRect = null;
        boxUIElement.GetComponent<UnityEngine.UI.Image>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // box select
        if(Input.GetButtonDown("Fire1"))
        {
            // start drag
            boxUIElement.transform.position = Input.mousePosition;
            boxUIElement.GetComponent<UnityEngine.UI.Image>().enabled = true;
            selectionRect = new Rect(Input.mousePosition.x,Input.mousePosition.y,0,0);
        }

        if(selectionRect != null)
        {
            updateDrag(Input.mousePosition);
        }

        if(Input.GetButtonUp("Fire1"))
        {
            endDrag();

            // LMB click to select
            if(hoveredObj != null)
                this.selectObject(hoveredObj);
        }

        // RMB click for action
        if(Input.GetButton("Fire2"))
        {
            if(selectedObjects.Count == 0)
            {
                return; // no units selected, nothin to do here
            }
            foreach(GameObject obj in selectedObjects)
            {
                Vector3? target = EnvironmentPositionUnderCursor();
                if(target != null)
                    obj.GetComponent<AIComponent>().currentCommand = new AIMoveCommand(obj, (Vector3)target);
            }
        }

        // update selection highlights
        for(int i = 0; i < selectionHighlights.Count; ++i)
        {
            var sh = selectionHighlights[i];
            sh.transform.position = Camera.main.WorldToScreenPoint(selectedObjects[i].transform.position);

            // set size
            Collider r = selectedObjects[i].GetComponent<Collider>();
            Vector2 sh_rect = worldBoundsToScreenRect(r.bounds) * 1.15f;
            sh.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sh_rect.x);
            sh.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sh_rect.y);
        }
    }

    public void selectObject(GameObject obj)
    {
        selectedObjects.Add(obj);

        Renderer r = obj.GetComponent<Renderer>();
        r.material.color = Color.red; // set selection highlight
        makeSelectionHighlighUI(obj);
    }

    public void makeSelectionHighlighUI(GameObject selectedObj)
    {
        Renderer r = selectedObj.GetComponent<Renderer>();
        Vector3 c = Camera.main.WorldToScreenPoint(r.bounds.center);
        
        
        var sh = Instantiate(selectionHighlight,c,Quaternion.identity,canvas.transform);
        selectionHighlights.Add(sh);
    }

    public Vector2 worldBoundsToScreenRect(Bounds bounds)
    {
        List<Vector3> points = new List<Vector3> {
            Camera.main.WorldToScreenPoint(bounds.center + new Vector3( bounds.extents.x, bounds.extents.y, bounds.extents.z)),
            Camera.main.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z)),
            Camera.main.WorldToScreenPoint(bounds.center + new Vector3( bounds.extents.x,-bounds.extents.y, bounds.extents.z)),
            Camera.main.WorldToScreenPoint(bounds.center + new Vector3( bounds.extents.x, bounds.extents.y,-bounds.extents.z)),
            Camera.main.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x,-bounds.extents.y, bounds.extents.z)),
            Camera.main.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y,-bounds.extents.z)),
            Camera.main.WorldToScreenPoint(bounds.center + new Vector3( bounds.extents.x,-bounds.extents.y,-bounds.extents.z)),
            Camera.main.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x,-bounds.extents.y,-bounds.extents.z))
            };
        float minx = points.OrderBy( v => v.x ).First().x;
        float maxx = points.OrderByDescending( v => v.x ).First().x;
        float miny = points.OrderBy( v => v.y ).First().y;
        float maxy = points.OrderByDescending( v => v.y ).First().y;
        return new Vector2(maxx-minx, maxy-miny);
    }

    public void reset() {
        selectionRect = null;
        deselectAll();
    }
    
    public void select_toggle()
    {
        if(selectedObjects.Contains(hoveredObj))
        {
            int idx = selectedObjects.IndexOf(hoveredObj);
            selectedObjects.Remove(hoveredObj);
            Destroy(selectionHighlights[idx]);
            selectionHighlights.RemoveAt(idx);
        }
        else
        {
            selectObject(hoveredObj);
        }
    }

    public void deselectAll()
    {
        foreach(GameObject obj in selectedObjects)
        {
            Renderer r = obj.GetComponent<Renderer>();
            r.material.color = Color.white;
        }
        selectedObjects.Clear();
        foreach(var obj in selectionHighlights)
        {
            Destroy(obj);
        }
        selectionHighlights.Clear();
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
