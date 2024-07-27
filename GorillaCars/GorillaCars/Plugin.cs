using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using Utilla;
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
                    GameObject garn = Plugin.Instance.bundle.LoadAsset<GameObject>("car");
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
                GameObject garn = Plugin.Instance.bundle.LoadAsset<GameObject>("car");
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

    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
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

        bool inRoom;

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

            bundle = LoadAssetBundle("GorillaCars.assets.car");
            CarGameObject = bundle.LoadAsset<GameObject>("car");

            CarGameObject = GameObject.Instantiate(CarGameObject);
            CarGameObject.name = "CarGameObject";
            CarGameObject.transform.position = new Vector3(-64.4182f, 2.3273f, -71.1818f);

            var table = PhotonNetwork.LocalPlayer.CustomProperties;
            table.AddOrUpdate("carX", CarGameObject.transform.position.x);
            table.AddOrUpdate("carY", CarGameObject.transform.position.y);
            table.AddOrUpdate("carZ", CarGameObject.transform.position.z);
            table.AddOrUpdate("carRotX", CarGameObject.transform.rotation.x);
            table.AddOrUpdate("carRotY", CarGameObject.transform.rotation.y);
            table.AddOrUpdate("carRotZ", CarGameObject.transform.rotation.z);

            Wheel = CarGameObject.transform.FindChildRecursive("st_wheel").gameObject;

            BoxCollider[] Colliders = CarGameObject.GetComponentsInChildren<BoxCollider>();
            Colliders.Where(col => col.isTrigger)
                     .ToList()
                     .ForEach(col => col.gameObject.AddComponent<ButtonManager>());

            CarGameObject.AddComponent<manager>();

            GorillaTagger.Instance.rigidbody.drag = 0;

            manager.Instance.Setup2();
            manager.Instance.UndoMySetup();

            gameObject.AddComponent<NetThingyWOOHOOO>();

        }
        void OnDisable()
        {
            manager.Instance.UndoMySetup();
            inRoom = false;
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
            if (inRoom && Time.time > lastTime + Tick)
            {   
                lastTime = Time.time;
                ExitGames.Client.Photon.Hashtable ht = PhotonNetwork.LocalPlayer.CustomProperties;
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

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            manager.Instance.Setup();
            inRoom = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            manager.Instance.UndoMySetup();
            inRoom = false;
        }
    }
}
