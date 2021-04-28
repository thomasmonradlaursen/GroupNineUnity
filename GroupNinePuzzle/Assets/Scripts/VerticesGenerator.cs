using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VerticesGenerator : MonoBehaviour
{
    
    public static int length = 3;
    public static int heigth = 4;
    public static int numberOfHorizontalVertices = 4;
    public static int numberOfVerticalVertices = 4;
    public Vector3[] vertices = new Vector3[16];
    public GameObject prefabDot = null;


    void Start()
    {
        insertVertices();
        logVertices(); 

        for(int i = 0; i<numberOfHorizontalVertices*numberOfVerticalVertices; i++)
        {
            Debug.Log(i);
            Instantiate(prefabDot, vertices[i], Quaternion.identity);
        }  
    }

    private void insertVertices()
    {
        float xInterval = (float) length/((float) numberOfHorizontalVertices-1);
        float yInterval = (float) heigth/((float) numberOfVerticalVertices-1);
        Debug.Log(xInterval);
        Debug.Log(yInterval);
        Vector2 lowerBound = new Vector2(0,0);
        Vector2 upperBound = new Vector2(xInterval, 0); // y should be 0 for first row and if we start by setting upperbound to 
                                                        // yInterval then the second row wil have upperbound 2*yInterval, which
                                                        // is not what we want

        for(int i = 0; i < numberOfHorizontalVertices * numberOfVerticalVertices; i++){
            
            if(i!=0){
                if(i%numberOfHorizontalVertices == 0) // if max index in row of vertices has been reached
                {
                    // reset horizontal bounds
                    lowerBound.x = 0;
                    upperBound.x = xInterval;

                    // reset vertical bounds
                    lowerBound.y = vertices[i-1].y;
                    upperBound.y += yInterval;
                }
                else // increase horizontally
                {
                    lowerBound.x = vertices[i-1].x;
                    upperBound.x += xInterval;
                }
            }
            
            // Generate x and y value of vertice
            vertices[i].x = generateChaoticFloat(lowerBound.x, upperBound.x);
            vertices[i].y = generateChaoticFloat(upperBound.y, lowerBound.y);

            // overwrite x and y values generated if vertice is on one of the sides of the game board
            if(i < numberOfHorizontalVertices){
                vertices[i].y = 0; // bottom
            } 
            if(i >= numberOfVerticalVertices * (numberOfHorizontalVertices-1))
            {
                vertices[i].y = heigth; // top
            }
            if(i%numberOfHorizontalVertices == numberOfHorizontalVertices-1) 
            {
                vertices[i].x = length; // right
            }
            if(i%numberOfHorizontalVertices == 0) 
            {
                vertices[i].x = 0; // left
            }

        }
    }

    private float generateChaoticFloat(float lowerBound, float upperBound)
    {
        return Random.Range(lowerBound, upperBound);
    }

    private void logVertices()
    {
        int counter = 1;
        for(int i=0; i< numberOfHorizontalVertices*numberOfVerticalVertices; i++)
        {
            Debug.Log("Vertex " + counter + ": " + vertices[i].x + "    " + vertices[i].y);
            counter++;
        }
    }
}
