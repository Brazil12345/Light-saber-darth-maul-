using BepInEx;
using System;
using UnityEngine;
using Utilla;
using UnityEngine.XR;
using System.IO;
using System.Reflection;

namespace LightSaber
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;

        GameObject _StarWars;

        GameObject _LightSaber;

        GameObject HandR;

        private readonly XRNode rNode = XRNode.RightHand;

        private readonly XRNode lNode = XRNode.RightHand;

        bool isgrip;

        bool cangrip = true;

        bool istrigger;

        bool cantrigger = true;

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("LightSaber.Assets.starwarssong");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            GameObject StarWarsSoundObject = bundle.LoadAsset<GameObject>("StarWarsSong");
            _StarWars = Instantiate(StarWarsSoundObject);

            Stream _str = Assembly.GetExecutingAssembly().GetManifestResourceStream("LightSaber.Assets.darthmaul");
            AssetBundle _bundle = AssetBundle.LoadFromStream(_str);
            GameObject LightSaberObject = _bundle.LoadAsset<GameObject>("LightSaber");
            _LightSaber = Instantiate(LightSaberObject);

            HandR = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/");
            _LightSaber.transform.SetParent(HandR.transform, false);
            _LightSaber.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            _LightSaber.transform.localRotation = Quaternion.Euler(62.5278f, 260.205f, 85.5954f);
            _LightSaber.transform.localPosition = new Vector3(-0.07f, -0.01f, -0.02f);
        }

        void Update()
        {
            InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out isgrip);
            InputDevices.GetDeviceAtXRNode(lNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out istrigger);

            if (isgrip)
            {
                // This is so we cannot spam the noise
                if (cangrip)
                {
                    _StarWars.GetComponent<AudioSource>().Play();
                    cangrip = false;
                }
            }
            else
            {
                // This is where grip is not pressed so here we will make it so u can grip
                cangrip = true;
            }

            if (istrigger)
            {
                if (cantrigger)
                {
                    _StarWars.GetComponent<AudioSource>().Stop();
                }
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
    }
}
