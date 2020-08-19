using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newpos = this.transform.position;

        // lateral
        newpos.x += Input.GetAxis("Horizontal") * speed;
        newpos.z += Input.GetAxis("Vertical") * speed;

        // set
        this.transform.position = newpos;
    }
}
