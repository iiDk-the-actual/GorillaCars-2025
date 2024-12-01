using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

namespace GorillaCars
{
    public class assestloaderstandcode : MonoBehaviourPunCallbacks
    {
        public int index = 0;
        public static assestloaderstandcode instance { get; set; }
        public TextMeshPro Name;
        public TextMeshPro Author;
        public TextMeshPro Description;
        public CustomCarDescripter carDescripter;

        private float touchTime = 0f;
        private const float debounceTime = 0.25f;

        private float touchTime1 = 0f;
        private const float debounceTime1 = 0.25f;
        public void Awake()
        {
            instance = this;
            Name = transform.FindChildRecursive("Name").GetComponent<TextMeshPro>();
            Author = transform.FindChildRecursive("Author").GetComponent<TextMeshPro>();
            Description = transform.FindChildRecursive("Description").GetComponent<TextMeshPro>();
            Name.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Name;
            Author.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Author;
            Description.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Description;

            foreach (GameObject cars in Plugin.Instance.carsGameobjs)
            {
                cars.GetComponentInChildren<Rigidbody>().isKinematic = true;
                cars.transform.position = Vector3.zero;
                cars.transform.GetChild(0).position = Vector3.zero;

            }
            Plugin.Instance.carsGameobjs.ElementAt(index).GetComponentInChildren<Rigidbody>().isKinematic = false;
            Plugin.Instance.carsGameobjs.ElementAt(index).GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
            Plugin.Instance.carsGameobjs.ElementAt(index).transform.position = Vector3.zero;
            Plugin.Instance.carsGameobjs.ElementAt(index).transform.GetChild(0).position = new Vector3(-64.4182f, 2.3273f, -71.1818f);
            Plugin.Instance.CarGameObject = Plugin.Instance.carsGameobjs.ElementAt(index);


        }
        public void Clicked(string btnname)
        {
            switch (btnname)
            {

                case "Next":
                    try
                    {
                        if (touchTime + debounceTime >= Time.time)
                        {
                            if (index < Plugin.Instance.carsGameobjs.Count - 1)
                            {

                                index++;

                            }
                        }
                        touchTime = Time.time;
                        Name.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Name;
                        Author.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Author;
                        Description.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Description;
                    }
                    catch
                    {
                        Debug.Log("Error with Next?");
                    }

                    break;
                case "Prev":
                    try
                    {
                        if (touchTime1 + debounceTime1 >= Time.time)
                        {
                            if (index > 0)
                            {

                                index--;

                            }
                        }
                        touchTime1 = Time.time;
                        Name.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Name;
                        Author.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Author;
                        Description.text = Plugin.Instance.carsGameobjs.ElementAt(index).GetComponent<CustomCarDescripter>().Description;
                    }
                    catch
                    {
                        Debug.Log("error with Prev?");
                    }

                    break;
                case "Confirm action":
                    try
                    {
                        foreach (GameObject cars in Plugin.Instance.carsGameobjs)
                        {
                            cars.GetComponentInChildren<Rigidbody>().isKinematic = true;
                            cars.transform.position = Vector3.zero;
                            cars.transform.GetChild(0).position = Vector3.zero;

                        }
                        Plugin.Instance.carsGameobjs.ElementAt(index).GetComponentInChildren<Rigidbody>().isKinematic = false;
                        Plugin.Instance.carsGameobjs.ElementAt(index).GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
                        Plugin.Instance.carsGameobjs.ElementAt(index).transform.position = Vector3.zero;
                        Plugin.Instance.carsGameobjs.ElementAt(index).transform.GetChild(0).position = new Vector3(-64.4182f, 2.3273f, -71.1818f);
                        Plugin.Instance.CarGameObject = Plugin.Instance.carsGameobjs.ElementAt(index);
                        var HT = new ExitGames.Client.Photon.Hashtable();
                        HT.AddOrUpdate("CarName", Plugin.Instance.CarGameObject.name);
                        PhotonNetwork.SetPlayerCustomProperties(HT);

                    }
                    catch
                    {
                        Debug.Log("error with confirm?");
                    }
                    break;
            }


        }

    }

}
