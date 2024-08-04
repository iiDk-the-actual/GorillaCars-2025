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
       
        public float X = 90;
        public float y = 90;
        public float z = 90;
        public const float Tick = 1.0f / 4f;
        public PhotonNetworkController scoreBoard;
        string filespath = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "Custom Cars");
        public string[] cars;
        private void OnGUI()
        {
            GUILayout.Label("Custom Properties"); // wryser prob added this
            GUILayout.BeginArea(new Rect(10, 10, Screen.width, 500));
            if (PhotonNetwork.InRoom)
            {
                GUILayout.BeginVertical();
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    GUILayout.Label(player.NickName + player.CustomProperties.ToString());
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }

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
            if(!Directory.Exists(filespath))
            {
                Directory.CreateDirectory(filespath);
                
            }
            cars = Directory.GetFiles(filespath, "*.car");
            CustomCarDescripter Descripter;
            for (int i = 0; i < cars.Length; i++)
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(cars[i]);
                CarGameObject = Instantiate(bundle.LoadAsset<GameObject>("Car"));
                Descripter = CarGameObject.GetComponent<CustomCarDescripter>();
            }
            try
            {
                BoxCollider[] Colliders = CarGameObject.GetComponentsInChildren<BoxCollider>();
                for (int i = 0; i < Colliders.Length; i++)
                {
                    Debug.Log(Colliders[i].name);
                    if (Colliders[i].isTrigger)
                    {
                        Colliders[i].gameObject.AddComponent<ButtonManager>();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Learn to code lmao");
            }
            
            IsSteamVr = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
            Debug.Log($"SteamVR: {IsSteamVr.ToString()}");

            Debug.Log("About to load asset");
         
            Debug.Log("Loaded AssetBundle");
        
            Debug.Log("Loaded Asset");

            
            Debug.Log("Instantiated carobject");
            Descripter = CarGameObject.GetComponent<CustomCarDescripter>();
            CarGameObject.name = Descripter.Name;
            CarGameObject.transform.GetChild(0).position = new Vector3(-64.4182f, 2.3273f, -71.1818f);
            Debug.Log("Set name, position");

            CarGameObject.transform.GetChild(0).AddComponent<manager>();
            Debug.Log("Made manager");

            Debug.Log("About to setup2");
            manager.Instance.Setup2();
            Debug.Log("Setup2");
            if (PhotonNetwork.LocalPlayer.CustomProperties != null)
            {
                var HT = new ExitGames.Client.Photon.Hashtable();
                HT.AddOrUpdate("Sitting", false);
                HT.AddOrUpdate("carname", CarGameObject.name);
                PhotonNetwork.SetPlayerCustomProperties(HT);
              
            }
            gameObject.AddComponent<NetThingyWOOHOOO>();
        }

    }
}
