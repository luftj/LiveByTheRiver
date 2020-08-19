using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : MonoBehaviour
{
    public AICommand currentCommand = null;

    // Update is called once per frame
    void Update()
    {
        if(currentCommand != null)
        {
            currentCommand.Update(Time.deltaTime);
            if(currentCommand.finished)
                currentCommand=null;
        }
    }

    public void AddCommand()
    {

    }
}
