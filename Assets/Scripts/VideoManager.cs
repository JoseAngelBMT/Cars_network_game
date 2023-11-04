using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using agora_gaming_rtc;
using Photon.Pun;

#if (UNITY_2018_3_OR_NEWER)
using UnityEngine.Android;
#endif

public class VideoManager : MonoBehaviour
{
    [Header("Agora Settings")]
    //The AppID of the Agora Project, from the Dashboard
    // Get your own App ID at https://dashboard.agora.io/
    public string appId;

    [Header("Scene References")]
    public GameObject videoUI;
    public InputField channelField;
    public Button joinButton;
    public Text buttonText;
    public GameObject localPlayer;
    public VideoSurface localPlayerVS;

    // The Agora chat engine
    private IRtcEngine mRtcEngine = null;
    private string channelName = "";
    private uint localPlayerUid = 0;
    private uint remotePlayerUid = 0;
    private GameObject remotePlayer = null;


    private void Start()
    {

#if (UNITY_2018_3_OR_NEWER)
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif

        // Initializes the IRtcEngine
        Debug.Log("Init Engine");
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // Add our callbacks to handle Agora events
        mRtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
        mRtcEngine.OnUserJoined += OnUserJoined;
        mRtcEngine.OnLeaveChannel += OnLeaveChannel;
        mRtcEngine.OnUserOffline += OnUserOffline;

        // Enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);

        //var photon = GetComponent<PhotonNetwork>();
        channelName = PhotonNetwork.CurrentRoom.Name;
        //channelField.text = "DefaultChannel";
        // Add the listener to the join button to allow the player to join the channel.
        joinButton.onClick.AddListener(JoinChannel);
        localPlayer.SetActive(false);
        localPlayerVS.SetEnable(false);
    }


    void OnApplicationQuit()
    {
        Debug.Log("Quit");
        DestroyRemotePlayer();
        DestroyRtcEngine();
    }

    void OnDestroy()
    {
        Debug.Log("Destroy");
        DestroyRemotePlayer();
        DestroyRtcEngine();
    }

    private void DestroyRemotePlayer()
    {
        if (remotePlayer != null)
        {
            Destroy(remotePlayer);
            remotePlayer = null;
        }
    }

    private void DestroyRtcEngine()
    {
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    // Handles joining the channel when the button is pressed
    private void JoinChannel()
    {
        //channelName = channelField.text;
        Debug.Log("Joining channel " + channelName);

        if (mRtcEngine == null)
            return;

        // The call starts in video mode
        mRtcEngine.EnableVideo();
        // To get video raw data, call both EnableVideo and EnableVideoObserver
        mRtcEngine.EnableVideoObserver();

        joinButton.onClick.RemoveListener(JoinChannel);
        joinButton.onClick.AddListener(LeaveChannel);
        buttonText.text = "Leave";

        // Join channel
        mRtcEngine.JoinChannel(channelName, null, 0);

        // Optional: if a data stream is required, here is a good place to create it
        //int streamID = mRtcEngine.CreateDataStream(true, true);
        //Debug.Log("Data stream created, id = " + streamID);
    }

    // Handles leaving the channel when the button is pressed
    private void LeaveChannel()
    {
        Debug.LogFormat("Leaving channel");

        joinButton.onClick.RemoveListener(LeaveChannel);
        joinButton.onClick.AddListener(JoinChannel);
        buttonText.text = "Join";

        mRtcEngine.LeaveChannel();

        // To stop getting video raw data, call both DisableVideo and DisableVideoObserver
        mRtcEngine.DisableVideo();
        mRtcEngine.DisableVideoObserver();

        DestroyRemotePlayer();
        remotePlayerUid = 0;
    }

    // Called when the local user joins the channel
    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("Local user joined with uid " + uid);
        localPlayerUid = uid;

        localPlayer.SetActive(true);
        // Set to zero to show local input
        localPlayerVS.SetForUser(0);
        localPlayerVS.SetEnable(true);
    }

    // Called when a remote user joins the channel
    private void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("Remote user joined with uid " + uid);

        if (remotePlayer != null)
            return;

        remotePlayer = GameObject.CreatePrimitive(PrimitiveType.Quad);
        if (remotePlayer != null)
        {
            remotePlayerUid = uid;
            remotePlayer.transform.SetParent(videoUI.transform, false);
            remotePlayer.name = uid.ToString();
            remotePlayer.transform.localPosition = new Vector3(-125.0f, 140.0f, 0.0f);
            remotePlayer.transform.Rotate(0.0f, 0.0f, 180.0f);
            remotePlayer.transform.localScale = new Vector3(2.0f, 1.5f, 1.0f);
            remotePlayer.AddComponent<RawImage>();
            VideoSurface remotePlayerVS = remotePlayer.AddComponent<VideoSurface>();
            // Set the remote video
            remotePlayerVS.SetForUser(uid);
            remotePlayerVS.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            // Adjust the video refreshing frame rate. Default is 15 fps.
            //remoteVideoSurface.SetGameFps(30);
            remotePlayerVS.SetEnable(true);
        }
    }

    // Called when the local user leaves the channel
    private void OnLeaveChannel(RtcStats stats)
    {
        Debug.Log("Local user leaves the channel");
        localPlayerVS.SetEnable(false);
        localPlayer.SetActive(false);
    }

    // Called when a remote user leaves the channel
    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        Debug.Log("Remote user with uid " + uid + " leaves the channel");

        if (remotePlayerUid == uid)
        {
            remotePlayerUid = 0;
            DestroyRemotePlayer();
        }
    }
}
