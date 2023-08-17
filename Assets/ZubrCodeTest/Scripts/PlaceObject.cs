using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject[] objectPrefab;
    private ARRaycastManager rayManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    //above initialise a new list to store all the raycast hits

    private GameObject selectedObject = null; //selected ARCube reference
    private Vector2 previousTouchPosition = Vector2.zero; //stores last touch position for movement
    private float previousDistance = 0.0f; //stores distance between two fingers for pinch to scale

    private void Awake()
    {
        rayManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        //enables touch simulation for in-editor testing
        EnhancedTouch.TouchSimulation.Enable(); //to use the mouse to simulate touch in the Editor
        //turns on enhanced touch
        EnhancedTouch.EnhancedTouchSupport.Enable(); //to enable enhanced touch itself
        //subscribes FingerDown method to touch event
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        //subscribes FingerMove method to finger movement event
        EnhancedTouch.Touch.onFingerMove += FingerMove;
    }

    private void OnDisable()
    {
        //opposite of above, cleans up on disabling
        EnhancedTouch.TouchSimulation.Disable(); //to use the mouse to simulate touch in the Editor
        EnhancedTouch.EnhancedTouchSupport.Disable(); //to enable enhanced touch itself
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        EnhancedTouch.Touch.onFingerMove -= FingerMove;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        //ignores all fingers except the first one
        if (finger.index != 0) //0 is the first in the index, so if using more than 1 finger then don't do anything
            return;
        else
            HandleRaycast(finger);
    }

    private void HandleRaycast(EnhancedTouch.Finger finger)
    {
        if (rayManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;
            //casting a ray to detect if we're touching any existing object
            Ray ray = Camera.main.ScreenPointToRay(finger.currentTouch.screenPosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                //if touching an object, store reference and change color
                selectedObject = hitInfo.transform.gameObject;
                selectedObject.GetComponent<Renderer>().material.color = Random.ColorHSV();
            }
            else
            {
                //if not touching any object, spawn a new one
                int i = Random.Range(0, objectPrefab.Length);
                Instantiate(objectPrefab[i], pose.position, pose.rotation);
            }
        }
    }

    private void FingerMove(EnhancedTouch.Finger finger)
    {
        if (selectedObject == null) return;
        //handles the movement of selected ARCube
        if (finger.index == 0)
        {
            Vector2 touchDelta = finger.currentTouch.screenPosition - previousTouchPosition;
            if (touchDelta.magnitude > 0.01f)
            {
                Ray ray = Camera.main.ScreenPointToRay(finger.currentTouch.screenPosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    selectedObject.transform.position = hit.point;
                }
            }
            previousTouchPosition = finger.currentTouch.screenPosition;
        }

        //handles pinch to scale functionality
        if (EnhancedTouch.Touch.activeFingers.Count == 2)
        {
            var touch0 = EnhancedTouch.Touch.activeFingers[0].currentTouch.screenPosition;
            var touch1 = EnhancedTouch.Touch.activeFingers[1].currentTouch.screenPosition;

            float currentDistance = Vector2.Distance(touch0, touch1);
            if (previousDistance > 0)
            {
                float scaleChange = currentDistance / previousDistance;
                selectedObject.transform.localScale *= scaleChange;
            }
            previousDistance = currentDistance;
        }
        else
        {
            previousDistance = 0.0f;
        }
    }
}
