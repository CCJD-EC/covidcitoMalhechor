using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementManyObjectsController : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;

    [SerializeField] private Camera arCamera;

    private Vector2 touchPosition = default;

    private ARRaycastManager _arRaycastManager;

    private bool onTouchHold = false;

    private GameObject placedObject;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }


    private void Update()
    {
        //on double touch swap click
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.transform.name.Contains("placedObject"))
                        onTouchHold = true;
                }
            }

            if (touch.phase == TouchPhase.Ended)
                onTouchHold = false;
        }

        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            if (placedObject == null)
            {
                placedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                placedObject.transform.position = hitPose.position;
                placedObject.transform.rotation = hitPose.rotation;
            }
        }
    }
}