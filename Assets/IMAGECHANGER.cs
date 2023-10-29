using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IMAGECHANGER : MonoBehaviour
{

    [SerializeField] List<Sprite> spritet;
    [SerializeField] Image ima;

    int cur = 0;



    IEnumerator aa()
    {
        yield return new WaitForSeconds(1);
        cur++;
        if (cur >= spritet.Count) { cur = 0; }
        ima.sprite = spritet[cur];
        StartCoroutine(aa());

    }


    // Start is called
    // before the first frame update
    void Start()
    {
        ima = GetComponent<Image>();
        StartCoroutine(aa());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
