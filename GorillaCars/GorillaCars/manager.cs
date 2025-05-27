using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using UnityEngine.InputSystem;
using Photon.Pun;
using GorillaNetworking;
using ExitGames.Client.Photon;
using TMPro;
using System.Linq;
using UnityEngine.XR;

namespace GorillaCars
{
    public class manager : MonoBehaviourPunCallbacks
    {
        public static manager Instance { get; set; }

        bool IsCarOn;

        private float touchTime = 0f;
        private const float debounceTime = 0.25f;

        private float touchTime1 = 0f;
        private const float debounceTime1 = 0.25f;
        
        public bool sitting = false;

        public WheelCollider frontleft;
        public WheelCollider frontright;
        public WheelCollider backleft;
        public WheelCollider backright;

        public float acclration = 500f;
        public float breakforce = 200f;

        Vector2 leftStick;
        bool Drving = true;

        public static int layer = 29, layerMask = 1 << layer;
        private LayerMask baseMask;
        private const float horizontalMultiplier = 60f, verticalMultiplier = 50f;

        Transform frontLeftWheel;
        Transform frontRightWheel;
        Transform rearLeftWheel;
        Transform rearRightWheel;

        public GameObject driver;
        public GameObject passenger;
        public GameObject backdriver;
        public GameObject backpassenger;
        GameObject PowerOnCar;
        GameObject EngineStart;
        GameObject EngineStop;
        GameObject EngineLoop;
        public CustomCarDescripter CarDescriptor;
            
        public void Setup2()
        {
            try
            {
                
               
                frontleft = CarDescriptor.LeftFront;
                frontLeftWheel = CarDescriptor.LeftFrontwheel.transform;

                frontright = CarDescriptor.RightFront;
                frontRightWheel = CarDescriptor.RightFrontwheel.transform;

                backleft = CarDescriptor.RearLeft;
                rearLeftWheel = CarDescriptor.RearLeftwheel.transform;

                backright = CarDescriptor.RearRight;
                rearRightWheel = CarDescriptor.RearLeftwheel.transform;


            }
            catch
            {
                Debug.LogError("the wheel colliders are shit!!");
            }
            try
            {
                acclration = CarDescriptor.accleractionforce;
                breakforce = CarDescriptor.breakforce;
                driver = CarDescriptor.DriverSeat;
                passenger = CarDescriptor.PassengerSeat;
                backdriver = CarDescriptor.BackDriverSide;
                backpassenger = CarDescriptor.BackPassengerSide;

                CarDescriptor.DoorDriverSeat.name = "DriverSeat";
                CarDescriptor.DoorPassengerSeat.name = "PassengerSeat";
                CarDescriptor.DoorBackDriverSide.name = "BackDriverSeat";
                CarDescriptor.DoorBackPassengerSide.name = "BackPassengerSeat";

                PowerOnCar = CarDescriptor.poweroncar;
                PowerOnCar.name = "PowerOnCar";
                EngineStart = CarDescriptor.EngineStart.gameObject;
                EngineStop = CarDescriptor.EngineStop.gameObject;
                EngineLoop = CarDescriptor.EngineLoop.gameObject;

            }
            catch
            {
                Debug.Log("SKILL ISSUE");
            }
            var HT = new ExitGames.Client.Photon.Hashtable();
            HT.AddOrUpdate("CarName", Plugin.Instance.CarGameObject.name);
            PhotonNetwork.SetPlayerCustomProperties(HT);
        }

        public void Setup()
        {
            gameObject.SetActive(true);

            // setup = true;
        }

        public void UndoMySetup()
        {
            gameObject.SetActive(false);

            // setup = false;
        }

        public void Update()
        {
            try
            {
                if (Plugin.IsSteamVr)
                {
                    leftStick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
                }
                else
                {
                    ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out leftStick);
                }
            }
            catch
            {
                Debug.Log("WHAT");
            }
           

            if (sitting)
            {
                
                GorillaTagger.Instance.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                GorillaLocomotion.GTPlayer.Instance.transform.position = driver.transform.position;
               GorillaTagger.Instance.rigidbody.position = Vector3.zero;
               
            }
            try
            {
                if (Drving)
                {
                    backleft.motorTorque = ControllerInputPoller.instance.leftControllerIndexFloat * acclration;
                    backright.motorTorque = ControllerInputPoller.instance.leftControllerIndexFloat * acclration;

                }
                else if (!Drving)
                {
                    backleft.motorTorque = -ControllerInputPoller.instance.leftControllerIndexFloat * acclration;
                    backright.motorTorque = -ControllerInputPoller.instance.leftControllerIndexFloat * acclration;
                }

                frontleft.brakeTorque = ControllerInputPoller.instance.rightControllerGripFloat * breakforce;
                frontright.brakeTorque = ControllerInputPoller.instance.rightControllerGripFloat * breakforce;
                backleft.brakeTorque = ControllerInputPoller.instance.leftControllerGripFloat * breakforce;
                backright.brakeTorque = ControllerInputPoller.instance.leftControllerGripFloat * breakforce;

                frontleft.steerAngle = leftStick.x * 35f;
                frontright.steerAngle = leftStick.x * 35f;

                Updatewheel(frontleft, frontLeftWheel);
                Updatewheel(frontright, frontRightWheel);
                Updatewheel(backright, rearRightWheel);
                Updatewheel(backleft, rearLeftWheel);
            }
            catch
            {
                Debug.Log("error with car desctor");
            }

           
        }

        public void Updatewheel(WheelCollider collider, Transform wheel)
        {
            collider.GetWorldPose(out Vector3 wheeltrans, out Quaternion wheelrot);
            wheel.transform.position = wheeltrans;
            wheel.transform.rotation = wheelrot;
        }

        public void clicked(string BtnName)
        {
            switch (BtnName)
            {
                case "DriverSeat":
                    if (touchTime + debounceTime >= Time.time)
                    {
                        if (!sitting)
                        {
                            if (ControllerInputPoller.instance.rightControllerGripFloat > 0f || ControllerInputPoller.instance.leftControllerGripFloat > 0f)
                           {
                                sitting = true;
                                baseMask = GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers;
                                GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers = layerMask;
                                GorillaLocomotion.GTPlayer.Instance.bodyCollider.isTrigger = true;
                                GorillaLocomotion.GTPlayer.Instance.headCollider.isTrigger = true;
                             

                                if (PhotonNetwork.LocalPlayer.CustomProperties != null)
                                {
                                    var HT = new ExitGames.Client.Photon.Hashtable();
                                    HT.AddOrUpdate("Sitting", true);
                                    PhotonNetwork.SetPlayerCustomProperties(HT);

                                }


                            }
                        }
                        else if (sitting)
                        {
                            if (ControllerInputPoller.instance.rightControllerGripFloat > 0f || ControllerInputPoller.instance.leftControllerGripFloat > 0f)
                            {
                                sitting = false;
                                GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers = baseMask;
                                GorillaLocomotion.GTPlayer.Instance.bodyCollider.isTrigger = false;
                                GorillaLocomotion.GTPlayer.Instance.headCollider.isTrigger = false;
                              
                                if (PhotonNetwork.LocalPlayer.CustomProperties != null)
                                {
                                    var HT = new ExitGames.Client.Photon.Hashtable();
                                    HT.AddOrUpdate("Sitting", false);
                                    PhotonNetwork.SetPlayerCustomProperties(HT);

                                }

                            }
                           
                        }




                    }
                    touchTime = Time.time;
                    break;

                case "PowerOnCar":
                    if (touchTime1 + debounceTime1 >= Time.time)
                    {
                        if (!IsCarOn)
                        {
                            IsCarOn = true;
                            PowerOnCar.GetComponent<MeshRenderer>().material.color = Color.green;
                            EngineStart.GetComponent<AudioSource>().Play();
                            EngineLoop.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            IsCarOn = false;
                            PowerOnCar.GetComponent<MeshRenderer>().material.color = Color.red;
                            EngineLoop.GetComponent<AudioSource>().Stop();
                            EngineStart.GetComponent<AudioSource>().Stop();
                            EngineStop.GetComponent<AudioSource>().Play();
                        }
                    }
                    touchTime1 = Time.time;
                    break;
                case "Drive":
                    Drving = true;
                    break;
                case "Rerverse":
                    Drving = false;
                    break;

            }
        }
    }
}
