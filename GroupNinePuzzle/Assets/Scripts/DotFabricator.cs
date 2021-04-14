using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFabricator : MonoBehaviour
{
    // Fields for setup of vertices
    static int xAxisLength = 3;
    static int yAxisLength = 2;
    static int numberOfVerticesAlongX = 6;
    static int numberOfVerticesAlongY = 4;
    public GameObject prefabDot = null;

    // Arrays for coordinates
    Vector3[,] coordinatesOfCenterVertices = new Vector3[numberOfVerticesAlongX,numberOfVerticesAlongY];
    Vector3[,] coordinatesOfXAxisVertices = new Vector3[numberOfVerticesAlongX,2];
    Vector3[,] coordinatesOfYAxisVertices = new Vector3[numberOfVerticesAlongY,2];
    
    // Start is called before the first frame update
    void Start()
    {

        initializeCornerVertices(coordinatesOfXAxisVertices, 'x', xAxisLength, yAxisLength);
        initializeCornerVertices(coordinatesOfYAxisVertices, 'y', yAxisLength, xAxisLength);

        Debug.Log("Number of dots along x-axis: " + numberOfVerticesAlongX);
        Debug.Log("Number of dots along x-axis: " + numberOfVerticesAlongY);

        initializeCoordinatesForAxisVertices(coordinatesOfXAxisVertices, 'x', xAxisLength, yAxisLength, numberOfVerticesAlongX);

        for(int counter = 0; counter<numberOfVerticesAlongX; counter++){
            Debug.Log("X-bottom " + (counter+1) + ":" + coordinatesOfXAxisVertices[counter,0].ToString());
            Debug.Log("X-top " + (counter+1) + ":" + coordinatesOfXAxisVertices[counter,1].ToString());
        }

        initializeCoordinatesForAxisVertices(coordinatesOfYAxisVertices, 'y', yAxisLength, xAxisLength, numberOfVerticesAlongY);
        for(int counter = 0; counter<numberOfVerticesAlongY; counter++){
            Debug.Log("Y-bottom " + (counter+1) + ":" + coordinatesOfYAxisVertices[counter,0].ToString());
            Debug.Log("Y-top " + (counter+1) + ":" + coordinatesOfYAxisVertices[counter,1].ToString());
        }

        // Render vertices for X
        for(int outer = 0; outer<2; outer++){
                for(int inner = 0; inner<numberOfVerticesAlongX; inner++){
                Instantiate(prefabDot, coordinatesOfXAxisVertices[inner,outer], Quaternion.identity);
            }
        }

        // Render vertices for Y
        for(int outer2 = 0; outer2<2; outer2++){
                for(int inner1 = 0; inner1<numberOfVerticesAlongY; inner1++){
                Instantiate(prefabDot, coordinatesOfYAxisVertices[inner1,outer2], Quaternion.identity);
            }
        }

        initializeCoordinatesForCenterVertices(coordinatesOfCenterVertices, coordinatesOfXAxisVertices, coordinatesOfYAxisVertices);
        
        for(int outer = 0; outer<numberOfVerticesAlongY; outer++)
        {
            for(int inner = 0; inner<numberOfVerticesAlongX; inner++)
            {
                Instantiate(prefabDot, coordinatesOfCenterVertices[inner,outer], Quaternion.identity);
            }
        }

        initializeCornerVertices(coordinatesOfXAxisVertices, 'x', xAxisLength, yAxisLength);
        initializeCornerVertices(coordinatesOfYAxisVertices, 'y', yAxisLength, xAxisLength);
           
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    private Vector3[,] initializeCoordinatesForCenterVertices(Vector3[,] centerCoordinatesArray, Vector3[,] xCoordinates, Vector3[,] yCoordinates)
    { 
        for(int outer = 0; outer<(yCoordinates.Length/2); outer++)
        {
            for(int inner = 0; inner<(xCoordinates.Length/2)-1; inner++)
            {
                centerCoordinatesArray[inner, outer].x = generateRandomCoordinatFromBounds(xCoordinates[inner,0].x, xCoordinates[inner+1,0].x);
            }
        }

        for(int outer = 0; outer<(xCoordinates.Length/2); outer++)
        {
            for(int inner = 0; inner<(yCoordinates.Length/2)-1; inner++)
            {
                centerCoordinatesArray[outer, inner].y = generateRandomCoordinatFromBounds(yCoordinates[inner,0].y, yCoordinates[inner+1,0].y);
            }
        }

        return centerCoordinatesArray;
    }

    private Vector3[,] initializeCoordinatesForAxisVertices(Vector3[,] coordinateArray, char axis, int axisLength, int oppositeAxisLength, int numberOfVertices)
    {   
        
        int boundInterval = (axisLength*2)/numberOfVertices;
        float startingCoordinate = (float) (-axisLength);
        float lastUpper = 0.0f;

        if(axis == 'x')
        {
            for(int oppisiteAxisValue = 0; oppisiteAxisValue<2; oppisiteAxisValue++)
            {
                for(int counter = 1; counter<numberOfVertices; counter++){
                    if(counter == 1)
                    {
                        lastUpper = startingCoordinate;
                    } 
                    else 
                    {
                        lastUpper = lastUpper + boundInterval;
                    }           
                    float upperBound = lastUpper + boundInterval;
                    float lowerBound = coordinateArray[counter-1,oppisiteAxisValue].x;
                    float newCoordinate = generateRandomCoordinatFromBounds(lowerBound,upperBound);
                    Debug.Log("Upperbound " + counter + ": " + upperBound);
                    Debug.Log("Lowerbound " + counter + ": " + lowerBound);
                    Debug.Log("Coordinate " + counter + ":" + newCoordinate);
                    if(oppisiteAxisValue == 0)
                    {
                        coordinateArray[counter,oppisiteAxisValue].x = newCoordinate;
                        coordinateArray[counter,oppisiteAxisValue].y = -oppositeAxisLength;
                    }
                    else
                    {
                        coordinateArray[counter,oppisiteAxisValue].x = newCoordinate;
                        coordinateArray[counter,oppisiteAxisValue].y = oppositeAxisLength;
                    }
                    
                }
            }
        }

        if(axis == 'y')
        {
            for(int oppisiteAxisValue = 0; oppisiteAxisValue<2; oppisiteAxisValue++)
            {
                for(int counter = 1; counter<numberOfVertices; counter++){   
                    if(counter == 1)
                    {
                        lastUpper = startingCoordinate;
                    } 
                    else 
                    {
                        lastUpper = lastUpper + boundInterval;
                    }        
                    float upperBound = lastUpper + boundInterval;
                    float lowerBound = coordinateArray[counter-1,oppisiteAxisValue].y;
                    float newCoordinate = generateRandomCoordinatFromBounds(lowerBound,upperBound);
                    Debug.Log("Upperbound " + counter + ": " + upperBound);
                    Debug.Log("Lowerbound " + counter + ": " + lowerBound);
                    if(oppisiteAxisValue == 0)
                    {
                        coordinateArray[counter,oppisiteAxisValue].y = newCoordinate;
                        coordinateArray[counter,oppisiteAxisValue].x = -oppositeAxisLength;
                    }
                    else
                    {
                        coordinateArray[counter,oppisiteAxisValue].y = newCoordinate;
                        coordinateArray[counter,oppisiteAxisValue].x = oppositeAxisLength;
                    }
                    
                }
            }
        }

        return coordinateArray;
    }

    private Vector3[,] initializeCornerVertices(Vector3[,] coordinateArray, char axis, int axisLength, int oppositeAxisLength)
    {
        
        // For indexes
        int finalIndex = (axisLength*2) - 1;
        
        if(axis == 'x')
        {
            // Initialize corners
            coordinateArray[0,0].x = (float) (-axisLength);
            coordinateArray[0,0].y = (float) (-oppositeAxisLength);
            coordinateArray[finalIndex,0].x = (float) (axisLength);
            coordinateArray[finalIndex,0].y = (float) (-oppositeAxisLength);
            coordinateArray[0,1].x = (float) (-axisLength);
            coordinateArray[0,1].y = (float) (oppositeAxisLength);
            coordinateArray[finalIndex,1].x = (float) (axisLength);
            coordinateArray[finalIndex,1].y = (float) (oppositeAxisLength);
        }

        if(axis == 'y')
        {
            // Initialize corners
            coordinateArray[0,0].x = (float) (-oppositeAxisLength);
            coordinateArray[0,0].y = (float) (-axisLength);
            coordinateArray[finalIndex,0].x = (float) (-oppositeAxisLength);
            coordinateArray[finalIndex,0].y = (float) (axisLength);
            coordinateArray[0,1].x = (float) (oppositeAxisLength);
            coordinateArray[0,1].y = (float) (-axisLength);
            coordinateArray[finalIndex,1].x = (float) (oppositeAxisLength);
            coordinateArray[finalIndex,1].y = (float) (axisLength);
        }

        return coordinateArray;
    }

    private float generateRandomCoordinatFromBounds(float lowerBound, float upperBound){

        float resultingCoordinates;

        resultingCoordinates = Random.Range(lowerBound, upperBound);

        return resultingCoordinates;

    }
}
        