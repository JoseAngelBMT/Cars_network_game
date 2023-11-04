using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyRock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroyObject());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
