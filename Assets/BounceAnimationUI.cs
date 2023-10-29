using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnimationUU : MonoBehaviour
{

    Transform tr;
    Vector3 startSize;
    float scale = 1;
    float rotascale = 1;
    
    [SerializeField] Vector3 modVector = new Vector3(0, 1);
    [SerializeField] float bouncespeed;


    Vector3 startRota;
    [SerializeField] Vector3 rotaVector = new Vector3(0, 1, 0);

    [SerializeField] float rotaSpeed = .2f;

    private void Start()
    {
        tr = transform;
        startSize = tr.localScale;
        startRota = tr.localEulerAngles;
    }


    private void Update()
    {
        scale = Mathf.Sin(Time.time * bouncespeed);
        tr.localScale = startSize + modVector*scale;


        rotascale = Mathf.Sin(Time.time * rotaSpeed) ;
        tr.localEulerAngles = startRota + rotaVector * rotascale;
    }


}
