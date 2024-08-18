using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using System.Linq;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using System.Runtime.ConstrainedExecution;

namespace GorillaCars
{


    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject CarGameObject;
        public AssetBundle bundle;
        public static Plugin Instance { get; set; }
        public GameObject Wheel;
        public static bool IsSteamVr;
        bool loading = true;
        public float X = 90;
        public float y = 90;
        public float z = 90;
        public GameObject assetloaderstand;
        public const float Tick = 1.0f / 4f;
        public PhotonNetworkController scoreBoard;
        string filespath = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "Custom Cars");
        public string[] cars;
        GameObject raycastsphere;
        public List<GameObject> carsGameobjs = new List<GameObject>();


        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void OnGameInitialized()
        {
            try
            {
                if (!Directory.Exists(filespath))
                {
                    Directory.CreateDirectory(filespath);

                }
            }
            catch
            {
                Debug.Log("error at file maker?");
            }
            try
            {
                cars = Directory.GetFiles(filespath, "*.car");

            }
            catch (Exception ex)
            {
                Debug.Log("OMG");
            }

            try
            {

                foreach (string carspath in cars)
                {

                        Debug.Log("Path: " + carspath);
                        CustomCarDescripter Descripter;
                        AssetBundle bundle = AssetBundle.LoadFromFile(carspath);
                        var car = Instantiate(bundle.LoadAsset<GameObject>("Car"));
                        carsGameobjs.Add(car);
                        Descripter = car.GetComponent<CustomCarDescripter>();
                        car.AddComponent<manager>();
                        car.GetComponentInChildren<Rigidbody>().isKinematic = true;
                        car.transform.position = Vector3.zero;
                        car.transform.GetChild(0).position = Vector3.zero;
                        bundle.Unload(false);
                    
                    
                }
            }
            catch
            {
                Debug.Log("Error? for some reason");
            }
            try
            {
                foreach (var car in carsGameobjs)
                {
                    BoxCollider[] Colliders = car.GetComponentsInChildren<BoxCollider>();
                    for (int i = 0; i < Colliders.Length; i++)
                    {
                        Debug.Log(Colliders[i].name);
                        if (Colliders[i].isTrigger)
                        {
                            Colliders[i].gameObject.AddComponent<ButtonManager>();
                        }
                    }
                }
               

            }
            catch (Exception e)
            {
                Debug.Log("Learn to code lmao");
            }

            var asserloader = LoadAssetBundle("GorillaCars.assets.assetloader");
            assetloaderstand = asserloader.LoadAsset<GameObject>("assetloader");
            assetloaderstand = GameObject.Instantiate(assetloaderstand);
            assetloaderstand.AddComponent<assestloaderstandcode>(); 
            assetloaderstand.transform.position = new Vector3(-65.6924f, 11.9549f, - 80.1833f);
            assetloaderstand.transform.rotation = Quaternion.Euler(359.4089f, 212.4161f, - 0.0003f);
            IsSteamVr = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";

            BoxCollider[] Collidersforstand = assetloaderstand.GetComponentsInChildren<BoxCollider>();
            for (int i = 0; i < Collidersforstand.Length; i++)
            {
                Debug.Log(Collidersforstand[i].name);
                if (Collidersforstand[i].isTrigger)
                {
                    Collidersforstand[i].gameObject.AddComponent<ButtonManager>();
                }
            }
            Debug.Log($"SteamVR: {IsSteamVr.ToString()}");

            Debug.Log("About to load asset");

            Debug.Log("Loaded AssetBundle");

            Debug.Log("Loaded Asset");
            try
            {
                foreach (var car in carsGameobjs)
                {
                    var Descripter = car.GetComponent<CustomCarDescripter>();
                    car.name = Descripter.Name;
                    car.GetComponent<manager>().CarDescriptor = Descripter;
                    car.GetComponent<manager>().Setup2();
                }
            }
            catch
            {
                Debug.Log("MAD skill issue");
            }

            Debug.Log("Instantiated carobject");

            Debug.Log("Made manager");

            Debug.Log("About to setup2");
            try
            {
                
            }
            catch
            {
                Debug.Log("cars may have no manager??");
            }

            Debug.Log("Setup2");
            gameObject.AddComponent<Moddedcheck>();
            try
            {
              
            }
            catch
            {
                Debug.Log("kinda dumb but gotta check");
            }

            if (PhotonNetwork.LocalPlayer.CustomProperties != null)
            {
                var HT = new ExitGames.Client.Photon.Hashtable();
                HT.AddOrUpdate("Sitting", false);
                PhotonNetwork.SetPlayerCustomProperties(HT);

            }
            var Networker = new GameObject("NetworkManager", typeof(NetworkingManager));

        }
        public void spawnsphere()
        {
            raycastsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(raycastsphere.GetComponent<SphereCollider>());
            raycastsphere.GetComponent<MeshRenderer>().material.shader = GorillaTagger.Instance.offlineVRRig.mainSkin.GetComponent<SkinnedMeshRenderer>().material.shader;
            raycastsphere.transform.localScale = new Vector3(.1f, .1f, .1f);
        }
        public void FixedUpdate()
        {
            try
            {
                if (raycastsphere == null)
                    spawnsphere();
                if (GorillaLocomotion.Player.Instance != null && Physics.Raycast(GorillaLocomotion.Player.Instance.leftControllerTransform.position, GorillaLocomotion.Player.Instance.leftControllerTransform.forward, out RaycastHit hit, 100))
                {
                    if (hit.collider != null)
                        raycastsphere.transform.position = hit.point;
                    else
                        raycastsphere.transform.position = Vector3.zero;
                }
                if (ControllerInputPoller.instance.leftControllerPrimaryButton)
                {
                    CarGameObject.transform.GetChild(0).position = raycastsphere.transform.position + new Vector3(0, 1.5f, 0);
                    CarGameObject.transform.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
                }
            }
            catch
            {
                Debug.Log("loading the sphereraycast blah blah balh");
            }
            
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
    }

}
