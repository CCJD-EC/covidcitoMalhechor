using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Button = UnityEngine.UI.Button;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementWithDraggingDroppingController : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;

    [SerializeField] private GameObject welcomePanel;

    [SerializeField] private Camera arCamera;

    [SerializeField] private Button dissmissButton;

    private Vector2 touchPosition = default;

    private ARRaycastManager _arRaycastManager;

    private bool onTouchHold = false;

    private GameObject placedObject;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        dissmissButton.onClick.AddListener(Dismiss);
    }

    private void Dismiss() => welcomePanel.SetActive(false);

    private void Update()
    {
        if (welcomePanel.activeSelf)
            return;
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
                    

                    if (hitObject.transform.name.Contains("PlacedObject"))
                    {
                        Debug.Log("I had touched a spray");
                        onTouchHold = true;
                    }
                        
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
                if (onTouchHold)
                {
                    placedObject.transform.position = hitPose.position;
                    placedObject.transform.rotation = hitPose.rotation;
                }
            }
        }
    }
}