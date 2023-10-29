using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHandler : MonoBehaviour
{

    [SerializeField] Transform Varsi;
    [SerializeField] Transform Kasii;


    [SerializeField] Vector3 DestinationPoint;
    [SerializeField] Vector3 startPos;

    float speed = 15f;
    [SerializeField] Vector3 growspeed;

    public void SetGotoPos(float _pos) => DestinationPoint = new Vector3(0,0.017f, _pos);
    public bool handDone = false;

    public bool GrabSomething = true;
    int phase = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPos = Kasii.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        switch (phase) {

            case 0:
        Kasii.transform.localPosition = Vector3.MoveTowards(Kasii.transform.localPosition, DestinationPoint, speed * Time.deltaTime);

        if (Kasii.transform.localPosition != DestinationPoint)
        {
            Varsi.localScale += growspeed * Time.deltaTime * (speed/10f);
        }
        else
        {
            Debug.Log("A");
                    if (GrabSomething)
                    {
                        handDone = true;
                    }
                    else { phase = 1; }
        }
                break;

            case 1:

                Kasii.transform.localPosition = Vector3.MoveTowards(Kasii.transform.localPosition, startPos, speed * Time.deltaTime * 2);

                if (Kasii.transform.localPosition != startPos)
                {
                    Varsi.localScale -= growspeed * Time.deltaTime * 2 * (speed / 10f);
                }
                else
                {
                   
                        handDone = true;
                    
                }

                break;
    }
    }
}
