using System.Collections.Generic; //needed to use lists
using UnityEngine;
using UnityEngine.XR.ARFoundation; //this and below needed for AR functionality
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch; //using for touchscreen functionality
//the above as a variable so we don't have to reference the long namespace each time


[RequireComponent(requiredComponent: typeof(ARRaycastManager),
    requiredComponent2: typeof(ARPlaneManager))]
//the above so that this script can only be attached to gameobject with those
//components, and those components can't be removed, essential components for this to work


public class PlaceObject : MonoBehaviour
{

    [SerializeField] private GameObject objectPrefab; //will attach cube here in editor

    private ARRaycastManager rayManager;
    //private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    //above, initialise a new list to store all the raycast hits

    private void Awake()
    {
        rayManager = GetComponent<ARRaycastManager>();
        //planeManager = GetComponent<ARPlaneManager>();
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

    private void FingerDown(EnhancedTouch.Finger finger) //info to do with the finger press passed in
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
        //similar to raycast functionity but optimised for AR, raycast takes in screen postion of where pressed,
        //adds list of hits to list initialised earlier, what we want to detect against (in this case plane
        //within polygon, as the plane object = polygon)
        //the other TrackableType possibilities: https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Experimental.XR.TrackableType.html

        if (rayManager.Raycast(screenPoint: finger.currentTouch.screenPosition,
            hitResults: hits, trackableTypes: TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits)
            {
                //for each hit we want to create a pose object from the hit.pose data which determines
                //position and orientation of whatever we hit on the plane
                Pose pose = hit.pose;

                //instantiate a new cube from prefab at the location and rotation of object for correct normals
                GameObject obj = Instantiate(original: objectPrefab, position:
                    pose.position, rotation: pose.rotation);
            }

        }
    }




}
