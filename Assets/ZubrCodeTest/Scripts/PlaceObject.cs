using System.Collections.Generic; //needed to use lists
using TMPro; //needed for text mesh pro
using UnityEngine;
using UnityEngine.XR.ARFoundation; //this and below needed for AR functionality
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch; //using for new touchscreen functionality
//the above as a variable so we don't have to reference the long namespace each time


[RequireComponent(requiredComponent: typeof(ARRaycastManager),
    requiredComponent2: typeof(ARPlaneManager))]
//the above so that this script can only be attached to gameobject with those
//components, and those components can't be removed, essential components for this to work


public class PlaceObject : MonoBehaviour
{

    [SerializeField] private GameObject objectPrefab; //will attach cube here in editor

    private ARRaycastManager rayManager; //for special AR raycast that can detect the planes to place objects
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    //above, initialise a new list to store all the raycast hits

    private GameObject lastSelectedObject = null;  //to keep track of the last selected object
    [SerializeField] private TextMeshProUGUI debugText; //so I can see debug messages within the app

    [SerializeField] private Camera arCamera; //very important, and important to specify this camera for physics raycast NOT main camera

    //for pinch to scale
    private Vector2 previousDistance = Vector2.zero;
    private float previousPinchDistance = 0f;

    //flag to make sure that selected objects can be moved/scaled and so a new cube is not instantiated
    private bool isCubeSelected = false;


    private void Awake()
    {
        rayManager = GetComponent<ARRaycastManager>(); //get this from the component this script is attached to
    }

    private void OnEnable()
    {
        EnhancedTouch.EnhancedTouchSupport.Enable(); //to enable enhanced touch itself
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        //above is an event, the syntax += is to subscribe to the event and add a listener method

    }

    private void OnDisable()
    {
        EnhancedTouch.EnhancedTouchSupport.Disable(); //to enable enhanced touch itself
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        //above is an event, the syntax -= is to unsubscribe to the event and remove a listener method

    }

    private void Update()
    {
        //check for single touch for dragging
        if (EnhancedTouch.Touch.activeFingers.Count == 1)
        {
            //get the details of the current touch event for the 1 active finger, store this in local var touch
            var touch = EnhancedTouch.Touch.activeFingers[0].currentTouch;

            //check that a cube is selected, the touch movement is ongoing, not ended, and we have a valid object reference for last selected cube
            if (isCubeSelected && touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && lastSelectedObject != null)
            {
                //check if our special AR ray hits any AR planes, it uses the screen position of the touch and stores any hit results in the 'hits' list made earlier
                //this is to only move the cube within the confines of AR planes
                if (rayManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    //display the name of the hit object in the debug text
                    debugText.text = "Hit: " + hits[0];
                    //if a hit was found, take the position/pose of the first hit
                    Pose hitPose = hits[0].pose;
                    //moves the last selected cube to this new position while finger still not lifted off screen
                    lastSelectedObject.transform.position = hitPose.position;
                }
            }

            //check if the current touch phase is ended/finger lifted off the screen
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                //reset the cube selection flag since the touch ended
                isCubeSelected = false;
            }
        }

        //check if 2 fingers are touching the screen at once/pinch gesture
        else if (EnhancedTouch.Touch.activeFingers.Count == 2)
        {
            //get the details of the current touch event for the first and second active fingers, store in local vars
            var touch0 = EnhancedTouch.Touch.activeFingers[0].currentTouch;
            var touch1 = EnhancedTouch.Touch.activeFingers[1].currentTouch;

            //check if this is the start of the pinch gesture by confirming no previous distance is recorded
            if (previousDistance == Vector2.zero)
            {
                //calculate the distance between the two touches in screen space
                previousDistance = touch1.screenPosition - touch0.screenPosition;
                //store the magnitude/length of this distance as the starting pinch distance
                previousPinchDistance = previousDistance.magnitude;
            }
            else //it is not the start of the pinch gesture
            {
                //calculate the current distance between the two touches in screen space
                Vector2 currentDistance = touch1.screenPosition - touch0.screenPosition;
                //find the magnitude/length of this distance
                float currentPinchDistance = currentDistance.magnitude;

                //find the change in distance between the current pinch and the last pinch
                float pinchDelta = currentPinchDistance - previousPinchDistance;

                //check if a cube object is currently selected
                if (lastSelectedObject != null)
                {
                    //calculate the scaling factor based on the change in pinch distance, multiplied by a sensitivity value of 0.01
                    float scaleFactor = 1 + (pinchDelta * 0.01f); //0.01f tested as a good sensitivity value for smooth pinch scaling
                    //apply this scale factor to the selected object's size
                    lastSelectedObject.transform.localScale *= scaleFactor;
                }

                //update the previous pinch distance to the current one for the next frame's calculation
                previousPinchDistance = currentPinchDistance;
            }
        }
        //if neither 1 or 2 fingers are touching the screen
        else
        {
            //reset the previous distance for pinch calculations
            previousDistance = Vector2.zero;

            isCubeSelected = false;
        }
    }



    private void FingerDown(EnhancedTouch.Finger finger) //info to do with the finger press is passed in
    {
       
        if (finger.index != 0) //0 is the first in the index, so if using more than 1 finger
        //then don't do anything, as for this implementation we want it to be just with a single touch
            return;
        else
            HandleRaycast(finger);

    }




    private void HandleRaycast(EnhancedTouch.Finger finger)
    {

        //similar to raycast functionity but optimised for AR, raycast takes in screen postion of where pressed,
        //adds list of hits to list initialised earlier, what we want to detect against, in this case plane
        //within polygon, as the plane object = polygon

        //the other TrackableType possibilities: https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Experimental.XR.TrackableType.html


        //define ray for a regular Physics.Raycast from AR camera to where the screen was touched, will be to check for other objects (cubes)
        Ray ray = arCamera.ScreenPointToRay(finger.currentTouch.screenPosition);
        //variable to store info about what the ray hits
        RaycastHit hitInfo;

        //shoot the ray into the scene and check if it hits anything
        if (Physics.Raycast(ray, out hitInfo))
        {
            //check if the hit object has the tag "Cube"
            if (hitInfo.transform.CompareTag("Cube"))
            {
                //display the name of the hit object in the debug text
                debugText.text = "Hit: " + hitInfo.transform.name;

                //if there was a previously selected cube, change its color back to white
                if (lastSelectedObject != null)
                {
                    lastSelectedObject.GetComponent<Renderer>().material.color = Color.white;
                }

                //change the color of the hit cube to green to indicate it's selected
                hitInfo.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
                //store this cube as the last selected object
                lastSelectedObject = hitInfo.transform.gameObject;
                //set the flag to indicate a cube has been selected
                isCubeSelected = true;
                return; //end the method since we've interacted with a cube
            }
        }

        //if no cube was selected and the AR raycast hits a plane, place a new object
        if (!isCubeSelected && rayManager.Raycast(screenPoint: finger.currentTouch.screenPosition, hitResults: hits, trackableTypes: TrackableType.PlaneWithinPolygon))
            {
            //for the first hit we want to create a pose object from the hit.pose data which determines
            //position and orientation of whatever we hit on the plane
            Pose pose = hits[0].pose;
            //if you didn't hit a cube but an AR Plane, instantiate a new object.
            Instantiate(objectPrefab, pose.position, pose.rotation);
        }
    }

}