using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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



    delegate void DELE();

    IEnumerator DoWithDelayEnu(float delay,DELE funkt)
    {
        yield return new WaitForSecondsRealtime(delay);
        funkt();
    }

    void DoWithDelay(float delay, DELE funkt)
    {
        StartCoroutine(DoWithDelayEnu(delay, funkt));
    }


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
    [SerializeField] GameObject WolfVisualObjMine = null;



    //lapset laittaa ittensä ja positionsa listalle, sit sielt etitää
    [SerializeField] List<Vector3> lapsiPos;
    [SerializeField] List<GameObject> lapset;



    [SerializeField] Swapper swapper;

    [SerializeField] int LapsiAmount = 0;

    [SerializeField] List<EnemyWander> enemyWanderers;


    [SerializeField] GameObject handoObj;
    [SerializeField] GameObject curHand = null;

    [SerializeField] Image YEETED;
    [SerializeField] Image CHILDED;

    GameObject eatThisChild = null;
    bool GameOver = false;
    bool Win = false;

    bool waitForSwap = false;
    bool noSwap = false;

    bool itWasChild = false;


    CameraRota camrot = null;

    FadeOut fadeout;

    GameObject gameOverBoat = null;

    [SerializeField] AudioClip SUSI;
    [SerializeField] AudioClip MOVE;
    [SerializeField] AudioClip SUSIOUT;
    [SerializeField] List<AudioClip> ONO;
    [SerializeField] AudioClip BOAT;
    [SerializeField] AudioClip TELE;
    [SerializeField] AudioClip ARM;
    [SerializeField] AudioClip GAMEO;
    [SerializeField] AudioClip ROTATE;
    [SerializeField] AudioClip WIN;
    [SerializeField] List<AudioClip> LAUGH;
    [SerializeField] AudioSource au;

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
        au = GetComponent<AudioSource>();
        var player = GameObject.FindGameObjectWithTag("Player");
        playerTR = player.transform;

        fadeout = FindObjectOfType<FadeOut>();
        //hide the wolf from the player
        var oldwolf = player.GetComponentInChildren<WolfMove>().gameObject;
        Vector3 pos = oldwolf.transform.position;
        oldwolf.SetActive(false);

        WolfVisualObjMine = Instantiate(WolfVisualObj).GetComponentInChildren<BounceAnimation>().gameObject;
        wolfmove = WolfVisualObjMine.GetComponentInChildren<WolfMove>();
        wolfmove.SetPlace(pos);
        wolfmove.SetGotoPlace(wolfmove.transform.position);
        camrot = FindObjectOfType<CameraRota>();
        LapsiAmount = GameObject.FindGameObjectsWithTag("Lapsi").Length;
        playerTR.position = RoundedPosition(playerTR.position);

        enemyWanderers.AddRange(FindObjectsOfType<EnemyWander>());

        swapper = GetComponent<Swapper>();

        wolfmove.SetGotoRota(playerTR.eulerAngles + new Vector3(0, 90f, 0));
        wolfmove.SetRota(playerTR.eulerAngles + new Vector3(0, 90f, 0));

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


        GameObject[] lapsetarra = GameObject.FindGameObjectsWithTag("Lapsi");
        for (int i = 0; i < lapsetarra.Length; i++)
        {
            lapset.Add(lapsetarra[i]);
            lapsiPos.Add(RoundedPosition(lapsetarra[i].transform.position));
        }

        }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        
        if (Input.GetKeyDown(KeyCode.E)) { au.PlayOneShot(ROTATE); }
        if (Input.GetKeyDown(KeyCode.Q)) { au.PlayOneShot(ROTATE); }


        if (waitForSwap)
        {
            if (noSwap && curHand != null && curHand.GetComponent<HandHandler>().handDone)
            {
                Destroy(curHand.gameObject);
                swapper.isDone = false;
                waitForSwap = false;
                playerWalkPosition = playerTR.position;
               // StartCoroutine(DoAllActions());

                canDetectInput = true;
                noSwap = false;
                SetBoatArrows();
                return;
            }

            if (swapper.isDone)
            {
                swapper.isDone = false;
                waitForSwap = false;
                playerWalkPosition = playerTR.position;

                canDetectInput = true;
                SetBoatArrows();
                //StartCoroutine(DoAllActions());
            }
        }
        
            if (GameOver && gameOverBoat != null)
            {
            gameOverBoat.transform.Translate(Vector3.forward * Time.deltaTime * 10f
                );
            if (!itWasChild) { 
                wolfmove.transform.position = gameOverBoat.transform.position+new Vector3(0,1,0);
            }
        }



        

    }

    void GetInput()
    {
        

        if (!canDetectInput) { return; }

        var _destVect = Vector3.zero;
        var _angler = 0f;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (camrot.rota)
            {
                case 0: _destVect = Vector3.forward; _angler = 0f; break;
                case 3: _destVect = Vector3.left; _angler = 270f; break;
                case 2: _destVect = Vector3.back; _angler = 180f; break;
                case 1: _destVect = Vector3.right; _angler = 90f; break;
            }
            SetWalkPosition(playerTR.position + _destVect, _angler);
        }
        else if (Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow))
        {

            switch (camrot.rota)
            {
                case 1: _destVect = Vector3.forward; _angler = 0f; break;
                case 0: _destVect = Vector3.left; _angler = 270f; break;
                case 3: _destVect = Vector3.back; _angler = 180f; break;
                case 2: _destVect = Vector3.right; _angler = 90f; break;
            }

            SetWalkPosition(playerTR.position + _destVect, _angler);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {

            switch (camrot.rota)
            {
                case 2: _destVect = Vector3.forward; _angler = 0f; break;
                case 1: _destVect = Vector3.left; _angler = 270f; break;
                case 0: _destVect = Vector3.back; _angler = 180f; break;
                case 3: _destVect = Vector3.right; _angler = 90f; break;
            }


            SetWalkPosition(playerTR.position + _destVect, _angler);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            switch (camrot.rota)
            {
                case 3: _destVect = Vector3.forward; _angler = 0f; break;
                case 2: _destVect = Vector3.left; _angler = 270f; break;
                case 1: _destVect = Vector3.back; _angler = 180f; break;
                case 0: _destVect = Vector3.right; _angler = 90f; break;
            }

            SetWalkPosition(playerTR.position + _destVect, _angler);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {


            au.PlayOneShot(ARM);

            var hand = Instantiate(handoObj, wolfmove.transform);


            curHand = hand;
            int _len = 0;
            var _pos = RoundedPosition(playerTR.position);
            

           // hand.transform.position = WolfVisualObjMine.transform.position;
            //hand.transform.localEulerAngles = wolfmove.transform.localEulerAngles - new Vector3(0,180f,0);

            var ray = playerTR.TransformDirection(Vector3.forward) * 50f;
            var _ray1 = playerTR.TransformDirection(Vector3.forward);
            RaycastHit hit;
            var pos = RoundedPosition(playerTR.position);

            waitForSwap = true;

            canDetectInput = false;

            if (Physics.Raycast(pos, ray, out hit, 50f, swappableMask))
            {

                hand.GetComponent<HandHandler>().SetGotoPos(Vector3.Distance(pos,hit.transform.position)*1.25f+1);
            
                


                //SetSwapPosition(hit.transform);

                bool isObstacle = (hit.collider.gameObject.GetComponent<EnemyWander>() == null);
                if (isObstacle)
                {
                    emptyPosition.Remove(RoundedPosition(playerTR.position));
                    emptyPosition.Add(RoundedPosition(hit.transform.position));
                }
                au.PlayOneShot(TELE);

                swapper.ACTIVATE(playerTR, hit.transform, wolfmove,hand);

            }
            else
            {
                noSwap = true;
                
                hand.GetComponent<HandHandler>().SetGotoPos(8);
                hand.GetComponent<HandHandler>().GrabSomething=false;
            }



        }
    }



    void CheckIfAChildCanBeEaten()
    {

        var ray = playerTR.TransformDirection(Vector3.forward);
        RaycastHit hit;
        var pos = RoundedPosition(playerTR.position);



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

        bool aa = false;

        if (emptyPosition.Contains(RoundedPosition(_walkPosition)))
        {
            if (CheckIfIsGameOver())
            {
                GameOver = true;
            }
            else
            {
                aa = true;
            }


            playerTR.eulerAngles = new Vector3(0f, angle, 0f);
            wolfmove.SetGotoRota(playerTR.eulerAngles + new Vector3(0, 90f, 0));

            if (aa)
            {

                CheckIfAChildCanBeEaten();
            }

            au.PlayOneShot(MOVE);
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
                SetBoatArrows();
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

            au.PlayOneShot(SUSI);
            DoWithDelay(.25f, () =>
            {

                au.PlayOneShot(LAUGH[Random.Range(0, LAUGH.Count)]);
            });

            LapsiAmount -= 1;
            Destroy(eatThisChild);
            Win = ((LapsiAmount <= 0) && !GameOver);
        }
    }

    void GameOverEvent() {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        au.PlayOneShot(BOAT);

        DoWithDelay(1f, () =>
         {
             au.PlayOneShot(GAMEO);
         });

        DoWithDelay(1.5f, () =>
        {
            fadeout.SetPhase(1);
        

            if (itWasChild)
            {

                fadeout.setGameOverImage(CHILDED);
            }
            else
            {

                fadeout.setGameOverImage(YEETED);
            }
        });



        DoWithDelay(3.5f, () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });





    }

    void WinEvent() {


        au.PlayOneShot(WIN);
        DoWithDelay(1.5f, () =>
        {
            fadeout.SetPhase(1);
        });

        if (SceneManager.sceneCount > SceneManager.GetActiveScene().buildIndex+1)
        {

            DoWithDelay(3f, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            });

        }
        else
        {
            DoWithDelay(3f, () =>
            {
                SceneManager.LoadScene(0);
            });
        }


    }



    void SetBoatArrows()
    {
        foreach (var e in enemyWanderers)
        {

            var pospos = RoundedPosition(e.transform.position);

           

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
                if (_r && _l)
                {
                    var _l2 = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.back) + enemy.transform.TransformDirection(Vector3.left))));

                    var _r2 = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.back) + enemy.transform.TransformDirection(Vector3.right))));

                    if (!_l2 && !_r2)
                    {
                        Debug.Log("A");
                        rota = 90f; ;
                        MoveVector = enemy.transform.TransformDirection(Vector3.right);
                    }
                    if (_l2 && _r2)
                    {
                        Debug.Log("A2");
                        rota = 0f;
                        MoveVector = enemy.transform.TransformDirection(Vector3.forward);
                    }
                    else if (_l2)
                    {
                        Debug.Log("B");
                        MoveVector = enemy.transform.TransformDirection(Vector3.right); rota = 90f;
                    }
                    else if (_r2)
                    {
                        Debug.Log("C");
                        rota = 90f; ;
                        MoveVector = enemy.transform.TransformDirection(Vector3.left);
                    }
                    else
                    {
                        Debug.Log("D");

                        MoveVector = enemy.transform.TransformDirection(Vector3.right); rota = 90f;
                    }
                }
                else if (_r)
                {
                    Debug.Log("E");
                    rota = 90f; ;
                    MoveVector = enemy.transform.TransformDirection(Vector3.right);
                }
                else
                {

                    Debug.Log("F");
                    rota = 0f;
                    MoveVector = enemy.transform.TransformDirection(Vector3.forward);
                }

            }
            else if (_r)
            {
                Debug.Log("G2"); MoveVector = enemy.transform.TransformDirection(Vector3.right); rota = 90f;
            }
            else if (_l)
            {
                Debug.Log("G"); MoveVector = enemy.transform.TransformDirection(Vector3.left); rota = -90f;
            }
            else if (_b)
            {
                Debug.Log("H"); MoveVector = enemy.transform.TransformDirection(Vector3.back); rota = 180f;
            }




            e.SetArrowRota(rota);

            


        }
    }

    void ActivateEnemyWanderers()
    {


        foreach (var e in enemyWanderers)
        {

            if (e.transform.position.x == playerTR.position.x && e.transform.position.z == playerTR.position.z)
            {
                au.PlayOneShot(SUSIOUT);
                e.setLERPING(false);
                gameOverBoat = e.gameObject;
                wolfmove.SetLERP(false);
                GameOver = true;
                break;
            }

            float rota = 0;
            var enemy = e.gameObject;


            RaycastHit hit;
            var MoveVector = Vector3.zero;
            var transpos = e.GetGotoPlace();
            Debug.Log(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.forward)));
            var _f = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.forward))));

            var _b = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.back))));

            var _r = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.right))));

            var _l = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.left))));

            Debug.Log(_f + " " + _b + " " + _r + " " + _l);

            if (_f)
            {
                if (_r && _l)
                {
                    var _l2 = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.back)+ enemy.transform.TransformDirection(Vector3.left))));
                    
                    var _r2 = (emptyPosition.Contains(RoundedPosition(transpos + enemy.transform.TransformDirection(Vector3.back) + enemy.transform.TransformDirection(Vector3.right))));

                    if (!_l2 && !_r2)
                    {
                        Debug.Log("A");
                        rota = 90f; ;
                        MoveVector = enemy.transform.TransformDirection(Vector3.right);
                    }
                    if (_l2 && _r2)
                    {
                        Debug.Log("A2");
                        rota = 0f;
                        MoveVector = enemy.transform.TransformDirection(Vector3.forward);
                    }
                    else if (_l2)
                    {
                        Debug.Log("B");
                        MoveVector = enemy.transform.TransformDirection(Vector3.right); rota = 90f;
                    }
                    else if (_r2)
                    {
                        Debug.Log("C");
                        rota = 90f; ;
                        MoveVector = enemy.transform.TransformDirection(Vector3.left);
                    }
                    else
                    {
                        Debug.Log("D");

                        MoveVector = enemy.transform.TransformDirection(Vector3.right); rota = 90f;
                    }
                }
                else if (_r)
                {
                    Debug.Log("E");
                    rota = 90f; ;
                    MoveVector = enemy.transform.TransformDirection(Vector3.right); }
                else
                {

                    Debug.Log("F");
                    rota = 0f;
                    MoveVector = enemy.transform.TransformDirection(Vector3.forward);
                }

            }
            else if (_r)
            {
                Debug.Log("G2"); MoveVector = enemy.transform.TransformDirection(Vector3.right); rota = 90f;
            }
            else if (_l)
            {
                Debug.Log("G"); MoveVector = enemy.transform.TransformDirection(Vector3.left); rota = -90f; }
            else if (_b)
            {
                Debug.Log("H"); MoveVector = enemy.transform.TransformDirection(Vector3.back); rota = 180f; }



            if (MoveVector != Vector3.zero)
            {

                
               enemy.transform.Rotate(new Vector3(0, rota, 0));

                e.SetGotoPlace(RoundedPosition(e.GetGotoPlace() + MoveVector));
                e.SetGotoRota(enemy.transform.eulerAngles);
                playerTR.position = RoundedPosition(playerTR.position);

                if (e.GetGotoPlace().x==playerTR.position.x && e.GetGotoPlace().z == playerTR.position.z)
                {
                    au.PlayOneShot(SUSIOUT);
                    e.setLERPING(false);
                    wolfmove.SetLERP(false);
                    gameOverBoat = e.gameObject;
                    GameOver = true;
                    break;
                }

                var _p = e.GetGotoPlace();

                if (lapsiPos.Contains(_p))
                {
                    GameObject _lapsilapsi = null;
                    foreach(var a in lapset)
                    {
                        if (a != null) { 
                        if (RoundedPosition(a.transform.position) == _p){
                            au.PlayOneShot(ONO[Random.Range(0,ONO.Count)]);
                            itWasChild = true;
                            a.transform.parent = e.gameObject.transform;
                            a.transform.localPosition = Vector3.zero;
                            e.setLERPING(false);
                            gameOverBoat = e.gameObject;
                            GameOver = true;
                        }
                        }
                    }
                }

            }


        }

    }

}


