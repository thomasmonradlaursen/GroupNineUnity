using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Below code is based on https://gamedev.stackexchange.com/questions/167317/scale-camera-to-fit-screen-size-unity
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FitCameraToPuzzle : MonoBehaviour
{

    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth;
    public float sceneHeight;

    Camera _camera;
    void Start()
    {
        // _camera = GetComponent<Camera>();
        // var jsonPuzzle = GetComponentInParent<MeshFromJsonGenerator>().Puzzle;
        // var shape = jsonPuzzle.puzzle.form;
        // var points = new Vector3[4];

        // sceneHeight = points[2].y - points[0].y;
        // Debug.Log(sceneHeight);
        // sceneWidth = points[2].x - points[0].x;
        // Debug.Log(sceneWidth);
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void Update()
    {
        _camera = GetComponent<Camera>();
        var jsonPuzzle = GetComponentInParent<MeshFromJsonGenerator>().Puzzle;
        var shape = jsonPuzzle.puzzle.form;
        var points = new Vector3[4];

        sceneHeight = points[2].y - points[0].y;
        Debug.Log(sceneHeight);
        sceneWidth = points[2].x - points[0].x;
        Debug.Log(sceneWidth);
        float unitsPerPixelWidth = sceneWidth / Screen.width;
        float unitsPerPixelHeight = sceneHeight / Screen.height;

        if (unitsPerPixelWidth > unitsPerPixelHeight)
        {
            float desiredHalfHeight = 0.5f * unitsPerPixelWidth * Screen.height;
            _camera.orthographicSize = desiredHalfHeight;
            Debug.Log(sceneWidth);
        }
        else
        {
            float desiredHalfHeight = 0.5f * unitsPerPixelHeight * Screen.width;
            _camera.orthographicSize = desiredHalfHeight;
            Debug.Log(sceneWidth);
        }
    }
}
