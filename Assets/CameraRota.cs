using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRota : MonoBehaviour
{

    public int rota = 0;
    float curRota = 0f;
    float gotoRota = 0f;
    float offset = 0f;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.localEulerAngles.y;
        curRota = 0;
        gotoRota = curRota;
        
    }

    // Update is called once per frame
    void Update()
    {
        curRota = Mathf.Lerp(curRota, gotoRota, .1f);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
            curRota+offset,
            transform.localEulerAngles.z);

        int _change = 0;

        if (Input.GetKeyDown(KeyCode.E)) { _change = -1; }
        if (Input.GetKeyDown(KeyCode.Q)) { _change = 1; }
        switch (_change)
        {
            case 1:
                gotoRota += 90;
                rota += 1; if (rota > 3) { rota = 0; }
                break;
            case -1:
                gotoRota -= 90;
                rota -= 1; if (rota < 0) { rota = 3; }
                break;

        }
    }
}
