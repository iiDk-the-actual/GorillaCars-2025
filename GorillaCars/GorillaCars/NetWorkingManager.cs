using GorillaCars;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
public class NetWorkManager : MonoBehaviourPunCallbacks
{
    public List<PlayerCarWrapped> PlayerAndCar = new List<PlayerCarWrapped>();

    public class PlayerCarWrapped
    {
        public Player playerWithCar;
        public GameObject Car;
        public VRRig rig;

        public void UpdateCarState()
        {
            Hashtable Table = playerWithCar.CustomProperties;
            if ((bool)Table[GorillaCars.Const.SittingProp])
            {
                Car.transform.parent = rig.transform;
                Car.transform.localPosition = new Vector3(0.2f, -0.9f, -0.3f);
                Car.transform.localRotation = Quaternion.Euler(0f, 359.6664f, 0f);
                Destroy(Car.transform.GetChild(0).GetComponent<Rigidbody>());
            }
            else
            {
                Car.transform.parent = null;
            }
        }

        public void GetRidOfME()
        {
            Destroy(Car);
        }
    }

    public override void OnJoinedRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            SetUpCar(player);
        }
    }

    void SetUpCar(Player player)
    {
        
        if (player.CustomProperties.ContainsKey(Const.SittingProp) && player != PhotonNetwork.LocalPlayer)
        {
            if (Plugin.Instance.CarGameObject.name == (string)player.CustomProperties[Const.CarNameProp])
            {
                GameObject PlayerCar = Instantiate(Plugin.Instance.CarGameObject);
                PlayerCar.transform.position = new Vector3(-64.4182f, 2.3273f, -71.1818f);
                PlayerCar.name = player.NickName + "'S: Car";

                Destroy(PlayerCar.transform.GetChild(0).GetComponent<Rigidbody>());
                Destroy(PlayerCar.GetComponentInChildren<LocalCarManager>());
                var rig = GorillaGameManager.StaticFindRigForPlayer(player);
                PlayerCarWrapped pcw = new PlayerCarWrapped()
                {
                    playerWithCar = player,
                    Car = PlayerCar,
                    rig = rig
                };
                PlayerAndCar.Add(pcw);
                pcw.UpdateCarState();
            }
        }
    }
    void OnDestroy()
    {
        foreach (PlayerCarWrapped PlayerWCar in PlayerAndCar)
        {
            PlayerWCar.GetRidOfME();
        }
    }
   public override void OnPlayerEnteredRoom(Player newPlayer)
   {
        SetUpCar(newPlayer);
   }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (PlayerCarWrapped PlayerWCar in PlayerAndCar)
        {
            if (PlayerWCar.playerWithCar == otherPlayer)
            {
                PlayerWCar.GetRidOfME();
                PlayerAndCar.Remove(PlayerWCar);
            }
        }
    }
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(Const.SittingProp))
        {
            foreach (PlayerCarWrapped PlayerWCar in PlayerAndCar)
            {
                if (PlayerWCar.playerWithCar == targetPlayer)
                {
                    PlayerWCar.UpdateCarState();
                }
            }
        }
    }

}