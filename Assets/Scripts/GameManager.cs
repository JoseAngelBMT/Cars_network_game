using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviourPun
{
    public int maxLaps = 3;

    private int numberCars = 0;
    private int players = 0;
    

    private bool start = false;
    private int start_seconds = 3;
    private bool ready = false;

    private GameObject[] cars;

    private string winner;
    private bool isWinner = false;
    private bool isEnd = false;

    private GameObject trafficLight;
    // Start is called before the first frame update
    void Start()
    {
        trafficLight = GameObject.Find("trafficLight");
    }

    // Update is called once per frame
    void Update()
    {
        cars = GameObject.FindGameObjectsWithTag("Car");
        if (!start)
        {
            //cars = GameObject.FindGameObjectsWithTag("Car");
            stopCars();
            ready = playersConnected();
            if (ready)
            {
                countdownSound();
                StartCoroutine(Countdown(start_seconds));
                start = true;
            }
        }
        else
        {
            getWinner();
        }        
    }

    bool playersConnected()
    {

        int count = cars.Length;
        players = PhotonNetwork.CurrentRoom.PlayerCount;
        numberCars = count;
        if (PhotonNetwork.CurrentRoom.PlayerCount == count) return true;

        return false;
    }

    void countdownSound()
    {
        AudioSource sound = trafficLight.GetComponent<AudioSource>();
        sound.Play();
    }

    void OnGUI()
    {
        Font myFont = (Font)Resources.Load("Fonts/ShoottoKill", typeof(Font));
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 140;
        guiStyle.normal.textColor = Color.black;
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.font = myFont;
        int width = 150;
        int height = 150;
        if (ready)
        {
            if (start_seconds > 0)
            {
                GUI.Label(new Rect((Screen.width-width) / 2, (Screen.height-height) / 2, width, height), "" + (start_seconds), guiStyle);
            }
            if (start_seconds == 0)
            {
                GUI.Label(new Rect((Screen.width-width) / 2, (Screen.height-height) / 2, width, height), "GO!", guiStyle);
            }
            else if (start_seconds == -1)
            {
                GUI.Label(new Rect((Screen.width -width)/ 2, (Screen.height-height) / 2, width, height), "", guiStyle);
            }
        }
        if (isWinner)
        {
            guiStyle.fontSize = 60;
            GUI.Label(new Rect((Screen.width - width) / 2f, (Screen.height - height) / 10f, width, height), "Winner "+winner, guiStyle);
        }
        
        
    }

    IEnumerator Countdown(int seconds)
    {
        int count = seconds;

        while (count > -1)
        {

            start_seconds = count;
            yield return new WaitForSeconds(1);
            //start_seconds = seconds;
            count--;
        }

        // count down is finished...
        start_seconds = -1;
        startCars();
        
    }

    void stopCars()
    {
        foreach (GameObject car in cars)
        {

            CarController script = car.GetComponent<CarController>();
            if (script.enginePower != 0.0f)
            {
                script.enginePowerReset = script.enginePower;
                script.enginePower = 0.0f;
            }       
        }
    }

    void startCars()
    {
        // Set trafficlights green
        var trafficRenderer = trafficLight.GetComponent<Renderer>();
        var trafficMaterials = trafficRenderer.materials;
        var material = trafficMaterials[1];
        material.SetColor("_EmissionColor", Color.green);

        foreach (GameObject car in cars)
        {
            CarController script = car.GetComponent<CarController>();
            script.ResetPower();
        }
    }

    void getWinner()
    {
        foreach(GameObject car in cars)
        {
            CarController script = car.GetComponent<CarController>();
            int lap = script.getLap();
            if (lap == maxLaps+1)
            {
                winner = script.getNickName();
                isWinner = true;
            }
        }
        if (isWinner && !isEnd)
        {
            isEnd = true;
            StartCoroutine(countToEnd());
            
        }

    }

    void endGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Launcher");
    }

    IEnumerator countToEnd ()
    {
        yield return new WaitForSeconds(5);
        endGame();
    }

}
