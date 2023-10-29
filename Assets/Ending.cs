using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    [SerializeField] Image FadeOutter;
    float alpha = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        alpha = Mathf.MoveTowards(alpha, -40, Time.deltaTime * 10f);

        Color col2 = Color.black;
        col2.a = alpha;
        FadeOutter.color = col2;

        if (alpha == -40)
        {
            Application.Quit();
        }

    }
}
