using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHandler : MonoBehaviour
{

    [SerializeField] Transform Varsi;
    [SerializeField] Transform Kasii;


    [SerializeField] Vector3 DestinationPoint;

    float speed = 10f;
    [SerializeField] Vector3 growspeed;

    public void SetGotoPos(float _pos) => DestinationPoint = new Vector3(0,0.017f, _pos);
    public bool handDone = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Kasii.transform.localPosition = Vector3.MoveTowards(Kasii.transform.localPosition, DestinationPoint, speed * Time.deltaTime);

        if (Kasii.transform.localPosition != DestinationPoint)
        {
            Varsi.localScale += growspeed * Time.deltaTime;
        }
        else
        {
            Debug.Log("A");
            handDone = true;
        }

        
    }
}
