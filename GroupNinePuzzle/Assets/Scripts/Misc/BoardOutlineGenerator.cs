using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class BoardOutlineGenerator : MonoBehaviour
{
    LineRenderer lineRenderer;
    void Start()
    {
        var lineobject = new GameObject();
        lineRenderer = lineobject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.yellow;
        lineRenderer.widthMultiplier = 0.03f;
        lineRenderer.positionCount = 4;
        lineRenderer.loop = true;
        lineobject.transform.parent = this.transform;
    }

    void Update()
    {
        var jsonPuzzle = GetComponent<PuzzleModel>().puzzle;
        var shape = jsonPuzzle.puzzle.form;

        var points = new Vector3[4];

        var idx = 0;
        foreach (var form in shape)
        {
            points[idx].x = form.coord.x;
            points[idx].y = form.coord.y;
            idx++;
        }
        lineRenderer.SetPositions(points);

    }
}