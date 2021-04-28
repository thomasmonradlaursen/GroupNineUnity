using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticesGenerator : MonoBehaviour
{
    
    static int length = 8;
    static int heigth = 3;
    public static int verticalVertices = 17;
    public static int horizontalVertices = 3;
    Vector3[] vertices = new Vector3[verticalVertices*horizontalVertices];
    public GameObject prefabDot = null;


    void Start()
    {
        insertVertices();
        logVertices(); 

        for(int i = 0; i<vertices.Length; i++)
        {
            Instantiate(prefabDot, vertices[i], Quaternion.identity);
        }  
    }

    private void insertVertices()
    {
        float xInterval = (float) length/((float) verticalVertices-2);
        float yInterval = (float) heigth/((float) horizontalVertices-2);
        Debug.Log(xInterval);
        Debug.Log(yInterval);
        Vector2 lowerBound = new Vector2(0,0);
        Vector2 upperBound = new Vector2(xInterval, yInterval);

        for(int i = 0; i < verticalVertices * horizontalVertices; i++){
            
            if(i%verticalVertices == 0 && i!=0)
            {
                lowerBound.x = 0;
                upperBound.x = xInterval;
                if(i > verticalVertices) {
                    lowerBound.y = upperBound.y;
                    upperBound.y = lowerBound.y + yInterval;
                }
            }
            
            vertices[i].x = generateChaoticFloat(lowerBound.x, upperBound.x);
            vertices[i].y = generateChaoticFloat(upperBound.y, lowerBound.y);

            if(i < verticalVertices) vertices[i].y = 0;
            if(i >= vertices.Length - verticalVertices) vertices[i].y = heigth;
            if(i%verticalVertices == verticalVertices-1) vertices[i].x = length;
            if(i%verticalVertices == 0) vertices[i].x = 0;

            if(i>0 && i%verticalVertices != 0)
            {
                lowerBound.x = upperBound.x;
                upperBound.x = lowerBound.x + xInterval;
            }
        }
    }

    private float generateChaoticFloat(float lowerBound, float upperBound)
    {
        float resultingCoordinates;
        resultingCoordinates = Random.Range(lowerBound, upperBound);
        return resultingCoordinates;
    }

    private void logVertices()
    {
        int counter = 1;
        foreach(Vector3 vertex in vertices)
        {
            Debug.Log("Vertex " + counter + ": " + vertex);
            counter++;
        }
    }
}
