using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Mono.Security.X509.X520;

namespace GorillaCars
{
    public class NetworkingManager : MonoBehaviourPunCallbacks
    {
        public Dictionary<Player, GameObject> PlayerList = new Dictionary<Player, GameObject>();
        public override void OnJoinedRoom()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                var rig = GorillaGameManager.instance.FindPlayerVRRig(player);
                if (player.CustomProperties.ContainsKey("Sitting") && !player.IsLocal)
                {
                    Debug.Log(player.NickName + ": Has Joined Room");
                    if (player.CustomProperties.TryGetValue("CarName", out object carname))
                    {
                        if (Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault())
                        {
                            GameObject carobj = Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault();
                            var car = GameObject.Instantiate(carobj);
                            car.name = player.NickName + ": " + carobj.name;
                            PlayerList.Add(player, car);
                            car.transform.position = Plugin.Instance.CarGameObject.transform.position;
                        }
                    }
                    if ((bool)player.CustomProperties["Sitting"])
                    {
                        PlayerList[player].transform.parent = rig.transform;
                        PlayerList[player].transform.GetChild(0).localPosition = rig.transform.position;
                        PlayerList[player].transform.GetChild(0).localRotation = rig.transform.rotation;

                    }
                    else
                    {
                        PlayerList[player].transform.parent = null;
                    }
                }
                else
                {
                    Debug.Log("==================================== Player does not have car mod silly ====================================");
                }
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer.CustomProperties.ContainsKey("Sitting") && !newPlayer.IsLocal)
            {
                var rig = GorillaGameManager.instance.FindPlayerVRRig(newPlayer);
                Debug.Log(newPlayer.NickName + ": Has Entered Room");
                if (newPlayer.CustomProperties.TryGetValue("CarName", out object carname))
                {
                    if (Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault())
                    {
                        GameObject carobj = Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault();
                        var car = GameObject.Instantiate(carobj);
                        car.name = newPlayer.NickName + ": " + carobj.name;
                        PlayerList.Add(newPlayer, car);
                        car.transform.position = Plugin.Instance.CarGameObject.transform.position;
                    }
                }
                if ((bool)newPlayer.CustomProperties["Sitting"])
                {
                    PlayerList[newPlayer].transform.parent = rig.transform;
                    PlayerList[newPlayer].transform.GetChild(0).localPosition = rig.transform.position;
                    PlayerList[newPlayer].transform.GetChild(0).localRotation = rig.transform.rotation;

                }
                else
                {
                    PlayerList[newPlayer].transform.parent = null;
                }
            }
            else
            {
                Debug.Log("==================================== newPlayer does not have car mod silly ====================================");

            }

        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            try
            {
                if (PlayerList.ContainsKey(otherPlayer))
                {
                    Destroy(PlayerList[otherPlayer]);
                    PlayerList.Remove(otherPlayer);
                }
            }
            catch
            {
                Debug.Log(otherPlayer + "has left but there was an error");
            }



        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {

            if (!targetPlayer.IsLocal)
            {
                if (PlayerList.ContainsKey(targetPlayer))
                {
                    try
                    {
                        if (changedProps.TryGetValue("Sitting", out object yippie))
                        {
                            var rig =  GorillaGameManager.instance.FindPlayerVRRig(targetPlayer);
                           
                            if ((bool)changedProps["Sitting"])
                            {
                                PlayerList[targetPlayer].transform.parent = rig.transform;
                                PlayerList[targetPlayer].transform.GetChild(0).localPosition = rig.transform.position;
                                PlayerList[targetPlayer].transform.GetChild(0).localRotation = rig.transform.rotation;

                            }
                            else
                            {
                                PlayerList[targetPlayer].transform.parent = null;
                            }
                        }

                    }
                    catch
                    {
                        Debug.Log("idek");
                    }


                    try
                    {
                        if (targetPlayer.CustomProperties.ContainsKey("CarName"))
                        {
                            Debug.Log(targetPlayer.NickName + ": props updated");
                            if (changedProps.TryGetValue("CarName", out object carname))
                            {

                                if (Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault())
                                {
                                    Destroy(PlayerList[targetPlayer]);
                                    GameObject carobj = Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault();
                                    var car = GameObject.Instantiate(carobj);
                                    car.name = targetPlayer.NickName + ": " + carobj.name;
                                    PlayerList.AddOrUpdate(targetPlayer, car);
                                    car.transform.localPosition = PlayerList[targetPlayer].transform.FindChildRecursive("DriverSeat").position;
                                    car.transform.localRotation = Quaternion.Euler(0, 0, 0);
                                }
                                else
                                {
                                    Destroy(PlayerList[targetPlayer]);
                                }
                            }
                        }
                    }
                    catch
                    {
                        Debug.Log("Error with diffrent cars?");
                    }

                }
            }
        }



        public override void OnLeftRoom()
        {
            try
            {
                foreach (GameObject cars in PlayerList.Values)
                {
                    Destroy(cars);
                }
                PlayerList.Clear();
            }
            catch
            {
                Debug.Log("dawg if this called you suck");
            }



        }
    }
}
