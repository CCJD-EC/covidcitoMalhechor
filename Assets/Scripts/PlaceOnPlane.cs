using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceOnPlane : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;
        public GameObject another_prefab;

        
        
        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }
        public GameObject itemOn { get; private set; }

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
        }

        void Start()
        {
            m_animator = GetComponent<Animator>();
        }

        bool TryGetTouchPosition(out Vector2 touchPosition)
        {
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            touchPosition = default;
            return false;
        }

        void Update()
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;

                if (spawnedObject == null)
                {
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    /*var position = spawnedObject.transform.position;
                    itemOn= Instantiate(another_prefab, new Vector3(position.x,
                            position.y +10f, position.z), m_PlacedPrefab.transform.rotation);*/
                }
                else
                {
                    itemOn.transform.position = hitPose.position;

                }
                /*else
                {
                    spawnedObject.transform.position = hitPose.position;
                }*/
                
                
            }
        }
        bool AnimatorIsPlaying()
        {
            return m_animator.GetCurrentAnimatorStateInfo(0).length >
                   m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        private Animator m_animator;
        private List<GameObject> n_items = new List<GameObject>();

        private List<Vector3> n_coordinates = new List<Vector3>()
        {
            new Vector3(-0.3f, 1f, -0.1f),
            new Vector3(-0.3f, 1f, 0.3f),
            new Vector3(-0.3f, 1f, 0.08f),
            new Vector3(-0.1f, 1f, 0),
            new Vector3(0.3f, 1f, 0),
        };

        private float YpositionAbs = 0;

        static List<GameObject> n_Items = new List<GameObject>();
        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}
