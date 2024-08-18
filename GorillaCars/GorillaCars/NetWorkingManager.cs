using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GorillaCars
{
    public class NetworkingManager : MonoBehaviourPunCallbacks
    {
        public Dictionary<Player, GameObject> PlayerList = new Dictionary<Player, GameObject>();
        

        public override void OnJoinedRoom()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if(!p.IsLocal)
                {
                    try
                    {
                        if (p.CustomProperties.ContainsKey("Sitting"))
                        {
                            var car = GameObject.Instantiate(Plugin.Instance.CarGameObject);
                            car.name = PhotonNetwork.NickName + ": " + Plugin.Instance.CarGameObject.name;
                            Destroy(car.GetComponentInChildren<Rigidbody>());
                            Destroy(car.GetComponentInChildren<manager>());
                            PlayerList.Add(p, car);
                            car.transform.position = Plugin.Instance.CarGameObject.transform.position;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("get a car bro :skull:");
                    }
                    if (p.CustomProperties["sitting"] != null)
                    {
                        if ((bool)p.CustomProperties["Sitting"])
                        {
                            var rig = GorillaGameManager.StaticFindRigForPlayer(p);
                            foreach (GameObject cars in PlayerList.Values)
                            {

                                cars.transform.parent = PlayerList[p].transform;
                                cars.transform.GetChild(0).localPosition = new Vector3(-65.1881f, 1.3421f, - 71.4834f);
                                cars.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 84.4751f, 0f);

                            }
                        }
                        else
                        {
                            PlayerList[p].transform.parent = null;
                        }
                    }
                    
                }
                

            }


        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(PlayerList[otherPlayer]);
            PlayerList.Remove(otherPlayer);
            
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {

            if (!newPlayer.IsLocal)
            {
                if (newPlayer.CustomProperties["Sitting"] != null)
                {
                    if (newPlayer.CustomProperties.ContainsKey("Sitting"))
                    { 
                        var car = GameObject.Instantiate(Plugin.Instance.CarGameObject);
                        car.name = PhotonNetwork.NickName + ": " + Plugin.Instance.CarGameObject.name;
                        Destroy(car.GetComponentInChildren<Rigidbody>());
                        Destroy(car.GetComponentInChildren<manager>());
                        PlayerList.Add(newPlayer, car);
                        car.transform.position = Plugin.Instance.CarGameObject.transform.position;
                    }
                    if ((bool)newPlayer.CustomProperties["Sitting"])
                    {
                        var rig = GorillaGameManager.StaticFindRigForPlayer(newPlayer);
                         PlayerList[newPlayer].transform.parent = rig.transform;
                        PlayerList[newPlayer].transform.GetChild(0).localPosition = new Vector3(-65.1881f, 1.3421f, -71.4834f);
                        PlayerList[newPlayer].transform.GetChild(0).localRotation = Quaternion.Euler(0f, 84.4751f, 0f);


                    }
                    else
                    {
                        PlayerList[newPlayer].transform.parent = null;
                    }
                }
                
            }
                
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!targetPlayer.IsLocal)
            {
               
                if (changedProps["Sitting"] != null)
                {
                    var rig = GorillaGameManager.StaticFindRigForPlayer(targetPlayer);
                    if ((bool)changedProps["Sitting"])
                    {
                        PlayerList[targetPlayer].transform.parent = rig.transform;
                        PlayerList[targetPlayer].transform.GetChild(0).localPosition = new Vector3(-65.1881f, 1.3421f, -71.4834f);
                        PlayerList[targetPlayer].transform.GetChild(0).localRotation = Quaternion.Euler(0f, 84.4751f, 0f);
                    }
                    else
                    {
                        PlayerList[targetPlayer].transform.parent = null;
                    }
                }
            }
           
            

        }
        public override void OnLeftRoom()
        {
            foreach (GameObject cars in PlayerList.Values)
            {
                Destroy(cars);
            }
            PlayerList.Clear();

        }
    }
}
