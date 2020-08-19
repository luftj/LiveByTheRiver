using UnityEngine;      

public class AIMoveCommand : AICommand
{
    Vector3 targetPosition;
    GameObject owner;
    float speed = 1f;

    public AIMoveCommand(GameObject owner, Vector3 target)
    {
        this.owner=owner;
        this.targetPosition=target+Vector3.up*0.5f;
    }

    public override void Update(float deltaTime) {
        Vector3 direction = targetPosition - owner.transform.position;
        if(direction.magnitude <= speed * deltaTime)
        {
            owner.transform.position = targetPosition;
            this.finished = true;
            return;
        }
        // owner.GetComponent<Rigidbody>().MovePosition(direction.normalized * speed * deltaTime);
        owner.transform.Translate(direction.normalized * speed * deltaTime);
    }
}