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
        GetInput();
    }

    void GetInput()
    {
        if (!canDetectInput) { return; }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SetWalkPosition(playerTR.position + Vector3.forward);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetWalkPosition(playerTR.position + Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetWalkPosition(playerTR.position + Vector3.back);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetWalkPosition(playerTR.position + Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            //TODO: Find swappable block
            //SetSwapPosition();
        }
    }

    void SetWalkPosition(Vector3 _walkPosition)
    {
        //If walkposition = empty ->
        if (emptyPosition.Contains(RoundedPosition(_walkPosition)))
        {
            playerWalkPosition = _walkPosition;
            playerActionType = PlayerActionType.Walk;
            StartCoroutine(DoAllActions());
        }
    }

    void SetSwapPosition()
    {
        //Set swappable block
        //Set swap position if swappable block is true
    }

    IEnumerator DoAllActions()
    {
        canDetectInput = false;
        playerTR.position = playerWalkPosition;
        //Do player action: walk, swap, error
        //wait for player animation to finish
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
}
