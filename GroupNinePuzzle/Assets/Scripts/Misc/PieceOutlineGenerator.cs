using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

// Author: Gustav Nilsson Pedersen

public class PieceOutlineGenerator
{
    public static void GenerateOutline(GameObject gameObject, Vector3[] points, float length, float width)
    {
        var lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.black;
        lineRenderer.widthMultiplier = (length + width) * 0.003f;
        lineRenderer.positionCount = points.Length;
        lineRenderer.loop = true;
        lineRenderer.SetPositions(points);
        lineRenderer.useWorldSpace = false;
    }
}