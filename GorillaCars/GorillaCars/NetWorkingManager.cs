using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using UnityEngine;
using static Mono.Security.X509.X520;

namespace GorillaCars
{
    public class NetworkingManager : MonoBehaviourPunCallbacks
    {
        public Dictionary<Player, GameObject> PlayerList = new Dictionary<Player, GameObject>();
        

        public override void OnJoinedRoom()
        {
            try
            {
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (!p.IsLocal)
                    {
                        try
                        {
                            if (p.CustomProperties.TryGetValue("Sitting", out object yippeitworks))
                            {
                                if (p.CustomProperties.ContainsKey("Sitting"))
                                {
                                    PlayerList.Add(p, null);
                                    Debug.Log(p.CustomProperties.TryGetValue("CarName", out object carname2));
                                    if (p.CustomProperties.TryGetValue("CarName", out object carname))
                                    {
                                        GameObject carobj = Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault();
                                        var car = GameObject.Instantiate(carobj);
                                        car.name = p.NickName + ": " + carobj.name;
                                        Destroy(car.GetComponentInChildren<Rigidbody>());
                                        Destroy(car.GetComponentInChildren<manager>());
                                        PlayerList.AddOrUpdate(p, car);
                                        car.transform.position = Plugin.Instance.CarGameObject.transform.position;
                                    }

                                        
                                }
                            }
                               
                        }
                        catch (Exception e)
                        {
                            Debug.Log("get a car bro 💀");
                        }
                        if (p.CustomProperties["sitting"] != null)
                        {
                            if ((bool)p.CustomProperties["Sitting"])
                            {
                                var rig = GorillaGameManager.StaticFindRigForPlayer(p);
                                foreach (GameObject cars in PlayerList.Values)
                                {

                                    cars.transform.parent = PlayerList[p].transform;
                                    cars.transform.GetChild(0).localPosition = new Vector3(-65.1881f, 1.3421f, -71.4834f);
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
            catch
            {
                Debug.Log("Error with OnJoinedRoom");
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
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            try
            {
                if (!newPlayer.IsLocal)
                {
                    if (newPlayer.CustomProperties.TryGetValue("Sitting", out object yippeitworks))
                    {
                        if (newPlayer.CustomProperties.ContainsKey("Sitting"))
                        {
                            PlayerList.Add(newPlayer, null);
                            Debug.Log(newPlayer.CustomProperties.TryGetValue("CarName", out object carname2));
                            if (newPlayer.CustomProperties.TryGetValue("CarName", out object carname))
                            {
                                GameObject carobj = Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault();
                                var car = GameObject.Instantiate(carobj);
                                car.name = newPlayer.NickName + ": " + carobj.name;
                                Destroy(car.GetComponentInChildren<Rigidbody>());
                                Destroy(car.GetComponentInChildren<manager>());
                                PlayerList.AddOrUpdate(newPlayer, car);
                                car.transform.position = Plugin.Instance.CarGameObject.transform.position;
                            }
                                
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
            catch
            {
                Debug.Log("error with " + newPlayer);
            }
           
                
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            try
            {
                if (!targetPlayer.IsLocal)
                {
                    if (PlayerList.ContainsKey(targetPlayer))
                    {
                        if (changedProps.TryGetValue("Sitting", out object yippeitworks))
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
                        if (changedProps.TryGetValue("CarName", out object carname))
                        {
                            if (PhotonNetwork.InRoom)
                            {
                                if (Plugin.Instance.carsGameobjs.Contains(carname))
                                {
                                    Destroy(PlayerList[targetPlayer]);
                                    GameObject carobj = Plugin.Instance.carsGameobjs.Where(obj => obj.name == carname.ToString()).SingleOrDefault();
                                    var car = GameObject.Instantiate(carobj);
                                    car.name = targetPlayer.NickName + ": " + carobj.name;
                                    Destroy(car.GetComponentInChildren<Rigidbody>());
                                    Destroy(car.GetComponentInChildren<manager>());
                                    PlayerList.AddOrUpdate(targetPlayer, car);
                                    car.transform.position = Plugin.Instance.CarGameObject.transform.position;
                                }

                            }




                        }
                    }



                }
            }
            catch
            {
                Debug.Log("props error");
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
