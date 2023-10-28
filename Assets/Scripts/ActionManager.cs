using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    enum PlayerActionType
    {
        Walk,
        Swap,
        Error
    }

    #region PlayerStuff
    [SerializeField] bool canDetectInput = true;
    [SerializeField] Transform playerTR;
    [SerializeField] WolfMove wolfmove;
    [SerializeField] Vector3 playerWalkPosition;
    [SerializeField] PlayerActionType playerActionType;
    #endregion

    [SerializeField] LayerMask layerMask;
    [SerializeField] List<Vector3> emptyPosition;
    [SerializeField] LayerMask swappableMask;
    [SerializeField] LayerMask lapsiMask;
    [SerializeField] LayerMask enemyMask;

    [SerializeField] GameObject WolfVisualObj;


    [SerializeField] Swapper swapper;

    [SerializeField] int LapsiAmount = 0;

    [SerializeField] List<EnemyWander> enemyWanderers;

    GameObject eatThisChild = null;
    bool GameOver = false;
    bool Win = false;

    bool waitForSwap = false;
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

        var player = GameObject.FindGameObjectWithTag("Player");
        playerTR = player.transform;

        //hide the wolf from the player
        var oldwolf = player.GetComponentInChildren<WolfMove>().gameObject;
        Vector3 pos = oldwolf.transform.position;
        oldwolf.SetActive(false);

        wolfmove = Instantiate(WolfVisualObj).GetComponentInChildren<WolfMove>();
        wolfmove.SetPlace(pos);
        wolfmove.SetGotoPlace(wolfmove.transform.position);

        LapsiAmount = GameObject.FindGameObjectsWithTag("Lapsi").Length;
        playerTR.position = RoundedPosition(playerTR.position);

        enemyWanderers.AddRange(FindObjectsOfType<EnemyWander>());

        swapper = GetComponent<Swapper>();

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


        if (waitForSwap)
        {
            if (swapper.isDone)
            {
                swapper.isDone = false;
                waitForSwap = false;
                playerWalkPosition = playerTR.position;
                StartCoroutine(DoAllActions());
            }
}

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

            waitForSwap = true;

            canDetectInput = false;

            if (Physics.Raycast(pos, ray, out hit, 50f, swappableMask))
            {
                //SetSwapPosition(hit.transform);

                bool isObstacle = (hit.collider.gameObject.GetComponent<EnemyWander>() == null);
                if (isObstacle)
                {
                    emptyPosition.Remove(RoundedPosition(playerTR.position));
                    emptyPosition.Add(RoundedPosition(hit.transform.position));
                }
                swapper.ACTIVATE(playerTR, hit.transform, wolfmove);

            }



        }
    }



    void CheckIfAChildCanBeEaten()
    {

        var ray = playerTR.TransformDirection(Vector3.forward);
        RaycastHit hit;
        var pos = RoundedPosition(playerTR.position);


        // Debug.DrawRay(pos, ray, Color.green);

        eatThisChild = null;

        if (Physics.Raycast(pos, ray, out hit, 1f, lapsiMask))
        {

            eatThisChild = hit.collider.gameObject;

        }

    }

    bool CheckIfIsGameOver()
    {
        var ray = playerTR.TransformDirection(Vector3.forward);
        RaycastHit hit;
        var pos = RoundedPosition(playerTR.position);

        return Physics.Raycast(pos, ray, out hit, 1f, enemyMask);
    }

        void SetWalkPosition(Vector3 _walkPosition, float angle = 0f)
    {
        //If walkposition = empty ->

        playerTR.eulerAngles = new Vector3(0f, angle, 0f);
        wolfmove.SetGotoRota(playerTR.eulerAngles + new Vector3(0, 90f, 0));


        if (emptyPosition.Contains(RoundedPosition(_walkPosition)))
        {
            if (CheckIfIsGameOver())
            {
                GameOver = true;
            }
            else
            {

                CheckIfAChildCanBeEaten();
            }
            
            playerWalkPosition = _walkPosition;
            playerActionType = PlayerActionType.Walk;
            StartCoroutine(DoAllActions());
        }
    }

    void SetSwapPosition(Transform _tr)
    {


        canDetectInput = false;

        //here we check if the object we're changing places with isn't an enemy
        //by checking if they have the enemywander script or not
        bool isEnemy = (_tr.gameObject.GetComponent<EnemyWander>() != null);

        if (!isEnemy)
        {
            emptyPosition.Add(RoundedPosition(_tr.position));
        }
        
        Vector3 oldPos = playerTR.position;
        playerTR.position = _tr.position;
        _tr.position = oldPos;
        
        if (isEnemy)
        {

            var obj = _tr.GetComponentInChildren<EnemyWander>();
            obj.SetGotoPlace(oldPos);
            obj.SetPlace(oldPos);
        }
      

        //move the wolf visual to this place)
        wolfmove.SetPlace(RoundedPosition(playerTR.position));
        wolfmove.SetGotoPlace(RoundedPosition(playerTR.position));

        if (!isEnemy)
        {
            emptyPosition.Remove(RoundedPosition(_tr.position));
        }
    }

    IEnumerator DoAllActions()
    {
        canDetectInput = false;
        playerTR.position = playerWalkPosition;
        wolfmove.SetGotoPlace(playerTR.position);
        //Do player action: walk, swap, error
        //wait for player animation to finish
        if (!GameOver)
        {
            //EAT THE CHILD
            EatTheChild();

            yield return new WaitForSeconds(.1f);
            //Do enemy actions if player action is walk or swap
            if (!Win)
            {
                ActivateEnemyWanderers();
            }
        }
        //wait for enemy animations to finish
        //Game over if boat osu pelaajaan
        if (!GameOver && !Win){ 
        canDetectInput = true;
        }
        else if (GameOver)
        { 
            GameOverEvent();
        }
        else
        {
            WinEvent();
        }
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
            Win = (LapsiAmount <= 0);
        }
    }

    void GameOverEvent() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void WinEvent() { }

    void ActivateEnemyWanderers()
    {


        foreach (var e in enemyWanderers)
        {

            if (e.transform.position.x == playerTR.position.x && e.transform.position.z == playerTR.position.z)
            {
                GameOver = true;
                break;
            }

            float rota = 0;
            var enemy = e.gameObject;


            RaycastHit hit;
            var MoveVector = Vector3.zero;
            var transpos = e.GetGotoPlace();

            var _f = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.forward))));

            var _b = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.back))));

            var _r = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.right))));

            var _l = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.left))));


            if (_f)
            {
                if (_r) {
                    rota = 90f; ;
                    MoveVector = enemy.transform.TransformDirection(Vector3.right); }
                else
                {

                    rota = 0f;
                    MoveVector = enemy.transform.TransformDirection(Vector3.forward);
                }

            }
            else if (_l) { MoveVector = enemy.transform.TransformDirection(Vector3.left); rota = -90f; }
            else if (_b) { MoveVector = enemy.transform.TransformDirection(Vector3.back); rota = 180f; }



            if (MoveVector != Vector3.zero)
            {

                
               enemy.transform.Rotate(new Vector3(0, rota, 0));

                e.SetGotoPlace(RoundedPosition(e.GetGotoPlace() + MoveVector));
                e.SetGotoRota(enemy.transform.eulerAngles);
                playerTR.position = RoundedPosition(playerTR.position);

                if (e.GetGotoPlace().x==playerTR.position.x && e.GetGotoPlace().z == playerTR.position.z)    
                {
                    GameOver = true;
                    break;
                }
            }


        }

    }

}


