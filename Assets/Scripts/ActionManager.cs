using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    //Find empty positions
    //Player action
    //If player swaps position:
    //-Remove oldPosition from empty positions
    //-Add current position to empty positions
    //Set target position
    //Remove Target position from empty positions
    //Rotate towards TargetPosition
    //Move to TargetPosition

    [SerializeField] List<Vector3> emptyPosition;

    // Start is called before the first frame update
    void Start()
    {
        FindEmptyPositions();   
    }

    void FindEmptyPositions()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 RoundedPosition(Vector3 _position)
    {
        Vector3 roundedPosition = new Vector3(Mathf.Round(_position.x), 0, Mathf.Round(_position.z));
        return roundedPosition;
    }
}
