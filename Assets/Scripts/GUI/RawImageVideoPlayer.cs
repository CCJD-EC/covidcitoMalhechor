using UnityEngine.XR.ARFoundation;

namespace covidcitoMalhechor.Scripts.GUI
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Video;

    /// <summary>
    /// Helper class that plays a video on a RawImage texture.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(VideoPlayer))]
    public class RawImageVideoPlayer : MonoBehaviour
    {
        /// <summary>
        /// The raw image where the video will be played.
        /// </summary>
        public RawImage RawImage;


        /// <summary>
        /// The ARSession of this scene
        /// </summary>
        [Tooltip("This is the ARSession object"), SerializeField]
        public ARSession m_Session;
        
        /// <summary>
        /// The video player component to be played.
        /// </summary>
        public VideoPlayer VideoPlayer;

        private Texture _rawImageTexture;

        /// <summary>
        /// The Unity Start() method.
        /// </summary>
        public void Start()
        {
            VideoPlayer.enabled = false;
            _rawImageTexture = RawImage.texture;
            VideoPlayer.prepareCompleted += PrepareCompleted;
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            if ((ARSession.state == ARSessionState.None) || 
                (ARSession.state == ARSessionState.CheckingAvailability))
            {
                VideoPlayer.Stop();
                return;
            }

            if (RawImage.enabled && !VideoPlayer.enabled)
            {
                VideoPlayer.enabled = true;
                VideoPlayer.Play();
            }
            else if (!RawImage.enabled && VideoPlayer.enabled)
            {
                // Stop video playback to save power usage.
                VideoPlayer.Stop();
                RawImage.texture = _rawImageTexture;
                VideoPlayer.enabled = false;
            }
        }

        private void PrepareCompleted(VideoPlayer player)
        {
            RawImage.texture = player.texture;
        }
    }
}
