using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
//the above as a variable so we don't have to reference the long namespace each time


[RequireComponent(requiredComponent: typeof(ARRaycastManager),
    requiredComponent2: typeof(ARPlaneManager))]
//the above so that this script can only be attached to gameobject with those
//components, and those components can't be removed


public class PlaceObject : MonoBehaviour
{

    [SerializeField] private GameObject[] objectPrefab;

    private ARRaycastManager rayManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    //above initialise a new list to store all the raycast hits

    private void Awake()
    {
        rayManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable(); //to use the mouse to simulate touch in the Editor
        EnhancedTouch.EnhancedTouchSupport.Enable(); //to enable enhanced touch itself
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        //above is an event, the syntax += is to subscribe to the event and add a listener method

    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable(); //to use the mouse to simulate touch in the Editor
        EnhancedTouch.EnhancedTouchSupport.Disable(); //to enable enhanced touch itself
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        //above is an event, the syntax -= is to unsubscribe to the event and remove a listener method

    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) //0 is the first in the index, so if using more than 1 finger
            //then don't do anything
            return;
        else
        {
            HandleRaycast(finger);

        }

    }

    private void HandleRaycast(EnhancedTouch.Finger finger)
    {
        if (rayManager.Raycast(screenPoint: finger.currentTouch.screenPosition,
            hitResults: hits, trackableTypes: TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits)
            {
                int i = Random.Range(0, objectPrefab.Length);
                Pose pose = hit.pose;
                GameObject obj = Instantiate(original: objectPrefab[i], position:
                    pose.position, rotation: pose.rotation);
            }

        }
    }




}
