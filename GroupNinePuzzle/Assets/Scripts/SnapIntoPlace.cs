using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapIntoPlace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 FindCenterOfMassInPiece(Mesh piece){
        float sumX = 0;
        float sumY = 0;

        foreach(var vertex in piece.vertices){
            sumX += vertex.x;
            sumY += vertex.y;
        }

        var centerOfMass = new Vector3();
        centerOfMass.x = sumX/piece.vertices.Length();
        centerOfMass.y = sumY/piece.vertices.Length();

        return centerOfMass;
    }

    float FindAreaOfPossiblePieces(Mesh piece, Vector3 centerOfMass){
        float greatestDistance = 0;

        foreach(var vertex in piece.vertices){
            var distanceFromCenterToVertex = sqrt((vertex.x - centerOfMass.x)^2 + (vertex.y -centerOfMass.y)^2);
            if(distanceFromCenterToVertex > greatestDistance){
                greatestDistance = distanceFromCenterToVertex;
            }
        }

        return greatestDistance + 1;
    }



}
