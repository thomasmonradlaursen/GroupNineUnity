using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DragNDrop : MonoBehaviour
{

    private  Vector3 mouseOffset;
    private float mouseZcoord = -10;

    void OnMouseDown(){
        mouseZcoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        
        mouseOffset = gameObject.transform.position - MouseWorldPosition();

    }

    private Vector3 MouseWorldPosition(){
        Vector3 mousePoint = Input.mousePosition;
        
        mousePoint.z = mouseZcoord;
        
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePoint);

        return mouseWorldPosition; 
    }

    void OnMouseDrag(){
        transform.position = MouseWorldPosition() + mouseOffset;
    }
}