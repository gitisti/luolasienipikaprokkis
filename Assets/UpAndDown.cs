using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDown : MonoBehaviour
{



    Transform tr;
    Vector3 startPos;
    float scale = 1;
    float rotascale = 1;

    [SerializeField] Vector3 modVector = new Vector3(0, 1);
    [SerializeField] float bouncespeed;


    Vector3 startRota;


    // Start is called before the first frame update
    void Start()
    {
        tr = transform;
        startPos = tr.localPosition;
        
    }

    // Update is called once per frame
    void Update()
    {

        scale = Mathf.Sin(Time.time * bouncespeed);
        tr.localPosition = startPos + modVector * scale;
    }
}
