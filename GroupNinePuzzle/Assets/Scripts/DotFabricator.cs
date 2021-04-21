using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotFabricator : MonoBehaviour
{
    // Fields for setup of vertices
    static int xAxisLength = 6;
    static int yAxisLength = 4;
    static int numberOfVerticesAlongX = 10;
    static int numberOfVerticesAlongY = 10;
    public GameObject prefabDot = null;

    // Arrays for coordinates
    public Vector3[,] coordinatesOfCenterVertices = new Vector3[numberOfVerticesAlongX-1,numberOfVerticesAlongY-1];
    public Vector3[,] coordinatesOfXAxisVertices = new Vector3[numberOfVerticesAlongX,2];
    public Vector3[,] coordinatesOfYAxisVertices = new Vector3[numberOfVerticesAlongY,2];
    
    // Start is called before the first frame update
    void Start(){
        initCornerVertices(coordinatesOfXAxisVertices, coordinatesOfYAxisVertices, xAxisLength, yAxisLength);
        // initCornerVertices(coordinatesOfYAxisVertices, yAxisLength, AxisLength);

        // Debug.Log("Number of dots along x-axis: " + numberOfVerticesAlongX);
        // Debug.Log("Number of dots along y-axis: " + numberOfVerticesAlongY);

        initializeCoordinatesForAxisVertices(coordinatesOfXAxisVertices, 'x', xAxisLength, yAxisLength, numberOfVerticesAlongX);
        for(int counter = 0; counter<numberOfVerticesAlongX; counter++){
            // Debug.Log("X-bottom " + (counter+1) + ":" + coordinatesOfXAxisVertices[counter,0].ToString());
            // Debug.Log("X-top " + (counter+1) + ":" + coordinatesOfXAxisVertices[counter,1].ToString());
        }

        initializeCoordinatesForAxisVertices(coordinatesOfYAxisVertices, 'y', yAxisLength, xAxisLength, numberOfVerticesAlongY);
        for(int counter = 0; counter<numberOfVerticesAlongY; counter++){
            // Debug.Log("Y-bottom " + (counter+1) + ":" + coordinatesOfYAxisVertices[counter,0].ToString());
            // Debug.Log("Y-top " + (counter+1) + ":" + coordinatesOfYAxisVertices[counter,1].ToString());
        }

        // Render vertices for X
        for(int outer = 0; outer<2; outer++){
                for(int inner = 0; inner<numberOfVerticesAlongX; inner++){
                Instantiate(prefabDot, coordinatesOfXAxisVertices[inner,outer], Quaternion.identity);
            }
        }

        //Render vertices for Y
        for(int outer2 = 0; outer2<2; outer2++){
                for(int inner1 = 0; inner1<numberOfVerticesAlongY; inner1++){
                Instantiate(prefabDot, coordinatesOfYAxisVertices[inner1,outer2], Quaternion.identity);
            }
        }

        initializeCoordinatesForCenterVertices(coordinatesOfCenterVertices, coordinatesOfXAxisVertices, coordinatesOfYAxisVertices);
        
        for(int outer = 0; outer<numberOfVerticesAlongY-1; outer++)
        {
            for(int inner = 0; inner<numberOfVerticesAlongX-1; inner++)
            {
                Instantiate(prefabDot, coordinatesOfCenterVertices[inner,outer], Quaternion.identity);
            }
        }
           
    }

    // Update is called once per frame
    void Update(){
           
    }

    private Vector3[,] initializeCoordinatesForCenterVertices(Vector3[,] centerCoordinatesArray, Vector3[,] xCoordinates, Vector3[,] yCoordinates)
    { 
        for(int i=0; i<yCoordinates.Length/2; i++){
                Debug.Log("x0: " + yCoordinates[i,0].x + " ; y0: " + yCoordinates[i,0].y);
                Debug.Log("x1: " + yCoordinates[i,1].x + " ; y1: " + yCoordinates[i,1].y);
        }
        for(int outer = 0; outer<numberOfVerticesAlongY-1; outer++)
        {
            for(int inner = 0; inner<numberOfVerticesAlongX-1; inner++)
            {
                // if(outer == (yCoordinates.Length/2)-1)
                // {

                // }
                centerCoordinatesArray[inner, outer].x = generateRandomCoordinatFromBounds(xCoordinates[inner,0].x, xCoordinates[inner+1,0].x);
                // if(outer == (yCoordinates.Length/2)-1)
                // {
                // centerCoordinatesArray[inner, outer].y = generateRandomCoordinatFromBounds(yCoordinates[outer,0].y, );
                // }
                var test = generateRandomCoordinatFromBounds(yCoordinates[outer,0].y, yCoordinates[outer+1,0].y);
                Debug.Log("outer: " + outer + ". inner: " + inner + "upper bound y: " + yCoordinates[outer,0].y + " ; " + yCoordinates[outer+1,0].y);
                centerCoordinatesArray[inner, outer].y = test;

            }
        }



        return centerCoordinatesArray;
    }


    private Vector3[,] initializeCoordinatesForAxisVertices(Vector3[,] coordinateArray, char axis, int axisLength, int oppositeAxisLength, int numberOfVertices){   
        
        float boundInterval = (axisLength*2)/((float)numberOfVertices-2);
        float startingCoordinate = (float) (-axisLength);

        if(axis == 'x'){
            // for(int oppAxisVal = 0; oppAxisVal<2; oppAxisVal++){
                float lowerBound = startingCoordinate;
                for(int counter = 1; counter<numberOfVertices-1; counter++){
                    
                    float upperBound = lowerBound + boundInterval;
                    //float lowerBound = coordinateArray[counter-1,oppAxisVal].x;
                    float newCoordinate = generateRandomCoordinatFromBounds(lowerBound,upperBound);
                    // Debug.Log("Upperbound " + counter + ": " + upperBound);
                    // Debug.Log("Lowerbound " + counter + ": " + lowerBound);
                    // Debug.Log("Coordinate " + counter + ":" + newCoordinate);
                    // if(oppAxisVal == 0)
                    // {
                        coordinateArray[counter,0].x = newCoordinate;
                        coordinateArray[counter,0].y = -oppositeAxisLength;
                    // }
                    // else
                    // {
                        coordinateArray[counter,1].x = newCoordinate;
                        coordinateArray[counter,1].y = oppositeAxisLength;
                    // }
                    lowerBound += boundInterval;
                    //lastUpper2 = lastUpper2 + boundInterval;
                }
            // }
        }

        if(axis == 'y'){
            // for(int oppAxisVal = 0; oppAxisVal<2; oppAxisVal++) {
                float lowerBound = startingCoordinate;

                for(int counter = 1; counter<numberOfVertices-1; counter++){   
                    
                    float upperBound = lowerBound + boundInterval;
                    //float lowerBound = coordinateArray[counter-1,oppAxisVal].y;
                    float newCoordinate = generateRandomCoordinatFromBounds(lowerBound,upperBound);
                    // Debug.Log("Upperbound " + counter + ": " + upperBound);
                    // Debug.Log("Lowerbound " + counter + ": " + lowerBound);
                    // if(oppAxisVal == 0){
                        coordinateArray[counter,0].y = newCoordinate;
                        coordinateArray[counter,0].x = -oppositeAxisLength;
                    // }
                    // else{
                        coordinateArray[counter,1].y = newCoordinate;
                        coordinateArray[counter,1].x = oppositeAxisLength;
                    // }
                    lowerBound += boundInterval;
                    
                }
            // }
        }

        return coordinateArray;
    }
    private void initCornerVertices(Vector3[,] xCoordArray, Vector3[,] yCoordArray, int xAxisLength, int yAxisLength){
        // For indexes
        int finalIndexX = (xCoordArray.Length/2) - 1;
        // Initialize corners
        xCoordArray[0,0].x = (float) (-xAxisLength);
        xCoordArray[0,0].y = (float) (-yAxisLength);
        xCoordArray[finalIndexX,0].x = (float) (xAxisLength);
        xCoordArray[finalIndexX,0].y = (float) (-yAxisLength);
        xCoordArray[0,1].x = (float) (-xAxisLength);
        xCoordArray[0,1].y = (float) (yAxisLength);
        xCoordArray[finalIndexX,1].x = (float) (xAxisLength);
        xCoordArray[finalIndexX,1].y = (float) (yAxisLength);

        int finalIndexY = (yCoordArray.Length/2) - 1;
        // Initialize corners
        yCoordArray[0,0].x = (float) (-xAxisLength);
        yCoordArray[0,0].y = (float) (-yAxisLength);
        yCoordArray[finalIndexY,0].x = (float) (-xAxisLength); // reversed compared to X coordinates. This way it works with the current version of initializeCoordinatesForCenterVertices
        yCoordArray[finalIndexY,0].y = (float) (yAxisLength);
        yCoordArray[0,1].x = (float) (xAxisLength);
        yCoordArray[0,1].y = (float) (-yAxisLength);
        yCoordArray[finalIndexY,1].x = (float) (xAxisLength);
        yCoordArray[finalIndexY,1].y = (float) (yAxisLength);

    }
    /*
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
    */
    private float generateRandomCoordinatFromBounds(float lowerBound, float upperBound){

        float resultingCoordinates;

        resultingCoordinates = Random.Range(lowerBound, upperBound);

        return resultingCoordinates;

    }
}
        