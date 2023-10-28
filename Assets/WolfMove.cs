﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfMove : MonoBehaviour
{

    [SerializeField] Vector3 gotoPlace;
    [SerializeField] Vector3 gotoRota;
    public void SetGotoPlace(Vector3 _gotoPlace) => gotoPlace = new Vector3(_gotoPlace.x,transform.localPosition.y,_gotoPlace.z);
    public void SetPlace(Vector3 _place) => transform.position = new Vector3(_place.x, transform.localPosition.y, _place.z);
    public void SetGotoRota(Vector3 _gotoRota) => gotoRota = _gotoRota;
    public void SetRota(Vector3 _rota) => transform.localEulerAngles = _rota;

    Transform tr;

    // Start is called before the first frame update
    void Start()
    {
        tr = transform;
        gotoRota = tr.localEulerAngles;
        gotoPlace = tr.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
 
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(gotoRota), .3f);

        gotoPlace.y = tr.localPosition.y;
        tr.localPosition = Vector3.Lerp(tr.localPosition, gotoPlace, .3f);
    }
}