using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFabricator : MonoBehaviour
{
    
    int numberOfDots = 20;
    public GameObject prefabDot = null;
    
    // Start is called before the first frame update
    void Start()
    {
        while(numberOfDots > 0)
        {
            Debug.Log("Made dot");
        Vector3 positionVector = generateRandomPosition();
        Instantiate(prefabDot, positionVector, Quaternion.identity);
        numberOfDots--;
        }  
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    private Vector3 generateRandomPosition()
    {
        Vector3 result;
        
        result.x = Random.Range(-6.0f, 6.0f);
        result.y = Random.Range(-4.0f, 4.0f);
        result.z = 0.0f;
        
        return result;
    }
}