using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{


    [SerializeField] Image startButton;
    [SerializeField] Image quitButton;

    [SerializeField] RawImage selector;

    [SerializeField] GameObject pos1;
    [SerializeField] GameObject pos2;

    [SerializeField] Color colorUnselected;


    [SerializeField] GameObject text;
    [SerializeField] Image fadeOutter;

    [SerializeField] AudioClip MOVE;
    [SerializeField] AudioClip START;

    [SerializeField] AudioSource au;

    int PHASE = 0;

    int selected = 0;

    float alpha = 1;




    delegate void DELE();

    IEnumerator DoWithDelayEnu(float delay, DELE funkt)
    {
        yield return new WaitForSecondsRealtime(delay);
        funkt();
    }

    void DoWithDelay(float delay, DELE funkt)
    {
        StartCoroutine(DoWithDelayEnu(delay, funkt));
    }




    // Start is called before the first frame update
    void Start()
    {
        au = GetComponent<AudioSource>();
        startButton.color = Color.white;
        quitButton.color = colorUnselected;

        selector.transform.position = pos1.transform.position;
        fadeOutter.color = Color.black;

    }

    // Update is called once per frame
    void Update()
    {
        switch (PHASE)
        {
            case 0:



                alpha = Mathf.MoveTowards(alpha, 0, Time.deltaTime * 10f);

                Color col = Color.black;
                col.a = alpha;
                fadeOutter.color = col;

                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {

                    au.PlayOneShot(MOVE);

                    switch (selected)
                    {
                        case 0:

                            startButton.color = colorUnselected;
                            quitButton.color = Color.white;
                            selector.transform.position = pos2.transform.position;
                            selected = 1;
                            break;

                        case 1:

                            startButton.color = Color.white;
                            quitButton.color = colorUnselected;

                            selector.transform.position = pos1.transform.position;

                            selected = 0;
                            break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    au.PlayOneShot(START);
                    PHASE = 1;
                }
        
        break;

            case 1:

                alpha = Mathf.MoveTowards(alpha, 1, Time.deltaTime * 10f);

                Color col2 = Color.black;
                col2.a = alpha;
                fadeOutter.color = col2;

                if (alpha == 1)
                {
                    switch (selected)
                    {
                        case 0:

                            DoWithDelay(1f, () =>
                            {
                                text.SetActive(true);
                            });


                            DoWithDelay(5f, () =>
                            {
                                text.SetActive(false);
                            });

                            DoWithDelay(7f, () =>
                            {
                                SceneManager.LoadScene(1);
                            });


                            break;
                        case 1:
                            Application.Quit();
                            break;
                    }
                }
                break;

    }
    }

}
