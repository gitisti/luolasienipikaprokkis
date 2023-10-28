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

    enum PlayerActionType {
        Walk,
        Swap,
        Error
    }

    #region PlayerStuff
    [SerializeField] bool canDetectInput = true;
    [SerializeField] Transform playerTR;
    [SerializeField] Vector3 playerWalkPosition;
    [SerializeField] PlayerActionType playerActionType;
    #endregion

    [SerializeField] LayerMask layerMask;
    [SerializeField] List<Vector3> emptyPosition;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask lapsiMask;


    [SerializeField] int LapsiAmount = 0;


    GameObject eatThisChild = null;
    // Start is called before the first frame update
    void Start()
    {
        LoadLevel();
    }

    void LoadLevel()
    {
        //Instantiate LevelPrefab
        SetEssentials();
        FindEmptyPositions();
    }

    void SetEssentials()
    {
        playerTR = GameObject.FindGameObjectWithTag("Player").transform;
        LapsiAmount = GameObject.FindGameObjectsWithTag("Lapsi").Length;
        playerTR.position = RoundedPosition(playerTR.position);
    }

    void FindEmptyPositions()
    {
        emptyPosition.Clear();
        GameObject[] tile = GameObject.FindGameObjectsWithTag("Tile");

        for (int i = 0; i < tile.Length; i++)
        {
            //Raycast to check if tile is empty
            //Add to list if true

            RaycastHit hit;
            Vector3 raycastPos = tile[i].transform.position + Vector3.up;
            bool didHit = Physics.Raycast(raycastPos, Vector3.down, out hit, Mathf.Infinity, layerMask);

            if (didHit && hit.transform.CompareTag("Tile"))
            {
                emptyPosition.Add(RoundedPosition(hit.transform.position));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {


        var ray = playerTR.TransformDirection(Vector3.forward);
        RaycastHit hit;
        var pos = RoundedPosition(playerTR.position);


        Debug.DrawRay(pos, ray * 50, Color.green);

        GetInput();
    }

    void GetInput()
    {
        if (!canDetectInput) { return; }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SetWalkPosition(playerTR.position + Vector3.forward, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetWalkPosition(playerTR.position + Vector3.left, 270f);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetWalkPosition(playerTR.position + Vector3.back, 180f);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetWalkPosition(playerTR.position + Vector3.right, 90f);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {

            var ray = playerTR.TransformDirection(Vector3.forward) * 50f;
            RaycastHit hit;
            var pos = RoundedPosition(playerTR.position);


            Debug.DrawRay(pos, ray, Color.green);



            if (Physics.Raycast(pos, ray, out hit,50f, obstacleMask))
            {
                    SetSwapPosition(hit.transform);
            }



        }
    }


    void CheckIfAChildCanBeEaten()
    {
       
            var ray = playerTR.TransformDirection(Vector3.forward);
            RaycastHit hit;
            var pos = RoundedPosition(playerTR.position);


            Debug.DrawRay(pos, ray, Color.green);

        eatThisChild = null;

            if (Physics.Raycast(pos, ray, out hit, 1f, lapsiMask))
            {

            eatThisChild = hit.collider.gameObject;

            }
        
    }

    void SetWalkPosition(Vector3 _walkPosition, float angle = 0f)
    {
        //If walkposition = empty ->

        playerTR.eulerAngles = new Vector3(0f, angle, 0f);
       

        if (emptyPosition.Contains(RoundedPosition(_walkPosition)))
        {
            CheckIfAChildCanBeEaten();
            playerWalkPosition = _walkPosition;
            playerActionType = PlayerActionType.Walk;
            StartCoroutine(DoAllActions());
        }
    }

    void SetSwapPosition(Transform _tr)
    {

        Debug.Log(RoundedPosition(_tr.position));

        emptyPosition.Add(RoundedPosition(_tr.position));

        Vector3 oldPos = playerTR.position;
        playerTR.position = _tr.position;
        _tr.position = oldPos;



        emptyPosition.Remove(RoundedPosition(_tr.position));
        Debug.Log(RoundedPosition(_tr.position));
        //Set swappable block
        //Set swap position if swappable block is true
    }

    IEnumerator DoAllActions()
    {
        canDetectInput = false;
        playerTR.position = playerWalkPosition;
        //Do player action: walk, swap, error
        //wait for player animation to finish

        //EAT THE CHILD
        EatTheChild();

        //Do enemy actions if player action is walk or swap
        //wait for enemy animations to finish
        //Game over if boat osu pelaajaan
        canDetectInput = true;
        yield return null;
    }

    Vector3 RoundedPosition(Vector3 _position)
    {
        Vector3 roundedPosition = new Vector3(Mathf.Round(_position.x), 0, Mathf.Round(_position.z));
        return roundedPosition;
    }

    void EatTheChild()
    {
       if (eatThisChild != null)
        {
            LapsiAmount -= 1;
            Destroy(eatThisChild);
        }
    }

    }

