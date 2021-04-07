using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFabricator : MonoBehaviour
{
    

    static int xLength = 6;
    static int yLength = 4;
    int boardDots = xLength*yLength;
    public GameObject prefabDot = null;
    
    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("Number of dots along x-axis: " + xLength);
        Debug.Log("Number of dots along x-axis: " + yLength);
        Debug.Log("Number of dots on the board: " + boardDots);

        // Generates the points along the x-axis (top/bottom)
        for(int i = 0; i<xLength; i++){
            
            Vector3 positionVectorBottom = generateXAxisPointsAtRandom(-4.0f);
            Instantiate(prefabDot, positionVectorBottom, Quaternion.identity);

            Vector3 positionVectorTop = generateXAxisPointsAtRandom(4.0f);
            Instantiate(prefabDot, positionVectorTop, Quaternion.identity);
            
        }

        // Generates the points along the y-axis (left/right)
        for(int i = 0; i<yLength; i++){
            
            Vector3 positionVectorLeft = generateYAxisPointsAtRandom(-6.0f);
            Instantiate(prefabDot, positionVectorLeft, Quaternion.identity);

            Vector3 positionVectorRight = generateYAxisPointsAtRandom(6.0f);
            Instantiate(prefabDot, positionVectorRight, Quaternion.identity);

        }


        // Generates the points on the board
        while(boardDots > 0)
        {
            Vector3 positionVector = generateBoardPointsAtRandom();
            Instantiate(prefabDot, positionVector, Quaternion.identity);
            boardDots--;
        }  
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    private Vector3 generateXAxisPointsAtRandom(float yPosition)
    {
        Vector3 result;
        
        result.x = Random.Range(-6.0f, 6.0f);
        result.y = yPosition;
        result.z = 0.0f;
        
        return result;
    }

    private Vector3 generateYAxisPointsAtRandom(float xPosition)
    {
        Vector3 result;
        
        result.x = xPosition;
        result.y = Random.Range(-4.0f, 4.0f);
        result.z = 0.0f;
        
        return result;
    }
    
    private Vector3 generateBoardPointsAtRandom()
    {
        Vector3 result;
        
        result.x = Random.Range(-6.0f, 6.0f);
        result.y = Random.Range(-4.0f, 4.0f);
        result.z = 0.0f;
        
        return result;
    }
}
        