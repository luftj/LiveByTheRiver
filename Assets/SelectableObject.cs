using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    SelectionHandling selectionManager;

    // Start is called before the first frame update
    void Start()
    {
        selectionManager = FindObjectOfType<SelectionHandling>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter() {
        selectionManager.hover(this.gameObject);
    }

    private void OnMouseExit() {
        selectionManager.unhover();
    }
}
