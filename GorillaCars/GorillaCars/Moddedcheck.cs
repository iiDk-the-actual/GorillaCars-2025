using GorillaGameModes;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GorillaCars
{
    public class Moddedcheck : MonoBehaviourPunCallbacks
    {
        public static Moddedcheck modcheck;
        public object gameMode;
        public static int layer = 29, layerMask = 1 << layer;
        private LayerMask baseMask;
        public void Start()
        {
            modcheck = this;
            baseMask = GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers;
        }
        public bool IsModded()
        {
                return PhotonNetwork.InRoom && gameMode.ToString().Contains("MODDED");
        }

        public override void OnJoinedRoom()
        {

            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out gameMode);
            if(!IsModded())
            {
                foreach (var cars in Plugin.Instance.carsGameobjs)
                {
                    cars.gameObject.SetActive(false);
                    cars.GetComponent<manager>().sitting = false;
                    GorillaTagger.Instance.mainCamera.transform.parent.rotation = Quaternion.Euler(0f, 47.9593f, 0f);
                    
                    GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers = baseMask;
                    GorillaLocomotion.GTPlayer.Instance.bodyCollider.isTrigger = false;
                    GorillaLocomotion.GTPlayer.Instance.headCollider.isTrigger = false;
                }
            }
            else
            {
                foreach (var cars in Plugin.Instance.carsGameobjs)
                {
                    cars.gameObject.SetActive(true);
                    
                }
            }
        }
    }
}
