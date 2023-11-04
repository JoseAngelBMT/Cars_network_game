using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class SelectCarManager : MonoBehaviourPunCallbacks
{
    public GameObject UI;
    public GameObject buttonyellow;
    public GameObject buttonred;
    public GameObject buttonblack;


    [Space(5)]
    public GameObject[] spawn;

    [Space(5)]
    private GameObject player;

    private string roomInformation = "";
    private float timer = 0;

    void Start()
    {
        ActiveButtons();

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Launcher");
            return;
        }
       
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("Launcher");
        }

        if(roomInformation != "")
        {
            timer += Time.deltaTime;
            if (timer > 10f)
            {
                roomInformation = "";
                timer = 0;
            }
        }
        else
        {
            timer = 0;
        }
        

    }

    // Photon Methods
    public override void OnPlayerLeftRoom(Player other)
    {
        roomInformation = other.NickName + " leave the room";
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        roomInformation = other.NickName + " joined in the room";
    }


    void ActiveButtons()
    {
        Button btny = buttonyellow.GetComponent<Button>();
        btny.onClick.AddListener(SelectYellow);

        Button btnr = buttonred.GetComponent<Button>();
        btnr.onClick.AddListener(SelectRed);

        Button btnb = buttonblack.GetComponent<Button>();
        btnb.onClick.AddListener(SelectBlack);
    }

    public void SelectYellow()
    {
        int i = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.LocalPlayer) break;
            i++;
        }

        UI.SetActive(false);
        player = PhotonNetwork.Instantiate("Car_yellow", spawn[i].transform.position, spawn[i].transform.rotation, 0);
    }

    public void SelectRed()
    {
        int i = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.LocalPlayer) break;
            i++;
        }

        UI.SetActive(false);
        player = PhotonNetwork.Instantiate("Car_red", spawn[i].transform.position, spawn[i].transform.rotation, 0);
    }

    public void SelectBlack()
    {
        int i = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.LocalPlayer) break;
            i++;
        }

        UI.SetActive(false);
        player = PhotonNetwork.Instantiate("Car_black", spawn[i].transform.position, spawn[i].transform.rotation, 0);
        Destroy(spawn[i]);
    }


    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.red;
        GUI.Label(new Rect(Screen.width/2 - 300, 20, 300, 100), roomInformation, style);
    }




}