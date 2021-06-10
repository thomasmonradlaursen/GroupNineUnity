using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circumscribed //Circumscribed circle script
{
    float DeterminantOf3DMatrix(float[,] matrix)
    {
        float[,] subMatrix1 = new float[2, 2] { { matrix[1, 1], matrix[1, 2] }, { matrix[2, 1], matrix[2, 2] } };
        float[,] subMatrix2 = new float[2, 2] { { matrix[1, 0], matrix[1, 2] }, { matrix[2, 0], matrix[2, 2] } };
        float[,] subMatrix3 = new float[2, 2] { { matrix[1, 0], matrix[1, 1] }, { matrix[2, 0], matrix[2, 1] } };
        return matrix[0, 0] * DeterminantOf2DMatrix(subMatrix1) - matrix[0, 1] * DeterminantOf2DMatrix(subMatrix2) + matrix[0, 2] * DeterminantOf2DMatrix(subMatrix3);
    }
    float DeterminantOf2DMatrix(float[,] matrix)
    {
        return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
    }
    Vector3 CalculateConstantS(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 S = new Vector3();
        float[,] matrixForXCoordinate = new float[3, 3] { { Mathf.Pow(A.magnitude,2), A.y, 1 }, { Mathf.Pow(B.magnitude,2), B.y, 1 }, { Mathf.Pow(C.magnitude,2), C.y, 1 } };
        float[,] matrixForYCoordinate = new float[3, 3] { { A.x, Mathf.Pow(A.magnitude,2), 1 }, { B.x, Mathf.Pow(B.magnitude,2), 1 }, { C.x, Mathf.Pow(C.magnitude,2), 1 } };
        S.x = (1.0f / 2.0f) * DeterminantOf3DMatrix(matrixForXCoordinate);
        S.y = (1.0f / 2.0f) * DeterminantOf3DMatrix(matrixForYCoordinate);
        Debug.Log("x-coordinate: " + S.x);
        Debug.Log("y-coordinate: " + S.y);
        return S;
    }
    float CalculateConstantA(Vector3 A, Vector3 B, Vector3 C)
    {
        float[,] matrixForA = new float[3, 3] { { A.x, A.y, 1 }, { B.x, B.y, 1 }, { C.x, C.y, 1 } };
        float a = DeterminantOf3DMatrix(matrixForA);
        return a;
    }
    float CalculateConstantB(Vector3 A, Vector3 B, Vector3 C)
    {
        float[,] matrixForB = new float[3, 3] { { A.x, A.y, Mathf.Pow(A.magnitude,2) }, { B.x, B.y, Mathf.Pow(B.magnitude,2) }, { C.x, C.y, Mathf.Pow(C.magnitude,2) } };
        float b = DeterminantOf3DMatrix(matrixForB);
        return b;
    }
    Vector3 CalculateCircumcenter(Vector3 S, float a)
    {
        return S / a;
    }
    float CalculateCircumradius(Vector3 S, float a, float b)
    {
        return Mathf.Sqrt(b / a + Mathf.Pow(S.magnitude, 2) / Mathf.Pow(a, 2));
    }
    public (Vector3, float) GetCircumcenterAndCircumradius(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 S = CalculateConstantS(A, B, C);
        float a = CalculateConstantA(A, B, C);
        float b = CalculateConstantB(A, B, C);
        Vector3 circumcenter = CalculateCircumcenter(S, a);
        float circumradius = CalculateCircumradius(S, a, b);
        return (circumcenter, circumradius);
    }
    public bool IsPointOnCirle(Vector3 circumcenter, float circumradius, Vector3 point)
    {
        bool result = true;
        if(Vector3.Distance(point, circumcenter) > circumradius) result = false;
        return result;
    }
}
