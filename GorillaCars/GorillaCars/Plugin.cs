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

namespace GorillaCars
{
    public class NetThingyWOOHOOO : MonoBehaviourPunCallbacks
    {
        public override void OnJoinedRoom()
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
            {
                ExitGames.Client.Photon.Hashtable hashthingy = player.CustomProperties;
                if (hashthingy.ContainsKey("carX"))
                {
                    GameObject asset = Plugin.Instance.bundle.LoadAsset<GameObject>("car");
                    GameObject garn = Instantiate(asset);
                    garn.transform.position = new Vector3((float)hashthingy["carX"], (float)hashthingy["carY"], (float)hashthingy["carZ"]);
                    garn.transform.rotation = Quaternion.Euler((float)hashthingy["carRotX"], (float)hashthingy["carRotY"], (float)hashthingy["carRotZ"]);
                    Plugin.Instance.playerWithCarYeah.Add(player, garn);
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ExitGames.Client.Photon.Hashtable hashthingy = newPlayer.CustomProperties;
            if (hashthingy.ContainsKey("carX"))
            {
                GameObject asset = Plugin.Instance.bundle.LoadAsset<GameObject>("car");
                GameObject garn = Instantiate(asset);
                garn.transform.position = new Vector3((float)hashthingy["carX"], (float)hashthingy["carY"], (float)hashthingy["carZ"]);
                garn.transform.rotation = Quaternion.Euler((float)hashthingy["carRotX"], (float)hashthingy["carRotY"], (float)hashthingy["carRotZ"]);
                Plugin.Instance.playerWithCarYeah.Add(newPlayer, garn);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (Plugin.Instance.playerWithCarYeah.ContainsKey(otherPlayer))
            {
                Destroy(Plugin.Instance.playerWithCarYeah[otherPlayer]);
                Plugin.Instance.playerWithCarYeah.Remove(otherPlayer);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (changedProps.ContainsKey("carX"))
            {
                GameObject garn = Plugin.Instance.playerWithCarYeah[targetPlayer];
                garn.transform.position = new Vector3(Mathf.Lerp((float)changedProps["carX"], garn.transform.position.x, 0.6f), Mathf.Lerp((float)changedProps["carY"], garn.transform.position.y, 0.6f), Mathf.Lerp((float)changedProps["carZ"], garn.transform.position.z, 0.6f));
                garn.transform.rotation = Quaternion.Euler(Mathf.Lerp((float)changedProps["carRotX"], garn.transform.rotation.x, 0.6f), (Mathf.Lerp((float)changedProps["carRotY"], garn.transform.rotation.y, 0.6f)), Mathf.Lerp((float)changedProps["carRotZ"], garn.transform.rotation.z, 0.6f));
            }
        }
    }

    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject CarGameObject;
        public AssetBundle bundle;
        public static Plugin Instance { get; set; }
        public GameObject Wheel;
        public static bool IsSteamVr;
        public Dictionary<Player, GameObject> playerWithCarYeah = new Dictionary<Player, GameObject>();

        public const float Tick = 1.0f / 4f;
        float lastTime;

        bool inRoom = true;

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
            IsSteamVr = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
            Debug.Log($"SteamVR: {IsSteamVr.ToString()}");

            Debug.Log("About to load asset");
            bundle = LoadAssetBundle("GorillaCars.assets.car");
            Debug.Log("Loaded AssetBundle");
            CarGameObject = bundle.LoadAsset<GameObject>("car");
            Debug.Log("Loaded Asset");

            CarGameObject = GameObject.Instantiate(CarGameObject);
            Debug.Log("Instantiated carobject");
            CarGameObject.name = "CarGameObject";
            CarGameObject.transform.position = new Vector3(-64.4182f, 2.3273f, -71.1818f);
            Debug.Log("Set name, position");

            Wheel = CarGameObject.transform.FindChildRecursive("st_wheel").gameObject;
            Debug.Log("Found st_wheel");

            BoxCollider[] Colliders = CarGameObject.GetComponentsInChildren<BoxCollider>();
            for (int i = 0; i < Colliders.Length; i++)
            {
                if (Colliders[i].isTrigger)
                {
                    Colliders[i].gameObject.AddComponent<ButtonManager>();
                }
            }

            // shits hittin flips again -wryser
            MeshCollider[] a = CarGameObject.GetComponentsInChildren<MeshCollider>();
            for(int i = 0; i < a.Length; i++)
            {
                Destroy(a[i]);
            }
            Debug.Log("Destroyed MeshColliders and added ButtonManager to BoxColliders with isTrigger");

            CarGameObject.AddComponent<manager>();
            Debug.Log("Made manager");

            Debug.Log("About to setup2");
            manager.Instance.Setup2();
            Debug.Log("Setup2");

            gameObject.AddComponent<NetThingyWOOHOOO>();

            bool[] whatWentWrong = new bool[] { SetProperty("carX", CarGameObject.transform.position.x), SetProperty("carY", CarGameObject.transform.position.y) , SetProperty("carZ", CarGameObject.transform.position.z) };
            bool[] whatWentWrong2 = new bool[] { SetProperty("carRotX", CarGameObject.transform.rotation.x), SetProperty("carRotY", CarGameObject.transform.rotation.y) , SetProperty("carRotZ", CarGameObject.transform.rotation.z) };

            for (int i = 0; i < whatWentWrong.Length; i++)
            {
                if (!whatWentWrong[i] || !whatWentWrong2[i])
                {
                    Debug.LogError($"whatWentWrong or whatWentWrong2 had made an error on index [{i}]");
                    break;
                }
            }
        }

        public bool SetProperty(string key, object value)
        {
            try
            {
                PhotonNetwork.LocalPlayer.CustomProperties.AddOrUpdate(key, value);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }

        void Update()
        {
            if (PhotonNetwork.InRoom && Time.time > lastTime + Tick)
            {   
                lastTime = Time.time;
                var ht = PhotonNetwork.LocalPlayer.CustomProperties;
                bool moved = CarGameObject.transform.position != new Vector3((float)ht["carX"], (float)ht["carY"], (float)ht["carZ"]); 
                bool rotated = CarGameObject.transform.rotation != Quaternion.Euler((float)ht["carRotX"], (float)ht["carRotY"], (float)ht["carRotZ"]);
                if (moved)
                {
                    ht.AddOrUpdate("carX", CarGameObject.transform.position.x);
                    ht.AddOrUpdate("carY", CarGameObject.transform.position.y);
                    ht.AddOrUpdate("carZ", CarGameObject.transform.position.z);
                }
                if (rotated)
                {
                    ht.AddOrUpdate("carRotX", CarGameObject.transform.rotation.x);
                    ht.AddOrUpdate("carRotY", CarGameObject.transform.rotation.y);
                    ht.AddOrUpdate("carRotZ", CarGameObject.transform.rotation.z);
                }
            }
        }
    }
}
