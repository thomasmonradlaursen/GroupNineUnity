using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PieceOutlineGenerator
{
    public static void GenerateOutline(GameObject gameObject, Vector3[] points)
    {
        var lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.green;
        lineRenderer.widthMultiplier = 0.03f;
        lineRenderer.positionCount = points.Length;
        lineRenderer.loop = true;
        lineRenderer.SetPositions(points);
        lineRenderer.useWorldSpace = false;
    }
}