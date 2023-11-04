using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;

public class CarController : MonoBehaviourPun
{
    public float enginePower = 1500.0f;
    public float steer = 40.0f;
    public float maxTurbo = 100.0f;

    public GameObject camera;

    public float turbo = 0.0f;

    public float vertical_input = 0.0f;
    private float horizontal_input = 0.0f;

    // Wheels
    public WheelCollider Wheel_FL;
    public WheelCollider Wheel_FR;
    public WheelCollider Wheel_RL;
    public WheelCollider Wheel_RR;

    public Transform TWheel_FL;
    public Transform TWheel_FR;
    public Transform TWheel_RL;
    public Transform TWheel_RR;

    public float enginePowerReset;

    private string playerNickName;

    // Control laps
    private int laps;
    private Transform checkParent;
    private int number_checkpoints;
    private int lastCheckpoint;
    public int maxLaps;

    private int position = 1;

    void Awake()
    {
        enginePowerReset = enginePower;
        checkParent = GameObject.Find("Checkpoints").transform;
        number_checkpoints = checkParent.childCount;

        // Obtain max laps number
        GameObject raceController = GameObject.Find("GameManager");
        GameManager script = raceController.GetComponent<GameManager>();
        maxLaps = script.maxLaps;

        if (GetComponent<PhotonView>().IsMine)
        {
            camera.SetActive(true);
        }
        else
        {
            Debug.Log("Incorrect PhotonView");
        }
    }

    void Start()
    {

        laps = 0;
        playerNickName = photonView.Owner.NickName;
        lastCheckpoint = number_checkpoints;
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0f, -0.5f, 0.3f);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            vertical_input = Input.GetAxis("Vertical");
            horizontal_input = Input.GetAxis("Horizontal");

            Wheel_RR.motorTorque = enginePower * vertical_input;
            Wheel_RL.motorTorque = enginePower * vertical_input;


            HandBrake();
            Turbo();

            Wheel_FL.steerAngle = steer * horizontal_input;
            Wheel_FR.steerAngle = steer * horizontal_input;
            UpdateTransform();
        }
        else
        {
            camera.SetActive(false);
        }
        Position();
    }


    void Turbo()
    {
        if (Input.GetKey("space"))
        {
            if (turbo > 0f)
            {
                Wheel_RR.motorTorque += 10000f;
                Wheel_RL.motorTorque += 10000f;
                turbo -= Time.deltaTime * 50f;
            }
            else
            {
                turbo = 0f;
            }

        }
    }

    void HandBrake()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Wheel_RL.brakeTorque = enginePower * 1000f;
            Wheel_RR.brakeTorque = enginePower * 1000f;
            Wheel_RL.motorTorque = 0f;
            Wheel_RR.motorTorque = 0f;
        }
        else
        {
            Wheel_RR.brakeTorque = 0f;
            Wheel_RL.brakeTorque = 0f;
        }
    }

    void UpdateTransform()
    {
        TWheel_FL.Rotate(Wheel_FL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        TWheel_FR.Rotate(Wheel_FR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        TWheel_RL.Rotate(Wheel_RL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        TWheel_RR.Rotate(Wheel_RL.rpm / 60 * 360 * Time.deltaTime, 0, 0);

        Vector3 flwheel = TWheel_FL.localEulerAngles;
        Vector3 frwheel = TWheel_FR.localEulerAngles;

        flwheel.y = Wheel_FL.steerAngle - (TWheel_FL.localEulerAngles.z);
        TWheel_FL.localEulerAngles = flwheel;
        frwheel.y = Wheel_FR.steerAngle - (TWheel_FR.localEulerAngles.z);
        TWheel_FR.localEulerAngles = frwheel;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Turbo")
        {
            turbo = maxTurbo;
            Destroy(col.gameObject);
            return;
        }

        // Check the checkpoints to complete a lap
        if (col.gameObject.name == "cp1")
        {
            if (lastCheckpoint == number_checkpoints)
            {
                laps++;
                lastCheckpoint = 1;
            }
            return;
        }

        if (col.gameObject.name == ("cp" + (lastCheckpoint + 1)))
        {
            lastCheckpoint++;
            return;
        }
    }

    // When the game starts, the car can move
    public void ResetPower()
    {
        enginePower = enginePowerReset;
    }

    // Controll the position
    public int getPosition()
    {
        return position;
    }

    public int getLap()
    {
        return laps;
    }

    public int getLastCP()
    {
        return lastCheckpoint;
    }

    public float getDistanceCP()
    {
        string nameCP = "cp1";
        if (lastCheckpoint != number_checkpoints)
        {
            nameCP = "cp" + (lastCheckpoint + 1);
        }
        GameObject checkpoint = GameObject.Find(nameCP);
        float dist = Vector3.Distance(transform.position, checkpoint.transform.position);
        return dist;
    }

    public void Position()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");

        int newPosition = 1;
        foreach (GameObject car in cars)
        {

            CarController script = car.GetComponent<CarController>();
            int lapOpponent = script.getLap();
            int lastCPOpponent = script.getLastCP();
            int positionOpponent = script.getPosition();
            float distanceOpponent = script.getDistanceCP();
            float myPosition = getDistanceCP();
            

            if (lapOpponent > laps)
            {
                newPosition++;
            }
            else if (lapOpponent == laps && lastCPOpponent > lastCheckpoint)
            {
                newPosition++;
            }
            else if (lapOpponent == laps && lastCPOpponent == lastCheckpoint && myPosition > distanceOpponent)
            {
                newPosition++;
            }            
        }
        position = newPosition;
        Debug.Log("" + laps + " " + lastCheckpoint + " " + getDistanceCP());
    }

    public string getNickName()
    {
        return playerNickName;
    }

}
