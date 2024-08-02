using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using System.IO;
using UnityEngine;
using Photon.Pun;

namespace GorillaCars
{


    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject CarGameObject;
        public static Plugin Instance { get; set; }
        public static bool IsSteamVr;

        public float X = 90;
        public float y = 90;
        public float z = 90;
        public const float Tick = 1.0f / 4f;
        public PhotonNetworkController scoreBoard;
        string filespath = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "Custom Cars");
        public string[] cars;

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
            if (!Directory.Exists(filespath))
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
            BoxCollider[] Colliders = CarGameObject.GetComponentsInChildren<BoxCollider>();
            for (int i = 0; i < Colliders.Length; i++)
            {
                Debug.Log(Colliders[i].name);
                if (Colliders[i].isTrigger)
                {
                    Colliders[i].gameObject.AddComponent<ButtonManager>();
                }
            }

            IsSteamVr = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";

            Descripter = CarGameObject.GetComponent<CustomCarDescripter>();
            CarGameObject.name = Descripter.Name;
            CarGameObject.transform.GetChild(0).position = new Vector3(-64.4182f, 2.3273f, -71.1818f);

            CarGameObject.AddComponent<LocalCarManager>();

            LocalCarManager.Instance.Setup2();
            if (PhotonNetwork.LocalPlayer.CustomProperties != null)
            {
                var HT = new ExitGames.Client.Photon.Hashtable();
                HT.AddOrUpdate(Const.SittingProp, false);
                HT.AddOrUpdate(Const.CarNameProp, CarGameObject.name);
                PhotonNetwork.SetPlayerCustomProperties(HT);
            }
        }

    }
}
