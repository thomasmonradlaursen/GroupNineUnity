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



    // bool selected;

    // private void OnMouseDown() {
    //     // if(selected)
    //     // {selected = false;}
    //     // else
    //     // {selected = true;}   
    //     selected = true;
    // }

    // private void OnMouseUp() {
    //     // if(selected)
    //     // {selected = false;}
    //     // else
    //     // {selected = true;}   
    //     selected = false;
    // }

    // private void Update() {
    //     if(selected){
            
    //         Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
    //         var offset = transform.position - Camera.main.ScreenToWorldPoint(mousePoint);
    //         transform.position = mousePosition;
    //         // transform.Translate(mousePosition);

    //     }
    // }  
    
}



// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class DragNDrop : MonoBehaviour
// {
    
//     bool selected;

//     private void OnMouseDown() {
//         if(selected)
//         {selected = false;}
//         else
//         {selected = true;}   
//     }

//     private void Update() {
//         if(selected){
            
//             Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
//             transform.Translate(mousePosition);

//         }
//     }  
    
// }