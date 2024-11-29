using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARRaycastPlace : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;
    public ARPlacementManager placementManager;
    public GameObject objectToPlace;

    public Camera arCamera;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();


    // Update is called once per frame
    void Update()
    {
        // Check if there's at least one touch on the screen
        if (Input.touchCount > 0)
        {
            // Get the first touch
            Touch touch = Input.GetTouch(0);

            // If the touch has just begun, perform the raycast
            if (touch.phase == TouchPhase.Began && placementManager.hoopPrefab == null)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);

                // Perform the AR raycast
                if (arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
                {
                    // Get the first hit from the raycast
                    Pose hitPose = hits[0].pose;

                    // Instantiate the object at the hit pose
                    Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
                }
            }
        }

    }
}
