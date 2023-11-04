using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SphereController : MonoBehaviourPun
{
    public GameObject stonePrefab;
    public GameObject wheelPrefab;
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {

        if (col.gameObject.tag == "Drop")
        {
            /*System.Random random = new System.Random();
            int number = random.Next(0, 2);
            switch (number)
            {
                case 0:
                    rocks();
                    break;
                case 1:
                    wheels();
                    break;
            }*/
            rocks();
            Destroy(col.gameObject);
            return;
        }

        if (col.gameObject.tag == "Brake")
        {
            bool brake = brakeOpponent();
            if (brake) Destroy(col.gameObject);
            return;
        }
    }

    void rocks()
    {
        Transform carTransform = gameObject.transform;
        Vector3 pos = carTransform.position;
        pos.y += 8;
        Instantiate(stonePrefab, pos, carTransform.rotation, carTransform);
    }

    void wheels()
    {
        Transform carTransform = gameObject.transform;
        Vector3 pos = carTransform.position;
        for (int i = 0; i < 10; i++)
        {
            pos.y += i + 5;
            Instantiate(wheelPrefab, pos, carTransform.rotation, carTransform);
        }

    }

    //Brake forward opponent
    bool brakeOpponent()
    {
        CarController script = GetComponent<CarController>();
        int position = script.getPosition();
        position--;
        if (position == 0) return false;

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        foreach (GameObject car in cars)
        {
            CarController scriptOpponent = car.GetComponent<CarController>();
            int positionOpponent = scriptOpponent.getPosition();
            if (positionOpponent == position)
            {
                Rigidbody m_Rigidbody = car.GetComponent<Rigidbody>();
                m_Rigidbody.constraints = RigidbodyConstraints.FreezePosition;  //Brake car
                m_Rigidbody.constraints = RigidbodyConstraints.None;            //Can move again
            }
        }
        return true;
    }
}
