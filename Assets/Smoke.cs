using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    GameObject smoke;
    GameObject Light;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fireSmoke()
    {
        Instantiate(smoke, this.gameObject.transform.position, Quaternion.identity);
    }

    public void fireLight()
    {
        Instantiate(smoke, this.gameObject.transform.position, Quaternion.identity);
    }
}
