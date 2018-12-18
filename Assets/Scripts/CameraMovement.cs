using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour {

    public Camera cam;
    public Transform Player1;
    public Transform Player2;
    public Vector3 offset;
    public float smoothTime = .5f;

    public float minZoom = 40.0f;
    public float maxZoom = 10.0f;
    public float zoomLimiter = 50f;

    private Vector3 velocity;

    [Header("UI Components: Used by other scripts")]
    public Text Player1_UIhealthNum;
    public Text Player2_UIhealthNum;
    public Slider Player1_UIhealthSlider;
    public Slider Player2_UIhealthSlider; 

    //LateUpdate waits for all script functions to finish before executing
    //Important for cameras that track players... They may move during the update
    void LateUpdate(){
        Move();
        Zoom();
    }

    void Zoom(){
        //Debug.Log(GetGreatestDistance());
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance(){
        if(Player1 == null){
            var bounds = new Bounds(Player2.position, Vector3.zero);
            return bounds.size.x;
        }
        else if(Player2 == null){
            var bounds = new Bounds(Player1.position, Vector3.zero);
            return bounds.size.x;
        }
        else{
            var bounds = new Bounds(Player1.position, Vector3.zero);
            bounds.Encapsulate(Player2.position);
            return bounds.size.x;
        }
    }

    void Move(){
        Vector3 centerpoint = GetCenterPoint();

        Vector3 newPosition = centerpoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

    }

    Vector3 GetCenterPoint(){
        if(Player1 == null){
            var bounds = new Bounds(Player2.position, Vector3.zero);
            return bounds.center;
        }
        else if(Player2 == null){
            var bounds = new Bounds(Player1.position, Vector3.zero);
            return bounds.center;
        }
        else{
            var bounds = new Bounds(Player1.position, Vector3.zero);
            bounds.Encapsulate(Player2.position);
            return bounds.center;
        }
    }

}
