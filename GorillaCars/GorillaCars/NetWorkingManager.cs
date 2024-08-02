using GorillaCars;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;

public class NetThingyWOOHOOO : MonoBehaviourPunCallbacks
{
    public Dictionary<Player, GameObject> playerWithCarYeah = new Dictionary<Player, GameObject>();
    public override void OnJoinedRoom()
    {
        try
        {

            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {

                ExitGames.Client.Photon.Hashtable hashthingy = player.CustomProperties;
                if (hashthingy.ContainsKey("Sitting") && player != PhotonNetwork.LocalPlayer)
                {
                    if (Plugin.Instance.CarGameObject.name == (string)hashthingy["carname"])
                    {
                        GameObject asset = Plugin.Instance.CarGameObject;
                        Debug.Log(player.ToString());
                        GameObject garn = Instantiate(asset);
                        garn = asset.transform.gameObject;
                        garn.transform.position = new Vector3(-64.4182f, 2.3273f, -71.1818f);
                        garn.name = player.NickName + "'S: Car";

                        Destroy(garn.GetComponent<Rigidbody>());
                        Destroy(garn.GetComponent<manager>());
                        var rig = GorillaGameManager.StaticFindRigForPlayer(player);
                        playerWithCarYeah.Add(player, garn);

                        if ((bool)hashthingy["Sitting"])
                        {
                            garn.transform.parent = rig.transform;
                            garn.transform.localPosition = new Vector3(0.2f, -0.9f, -0.3f);
                            garn.transform.localRotation = Quaternion.Euler(0f, 359.6664f, 0f);
                        }
                        else
                        {
                            garn.transform.parent = null;
                        }

                    }


                }
            }
        }
        catch
        {
            Debug.Log("error on joined room");
        }



    }
    void OnDestroy()
    {
        foreach (GameObject cars in playerWithCarYeah.Values)
        {
            Destroy(cars);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        try
        {
            ExitGames.Client.Photon.Hashtable hashthingy = newPlayer.CustomProperties;
            if (hashthingy.ContainsKey("Sitting"))
            {
                if (newPlayer != PhotonNetwork.LocalPlayer)
                {
                    if (Plugin.Instance.CarGameObject.name == (string)hashthingy["carname"])
                    {
                        GameObject asset = Plugin.Instance.CarGameObject;
                        GameObject garn = Instantiate(asset);
                        garn = asset.transform.GetChild(0).gameObject;

                        garn.transform.position = new Vector3(-64.4182f, 2.3273f, -71.1818f);
                        garn.name = newPlayer.NickName + "'S: Car";
                        Destroy(garn.GetComponent<Rigidbody>());
                        Destroy(garn.GetComponent<manager>());
                        var rig = GorillaGameManager.StaticFindRigForPlayer(newPlayer);
                        playerWithCarYeah.Add(newPlayer, garn);
                        if ((bool)hashthingy["Sitting"])
                        {
                            garn.transform.parent = rig.transform;
                            garn.transform.localPosition = new Vector3(0.2f, -0.9f, -0.3f);
                            garn.transform.localRotation = Quaternion.Euler(0f, 359.6664f, 0f);
                        }
                        else
                        {
                            garn.transform.parent = null;
                        }

                    }
                }

            }
        }
        catch
        {
            Debug.Log("ERROR CASUED AT OnPlayerEnteredRoom");
        }




    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        try
        {
            if (playerWithCarYeah.ContainsKey(otherPlayer))
            {
                Destroy(playerWithCarYeah[otherPlayer]);
                playerWithCarYeah.Remove(otherPlayer);
            }
        }
        catch
        {
            Debug.Log("bruh");
        }

    }
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        try
        {
            if (targetPlayer != PhotonNetwork.LocalPlayer)
            {

                var rig = GorillaGameManager.StaticFindRigForPlayer(targetPlayer);
                GameObject garn = playerWithCarYeah[targetPlayer];
                if ((bool)changedProps["Sitting"] != null)
                {
                    if ((bool)changedProps["Sitting"])
                    {
                        garn.transform.parent = rig.transform;
                        garn.transform.localPosition = new Vector3(0.2f, -0.9f, -0.3f);
                        garn.transform.localRotation = Quaternion.Euler(0f, 359.6664f, 0f);
                    }
                    else
                    {
                        garn.transform.parent = null;
                    }
                }
            }
        }
        catch
        {
            Debug.Log("ERROR CASUED AT ONPLAYERPROPSUPDATE");
        }



    }

}
