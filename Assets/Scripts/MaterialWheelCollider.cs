using UnityEngine;
using System.Collections;

public class MaterialWheelCollider : MonoBehaviour
{
    private WheelCollider wheel;

    void Start()
    {
        wheel = GetComponent<WheelCollider>();
    }
    // static friction of the ground material.
    void FixedUpdate()
    {
        WheelHit hit;

        if (wheel.GetGroundHit(out hit))
        {
            float friction = hit.collider.material.staticFriction;


            if (friction == 10.0f)
            {
                wheel.motorTorque = Input.GetAxis("Vertical") * 250.0f;
            }
            else
            {
                WheelFrictionCurve fFriction = wheel.forwardFriction;
                fFriction.stiffness = friction;
                wheel.forwardFriction = fFriction;

                WheelFrictionCurve sFriction = wheel.sidewaysFriction;
                sFriction.stiffness = friction;
                wheel.sidewaysFriction = sFriction;
            }
        }
    }
}
