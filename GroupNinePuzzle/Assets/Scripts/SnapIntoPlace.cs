using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapIntoPlace : MonoBehaviour
{
    // Start is called before the first frame update
    void OnMouseUp()
    {
        AutoTranslate2();
    }

    Vector3 FindCenterOfMassInPiece(Mesh piece){
        float sumX = 0;
        float sumY = 0;

        foreach(var vertex in piece.vertices){
            sumX += vertex.x;
            sumY += vertex.y;
        }

        var centerOfMass = new Vector3();
        centerOfMass.x = sumX/piece.vertices.Length;
        centerOfMass.y = sumY/piece.vertices.Length;

        return centerOfMass;
    }

    float FindAreaOfPossiblePieces(Mesh piece, Vector3 centerOfMass){
        float greatestDistance = 0;

        foreach(var vertex in piece.vertices){
            var distanceFromCenterToVertex = Mathf.Sqrt(Mathf.Pow(vertex.x - centerOfMass.x, 2) + Mathf.Pow(vertex.y - centerOfMass.y, 2));
            if(distanceFromCenterToVertex > greatestDistance){
                greatestDistance = distanceFromCenterToVertex;
            }
        }

        return greatestDistance + 1;
    }

    void AutoTranslate(Mesh piece2){
        //piece1 is the one that was just moved, piece2 is the one piece1 moved close to
        Mesh piece1 = GetComponentInParent<MeshFilter>().mesh;
        float minDist = Mathf.Infinity;
        Vector3 closestPoint = new Vector3(0,0,0);
        foreach(Vector3 a in piece1.vertices){
            float temp = Mathf.Sqrt((a[0]*a[0])+(a[1]*a[1]));
            foreach(Vector3 b in piece2.vertices){
                float testDist = Mathf.Sqrt((b[0]*b[0])+(b[1]*b[1]));
                if(testDist < minDist){
                    minDist = testDist;
                    closestPoint = b;
                }
            }
        }
        //move piece
        transform.position = closestPoint;
    }

    void AutoTranslate2(){
        
        Mesh piece1 = GetComponentInParent<MeshFilter>().mesh;
        Vector3 location = piece1.vertices[0];
        var collider = GetComponent<SphereCollider>();
        
        Debug.Log("Collider radius: "+ collider.radius);
        if (!collider)
        {
            return; // nothing to do without a collider
        }
        Vector3 closestPoint = collider.ClosestPoint(location);
        Debug.Log("Closest point: "+ closestPoint);
        transform.position = closestPoint;
    }
}
