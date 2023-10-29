using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swapper : MonoBehaviour
{
    [SerializeField] GameObject TPVF;

    GameObject TP1 = null;
    GameObject TP2 = null;


    Vector3 StartPos;
    Vector3 EndPos;

    Transform StartObj;
    Transform EndObj;


    Vector3 startScale;
    Vector3 endScale;
    Vector3 wolfScale;

    Vector3 startRota;
    Vector3 endRota;

    

    int STATE = 0;
    float anima = 0f;

    float animaSpeed = 3f;

    WolfMove wolf = null;

    bool ACTIVE = false;
    GameObject obj;
    EnemyWander isEnemy = null;

    Vector3 GroundVector = new Vector3(0, -2, 0);
    public bool isDone = false;

    public HandHandler hand;
    bool handIsDone = false;
    void Start()
    {
       
    }


    public void ACTIVATE(Transform _start, Transform _end,WolfMove _wolmove = null,GameObject _obj=null)
    {

        wolf = _wolmove;
        StartObj = _start.transform;
        EndObj = _end.transform;
        obj = _obj;

        StartPos = RoundedPosition(StartObj.position);
        EndPos = RoundedPosition(EndObj.position);

        StartPos.y = 0;
        EndPos.y = 0;



        startScale = StartObj.localScale;
        endScale = EndObj.localScale;

        startRota = StartObj.eulerAngles;
        endRota = EndObj.eulerAngles;



        GameObject TP1 = Instantiate(TPVF);
        GameObject TP2 = Instantiate(TPVF);
        TP1.transform.position = StartPos;
        TP2.transform.position = EndPos;


        if (_obj != null)
        {
            hand = _obj.GetComponent<HandHandler>();
        }


        isEnemy = _end.GetComponent<EnemyWander>();
        if (isEnemy != null)
        {
            isEnemy.setLERPING(false);
        }

        if (wolf != null)
        {
            wolfScale = wolf.transform.localScale;
            wolf.SetLERP(false);
        }

        ACTIVE = true;




    }

    // Update is called once per frame
    void Update()
    {
        if (!ACTIVE) { return; }

        switch (STATE)
        {

            case 0:

                if (!hand.handDone) { return; }

                anima = Mathf.MoveTowards(anima, 1, animaSpeed * Time.deltaTime);

                //StartObj.transform.localScale = startScale * (1f - anima);
                //EndObj.transform.localScale = endScale * (1f - anima);

                if (wolf != null)
                {
                    wolf.bonusPosition = GroundVector * (anima);
                }


               // StartObj.position = StartPos;
                EndObj.position = EndPos + GroundVector * (anima);

                //SWAP PLACES
                if (anima == 1) { STATE = 1;

                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                   


                    if (wolf != null)
                    {
                        wolf.SetGotoPlace(EndPos);
                        wolf.SetPlace(EndPos+GroundVector);
                    }

                }
                break;
            case 1:


                anima = Mathf.MoveTowards(anima, 0, animaSpeed * Time.deltaTime);


               // 
                EndObj.position = StartPos + GroundVector * (anima);

                //StartObj.transform.localScale = startScale * (1f - anima);
                //EndObj.transform.localScale = endScale * (1f - anima);



                if (wolf != null)
                {
                   wolf.bonusPosition = GroundVector * (anima);
                }

                if (anima == 0)
                {
                    if (wolf != null)
                    {

                        wolf.SetLERP(true);
                    }

                    if (isEnemy != null)
                    {
                        isEnemy.SetGotoPlace(StartPos);
                        isEnemy.setLERPING(true);
                    }

                    StartObj.position = EndPos;
                    wolf.bonusPosition = Vector3.zero;
                    StartObj.eulerAngles = startRota;
                    EndObj.eulerAngles = endRota;

                    STATE = 0;
                    ACTIVE = false;

                    isDone = true;

                    Destroy(TP1);
                    Destroy(TP2);
                }
                break;


        }
    }


    Vector3 RoundedPosition(Vector3 _position)
    {
        Vector3 roundedPosition = new Vector3(Mathf.Round(_position.x), 0, Mathf.Round(_position.z));
        return roundedPosition;
    }
}
