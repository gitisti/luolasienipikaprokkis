using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{


  [SerializeField] Image fadeOuter = null;
    float spd = 2f;
    float phase = 0;

    float alpha = 1.5f;

    bool isDone = false;

    // Start is called before the first frame update
    void Start()
    {
        Color col = Color.black;
        col.a = 1f;
        fadeOuter.color = col;
    }

    public void SetPhase(float _f) { isDone = false; phase = _f; }

    // Update is called once per frame
    void Update()
    {
         if (fadeOuter == null) { return; }
        alpha = Mathf.MoveTowards(alpha, phase, Time.deltaTime * spd);

        Color col = Color.black;
        col.a = alpha;
        fadeOuter.color = col;

        if (alpha == phase) { isDone = true; }
        
    }
}
