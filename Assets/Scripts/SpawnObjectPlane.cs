using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnObjectPlane : MonoBehaviour
{
    /// <summary>
    /// Prefab used for initialization of game
    /// </summary>
    public GameObject _prefabInicial;

    private ARPlaneManager _planeManager;
    private GameObject spawn;
    
    // Start is called before the first frame update
    void Start()
    {
            m_RaycastManager = GetComponent<ARRaycastManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var plane in _planeManager.trackables)
        {
            if (spawn==null)
            {
                spawn= Instantiate(_prefabInicial, plane.gameObject.transform.position, Quaternion.identity);
            }
        }

        if (!TryGetTouchPosition(out Vector3 touchPosition))
            return;

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Destroy(_prefabInicial);
        }
    }
    
    bool TryGetTouchPosition(out Vector3 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }
    private void Awake()
    {
        _planeManager = GetComponent<ARPlaneManager>();
    }
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    ARRaycastManager m_RaycastManager;
}
