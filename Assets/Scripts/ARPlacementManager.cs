using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlacementManager : MonoBehaviour
{
    public GameObject hoopPrefab;
    public GameObject placementIndicatorPrefab;
    public BasketballShooter basketballShooter;
    public ARRaycastManager arRaycastManager;
    public float indicatorHeightOffset = 1.5f;

    public ARPlaneManager arPlaneManager;

    private GameObject placementIndicator;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool hoopPlaced = false;
    private bool indicatorEnabled = false;

    public float indicatorDistance = 2f;

    private GameObject instantiatedHoop; // Store the reference to the instantiated hoop

    void Start()
    {
        placementIndicator = Instantiate(placementIndicatorPrefab);
        placementIndicator.SetActive(false);
    }

    void Update()
    {
        if (hoopPlaced && indicatorEnabled)
        {
            UpdatePlacementIndicator();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !hoopPlaced)
        {
            UpdatePlacementPose(Input.GetTouch(0).position);
            if (placementPoseIsValid)
            {
                PlaceHoop();
            }
        }

        if (Input.GetMouseButtonDown(0) && !hoopPlaced)
        {
            UpdatePlacementPose(Input.mousePosition);
            if (placementPoseIsValid)
            {
                PlaceHoop();
            }
        }
    }

    void UpdatePlacementPose(Vector2 touchPosition)
    {
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(touchPosition, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    void UpdatePlacementIndicator()
    {
        var cameraTransform = Camera.main.transform;
        placementPose.position = cameraTransform.position + cameraTransform.forward * indicatorDistance;

        placementPose.position += new Vector3(0, indicatorHeightOffset, 0);

        placementPose.rotation = Quaternion.identity;

        placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        placementPoseIsValid = true;
    }

    void PlaceHoop()
    {
        instantiatedHoop = Instantiate(hoopPrefab, placementPose.position, placementPose.rotation); // Store the instantiated hoop
        hoopPlaced = true;
        indicatorEnabled = true;
        Debug.Log("Hoop Placed");

        placementIndicator.SetActive(true);

        // Assign the instantiated hoop to the basketball shooter
        basketballShooter.basketTransform = instantiatedHoop.transform; // Set the basketTransform reference in BasketballShooter

        // Disable plane detection and hide existing planes
        DisablePlaneDetection();
    }

    public void PlaceBall()
    {
        if (hoopPlaced && placementPoseIsValid && basketballShooter.currentLives > 0 && basketballShooter.SpawnedBasketball == null)
        {
            // Set the spawn point to the indicator's position
            Transform spawnPoint = placementIndicator.transform;

            // Add a vertical offset to the spawn point position
            Vector3 spawnPosition = spawnPoint.position + new Vector3(0, 0.02f, 0); // Adjust the 0.5f value as needed

            // Set the spawn point with the new position
            basketballShooter.spawnPoint.position = spawnPosition;
            basketballShooter.spawnPoint.rotation = spawnPoint.rotation;

            basketballShooter.SpawnBasketball();
        }
    }

    void DisablePlaneDetection()
    {
        arPlaneManager.enabled = false; // Disable the ARPlaneManager to stop detecting new planes

        // Deactivate all existing planes
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }

}
