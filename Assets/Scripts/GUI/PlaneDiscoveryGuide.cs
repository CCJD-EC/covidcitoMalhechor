using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneDiscoveryGuide : MonoBehaviour
{
    /// <summary>
    /// The time to delay, after ARCore loses tracking of any planes, showing the plane
    /// discovery guide.
    /// </summary>
    [Tooltip("The time to delay, after ARCore loses tracking of any planes, showing the plane" +
             "discovery guide.")]
    public float DisplayGuideDelay = 3.0f;

    /// <summary>
    /// The time to delay, after displaying the plane discovery guide, offering more detailed
    /// instructions on how to find a plane.
    /// </summary>
    [Tooltip("The time to delay, after displaying the plane discovery guide, offering more detailed " +
             "instructions on how to find a plane.")]
    public float OfferDetailedInstructionsDelay = 8.0f;

    /// <summary>
    /// The time to delay, after Unity Start, showing the plane discovery guide.
    /// </summary>
    private const float _onStartDelay = 1f;

    /// <summary>
    /// The time to delay, after a at least one plane is tracked by ARCore, hiding the plane discovery guide.
    /// </summary>
    private const float _hideGuideDelay = 0.75f;

    /// <summary>
    /// The duration of the hand animation fades.
    /// </summary>
    private const float _animationFadeDuration = 0.15f;

    /// <summary>
    /// The RawImage that provides rotating hand animation.
    /// </summary>
    [Tooltip("The RawImage that provides rotating hand animation.")]
    [FormerlySerializedAs("m_HandAnimation")]
    [SerializeField]
    private RawImage _handAnimation = null;
    
    /// <summary>
    /// The Game Object that contains the window with more instructions on how to find a plane.
    /// </summary>
    [Tooltip(
        "The Game Object that contains the window with more instructions on how to find " +
        "a plane.")]
    [FormerlySerializedAs("m_MoreHelpWindow")]
    [SerializeField]
    private GameObject _moreHelpWindow = null;

    /// <summary>
    /// The Game Object that contains the button to close the help window.
    /// </summary>
    [Tooltip("The Game Object that contains the button to close the help window.")]
    [FormerlySerializedAs("m_GotItButton")]
    [SerializeField]
    private Button _gotItButton = null;

    /// <summary>
    /// The elapsed time ARCore has been detecting at least one plane.
    /// </summary>
    private float _detectedPlaneElapsed;

    /// <summary>
    /// The elapsed time ARCore has been tracking but not detected any planes.
    /// </summary>
    private float _notDetectedPlaneElapsed;

    /// <summary>
    /// Indicates whether a lost tracking reason is displayed.
    /// </summary>
    private bool _isLostTrackingDisplayed;

    /// <summary>
    /// A list to hold detected planes ARCore is tracking in the current frame.
    /// </summary>
    private ARPlaneManager m_ARPlaneManager;


    // Start is called before the first frame update
    void Start()
    {
        _gotItButton.onClick.AddListener(OnGotButtonClicked);
        CheckFieldsAreNotNull();
        _moreHelpWindow.SetActive(false);
        _isLostTrackingDisplayed = false;
        _notDetectedPlaneElapsed = DisplayGuideDelay - _onStartDelay;
    }

    ///<summary>
    /// Unity's OnDestroy() method.
    ///</summary>
    public void OnDestroy()
    {
        _gotItButton.onClick.RemoveListener(OnGotButtonClicked);
    }

    ///<summary>
    /// Update is called once per frame
    ///</summary>

    /// <summary>
    /// Enable or Disable this Plane Discovery Guide.
    /// </summary>
    /// <param name="guideEnabled">Enable/Disable the guide.</param>
    void Update()
    {
        try
        {
            UpdateDetectedPlaneTrackingState();
            UpdateUI();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void EnablePlaneDiscoveryGuide(bool guideEnabled)
    {
        if (guideEnabled)
        {
            enabled = true;
        }
        else
        {
            enabled = false;
            _handAnimation.enabled = false;
        }
    }

    /// <summary>
    /// Callback executed when the got-it button has been clicked by the user.
    /// </summary>
    void OnGotButtonClicked()
    {
        _moreHelpWindow.SetActive(false);
        enabled = true;
    }
    // <summary>
    /// Checks the required fields are not null, and logs a Warning otherwise.
    /// </summary>
    private void CheckFieldsAreNotNull()
    {
        if (_moreHelpWindow == null)
        {
            Debug.LogError("MoreHelpWindow is null");
        }

        if (_gotItButton == null)
        {
            Debug.LogError("GotItButton is null");
        }

        if (_handAnimation == null)
        {
            Debug.LogError("HandAnimation is null");
        }
    }
    /// <summary>
    /// Checks whether at least one plane being actively tracked exists.
    /// </summary>
    void UpdateDetectedPlaneTrackingState()
    {
        if (ARSession.state == ARSessionState.SessionTracking)
        {
            return;
        }

        foreach (var plane in m_ARPlaneManager.trackables)
        {
            if (plane.trackingState == TrackingState.Tracking)
            {
                _detectedPlaneElapsed += Time.deltaTime;
                _notDetectedPlaneElapsed = 0f;
                return;
            }

            _detectedPlaneElapsed = 0f;
            _notDetectedPlaneElapsed += Time.deltaTime;
        }
    }

    /// <summary>
    /// Hides or shows the UI based on the existence of a plane being currently tracked.
    /// </summary>
    private void UpdateUI()
    {
        if (ARSession.notTrackingReason != NotTrackingReason.None)
        {
            _handAnimation.enabled = false;
            // mensajes en pantalla sobre la razon por que no se ve nada
            /*switch (ARSession.notTrackingReason)
            {
                
            }*/

            _isLostTrackingDisplayed = true;
            return;
        }else if (_isLostTrackingDisplayed)
        {
            // The session has moved from the lost tracking state.
            _isLostTrackingDisplayed = false;
        }

        if (_notDetectedPlaneElapsed > DisplayGuideDelay)
        {
            // The session has been tracking but no planes have been found by
            // 'DisplayGuideDelay'.
            if (!_handAnimation.enabled)
            {
                _handAnimation.GetComponent<CanvasRenderer>().SetAlpha(0f);
                _handAnimation.CrossFadeAlpha(1f, _animationFadeDuration, false);
            }

            _handAnimation.enabled = true;
        }else if (_notDetectedPlaneElapsed > 0f || _detectedPlaneElapsed > _hideGuideDelay)
        {
            // The session is tracking but no planes have been found in less than
            // 'DisplayGuideDelay' or at least one plane has been tracking for more than
            // '_hideGuideDelay'.
        }

        if (_handAnimation.enabled)
        {
            _handAnimation.GetComponent<CanvasRenderer>().SetAlpha(1f);
            _handAnimation.CrossFadeAlpha(0f, _animationFadeDuration, false);
        }

        _handAnimation.enabled = false;
    }
    private void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

   
}