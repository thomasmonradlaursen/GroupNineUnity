using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class BoardOutlineGenerator : MonoBehaviour
{

    LineRenderer lineRenderer;
    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        var jsonPuzzle = GetComponent<MeshFromJsonGenerator>().Puzzle;
        var shape = jsonPuzzle.puzzle.form;

        var points = new Vector3[4];

        var idx = 0;
        foreach (var form in shape)
        {
            points[idx].x = form.coord.x;
            points[idx].y = form.coord.y;
            idx++;
        }

        // var lineRenderer = gameObject.AddComponent<LineRenderer>();

        //         // A simple 2 color gradient with a fixed alpha of 1.0f.
        // float alpha = 1.0f;
        // Gradient gradient = new Gradient();
        // gradient.SetKeys(
        //     new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.green, 1.0f) },
        //     new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        // );
        // lineRenderer.colorGradient = gradient;
        lineRenderer.SetPositions(points);

    }
}
