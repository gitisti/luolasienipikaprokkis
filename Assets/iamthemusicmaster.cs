using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iamthemusicmaster : MonoBehaviour
{

    static iamthemusicmaster instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null) { Destroy(gameObject); } else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
