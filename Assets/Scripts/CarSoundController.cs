using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CarSoundController : MonoBehaviourPunCallbacks
{
    private AudioSource carSound;
    private CarController carController;
    public float modifier = 1.0f;
    public float soundPitch = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        carSound = GetComponent<AudioSource>();
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        //if the car is acelerating, then increment the pitch
       if (carController.vertical_input != 0.0)
        {
            soundPitch += 0.004f;
        }
        else
        {
            soundPitch -= 0.02f;
        }
        
    
        if(soundPitch < 0.1f) //Minimum pitch
        {
            soundPitch = 0.1f;
        }else if (soundPitch > 0.8f) //Maximum pitch
        {
            soundPitch = 0.8f;
        }

        carSound.pitch = soundPitch * modifier;
    }
}
