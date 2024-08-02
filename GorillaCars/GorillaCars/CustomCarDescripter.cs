using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CustomCarDescripter : MonoBehaviour
{
    [Header("WheelColiders")]
    public WheelCollider LeftFront;
    public WheelCollider RightFront;
    public WheelCollider RearLeft;
    public WheelCollider RearRight;

    [Header("ModStuff")]
    public string Name;
    public string Author;
    public string Description;

    [Header("WheelMeshes")]
    public GameObject LeftFrontwheel;
    public GameObject RightFrontwheel;
    public GameObject RearLeftwheel;
    public GameObject RearRightwheel;

    [Header("CarSettings")]
    public float accleractionforce;
    public float breakforce;
    

    [Header("Seats")]
    public GameObject DriverSeat;
    public GameObject PassengerSeat;
    public GameObject BackDriverSide;
    public GameObject BackPassengerSide;

    [Header("Doors")]
    public GameObject DoorDriverSeat;
    public GameObject DoorPassengerSeat;
    public GameObject DoorBackDriverSide;
    public GameObject DoorBackPassengerSide;

    [Header("CarComponets")]
    public GameObject poweroncar;
    public GameObject drive;
    public GameObject reverse;
    

    [Header("AudioStuff")]
    public AudioSource EngineStart;
    public AudioSource EngineStop;
    public AudioSource EngineLoop;

    [Header("Radio")]
    public GameObject Radio;


}