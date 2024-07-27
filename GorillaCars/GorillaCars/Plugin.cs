using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using Utilla;

namespace GorillaCars
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject CarGameObject;
        public static Plugin ThisScript;
        public GameObject Wheel;
        public static bool IsSteamVr;
        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }
        void OnGameInitialized(object sender, EventArgs e)
        {
            IsSteamVr = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
            var bundle = LoadAssetBundle("GorillaCars.assets.car");
            CarGameObject = bundle.LoadAsset<GameObject>("car");
            CarGameObject = GameObject.Instantiate(CarGameObject);
            CarGameObject.name = "CarGameObject";
            ThisScript = this;
            CarGameObject.transform.position = new Vector3(-64.4182f, 2.3273f, -71.1818f);
            Wheel = CarGameObject.transform.FindChildRecursive("st_wheel").gameObject;
            BoxCollider[] Colliders = CarGameObject.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider collider in Colliders)
            {
                if (collider.isTrigger)
                {
                    collider.gameObject.AddComponent<ButtonManager>();
                }

            }
            CarGameObject.AddComponent<manager>();
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

        }
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {

        }
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {

        }
    }
}
