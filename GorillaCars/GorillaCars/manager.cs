using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using UnityEngine.InputSystem;

namespace GorillaCars
{
    public class manager : MonoBehaviour
    {
        public static manager Instance { get; set; }

        bool IsCarOn;

        private float touchTime = 0f;
        private const float debounceTime = 0.25f;

        private float touchTime1 = 0f;
        private const float debounceTime1 = 0.25f;

        bool sitting;

        public WheelCollider frontleft;
        public WheelCollider frontright;
        public WheelCollider backleft;
        public WheelCollider backright;

        public float acclration = 500f;
        public float breakforce = 200f;
        public float curacceleration = 500f;
        public float curbreakforce = 200f;

        Vector2 leftStick;
        bool Drving = true;

        public static int layer = 29, layerMask = 1 << layer;
        private LayerMask baseMask;
        private const float horizontalMultiplier = 60f, verticalMultiplier = 50f;

        GameObject raycastsphere;

        Transform frontLeftWheel;
        Transform frontRightWheel;
        Transform rearLeftWheel;
        Transform rearRightWheel;

        GameObject WhatTheFuckIsThisCube; // copyrighted by biotest05

        bool setup;
        bool guiEnabled;

        void Awake()
        {
            Instance = this;
        }

        public void Setup2()
        {
            try
            {
                frontleft = transform.FindChildRecursive("FrontLeftCollider").GetComponent<WheelCollider>();
                frontLeftWheel = transform.FindChildRecursive("wheel_front_left");

                frontright = transform.FindChildRecursive("FrontRightCollider").GetComponent<WheelCollider>();
                frontRightWheel = transform.FindChildRecursive("wheel_front_right");

                backleft = transform.FindChildRecursive("BackLeftCollider").GetComponent<WheelCollider>();
                rearLeftWheel = transform.FindChildRecursive("wheel_rear_left");

                backright = transform.FindChildRecursive("BackRightCollider").GetComponent<WheelCollider>();
                rearRightWheel = transform.FindChildRecursive("wheel_rear_right");
            }
            catch
            {
                Debug.LogError("the wheel colliders are shit!!");
            }

            WhatTheFuckIsThisCube = Plugin.Instance.CarGameObject.transform.Find("Cube").gameObject;
            WhatTheFuckIsThisCube.transform.localPosition = new Vector3(-0.3969f, -0.74f, 0.481f);

            try
            {
                raycastsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(raycastsphere.GetComponent<SphereCollider>());
                raycastsphere.GetComponent<MeshRenderer>().material.shader = GorillaTagger.Instance.offlineVRRig.mainSkin.GetComponent<SkinnedMeshRenderer>().material.shader;
                raycastsphere.transform.localScale = new Vector3(.1f, .1f, .1f);
            }
            catch
            {
                Debug.LogError("Fuck you raycastsphere.");
            }
        }

        public void Setup()
        {
            Plugin.Instance.CarGameObject.SetActive(true);

            setup = true;
        }

        public void UndoMySetup()
        {
            Plugin.Instance.CarGameObject.SetActive(false);

            setup = false;
        }

        public void FixedUpdate()
        {
            if (raycastsphere == null)
                Setup2();
            if (GorillaLocomotion.Player.Instance != null && Physics.Raycast(GorillaLocomotion.Player.Instance.leftControllerTransform.position, GorillaLocomotion.Player.Instance.leftControllerTransform.forward, out RaycastHit hit, 100))
            {
                if (hit.collider != null)
                    raycastsphere.transform.position = hit.point;
            }
        }

        public void Update()
        {
            if (Keyboard.current.vKey.wasPressedThisFrame) guiEnabled = !guiEnabled;

            if (setup)
            {
                if (ControllerInputPoller.instance.leftControllerPrimaryButton)
                {
                    Plugin.Instance.CarGameObject.transform.position = raycastsphere.transform.position + new Vector3(0, 1.5f, 0);
                    Plugin.Instance.CarGameObject.transform.localRotation = Quaternion.Euler(new Vector3(leftStick.x, 0, 0));
                    Plugin.Instance.CarGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                if (Plugin.IsSteamVr)
                {
                    leftStick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
                }
                else
                {
                    ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out leftStick);
                }
                Plugin.Instance.Wheel.transform.rotation = Quaternion.Euler(0, 0, GorillaTagger.Instance.leftHandTransform.position.x * 360);

                if (sitting)
                {
                    GorillaTagger.Instance.rigidbody.drag = 60000;
                    GorillaTagger.Instance.mainCamera.transform.parent.transform.position = WhatTheFuckIsThisCube.transform.position;
                    GorillaTagger.Instance.mainCamera.transform.parent.rotation = WhatTheFuckIsThisCube.transform.rotation;
                }
                else
                {
                    GorillaTagger.Instance.rigidbody.drag = 0;
                }
                if (Drving)
                {
                    backleft.motorTorque = ControllerInputPoller.instance.leftControllerIndexFloat * 700;
                    backright.motorTorque = ControllerInputPoller.instance.leftControllerIndexFloat * 700;
                }
                else if (!Drving)
                {
                    backleft.motorTorque = -ControllerInputPoller.instance.leftControllerIndexFloat * 700;
                    backright.motorTorque = -ControllerInputPoller.instance.leftControllerIndexFloat * 700;
                }

                frontleft.brakeTorque = ControllerInputPoller.instance.rightControllerGripFloat * 200;
                frontright.brakeTorque = ControllerInputPoller.instance.rightControllerGripFloat * 200;
                backleft.brakeTorque = ControllerInputPoller.instance.leftControllerGripFloat * 200;
                backright.brakeTorque = ControllerInputPoller.instance.leftControllerGripFloat * 200;

                frontleft.steerAngle = leftStick.x * 35f;
                frontright.steerAngle = leftStick.x * 35f;

                Updatewheel(frontleft, frontLeftWheel);
                Updatewheel(frontright, frontRightWheel);
                Updatewheel(backright, rearRightWheel);
                Updatewheel(backleft, rearLeftWheel);
            }
        }

        public void Updatewheel(WheelCollider collider, Transform wheel)
        {
            collider.GetWorldPose(out Vector3 wheeltrans, out Quaternion wheelrot);
            wheel.transform.position = wheeltrans;
            wheel.transform.rotation = wheelrot;
        }

        void OnGUI()
        {
            if (guiEnabled)
            {
                GUILayout.Label("garn47");
                GUILayout.BeginArea(new Rect(10, 10, Screen.width, 500));
                GUILayout.Label("frontL wheel motorTorque: " + frontleft.motorTorque.ToString());
                GUILayout.Label("frontR wheel motorTorque: " + frontright.motorTorque.ToString());
                GUILayout.Label("backL wheel motorTorque: " + backleft.motorTorque.ToString());
                GUILayout.Label("backR wheel motorTorque: " + backright.motorTorque.ToString());
                GUILayout.EndArea();
            }
        }
        public void clicked(string BtnName)
        {
            switch (BtnName)
            {
                case "door_lf":
                    if (touchTime + debounceTime >= Time.time)
                    {
                        if (!sitting)
                        {
                            if (ControllerInputPoller.instance.rightControllerGripFloat > 0f || ControllerInputPoller.instance.leftControllerGripFloat > 0f)
                            {
                                sitting = true;
                                baseMask = GorillaLocomotion.Player.Instance.locomotionEnabledLayers;
                                GorillaLocomotion.Player.Instance.locomotionEnabledLayers = layerMask;
                                GorillaLocomotion.Player.Instance.bodyCollider.isTrigger = true;
                                GorillaLocomotion.Player.Instance.headCollider.isTrigger = true;
                                GorillaTagger.Instance.mainCamera.transform.parent.rotation = WhatTheFuckIsThisCube.transform.rotation;
                            }
                        }
                        else if (sitting)
                        {
                            if (ControllerInputPoller.instance.rightControllerGripFloat > 0f || ControllerInputPoller.instance.leftControllerGripFloat > 0f)
                            {
                                GorillaTagger.Instance.mainCamera.transform.parent.rotation = Quaternion.Euler(0f, 47.9593f, 0f);
                                sitting = false;
                                GorillaLocomotion.Player.Instance.locomotionEnabledLayers = baseMask;
                                GorillaLocomotion.Player.Instance.bodyCollider.isTrigger = false;
                                GorillaLocomotion.Player.Instance.headCollider.isTrigger = false;
                                Patches.TeleportPatch.TeleportPlayer(Plugin.Instance.CarGameObject.transform.position, Plugin.Instance.CarGameObject.transform.rotation.x, true);
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
                            Plugin.Instance.CarGameObject.transform.FindChildRecursive("PowerOnCar").GetComponent<MeshRenderer>().material.color = Color.green;
                            Plugin.Instance.CarGameObject.transform.FindChildRecursive("Engine Start").GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            IsCarOn = false;
                            Plugin.Instance.CarGameObject.transform.FindChildRecursive("PowerOnCar").GetComponent<MeshRenderer>().material.color = Color.red;
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
