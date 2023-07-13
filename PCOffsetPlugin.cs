using IPA;
using IPA.Config;
using IPA.Config.Stores;
using JetBrains.Annotations;
using System;
using System.Collections;
using Zenject;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using System.IO;

namespace PerControllerOffset
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class PCOffsetPlugin
    {
        internal static PCOffsetPlugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Harmony HarmonyInstance { get; private set; }

        [Init]
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            HarmonyInstance = new Harmony("uk.invoxiplaygames.percontrolleroffset");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            HarmonyInstance.PatchAll();
            ControllerConfigManager.EnsureFolderExists();
        }

        public static string controller_name { get; private set; }
        public static bool has_controller_name { get; private set; }

        public static void UpdateControllerName()
        {
            controller_name = "Unknown";
            has_controller_name = false;

            PCOffsetPlugin.Log.Debug($"Loaded XR device name: {XRSettings.loadedDeviceName}");
            if (XRSettings.loadedDeviceName.IndexOf("openvr", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                List<InputDevice> hand_devices = new List<InputDevice>();
                InputDevices.GetDevicesAtXRNode(XRNode.RightHand, hand_devices);
                if (hand_devices.Count > 0)
                {
                    controller_name = hand_devices.First().name;
                    has_controller_name = true;
                    PCOffsetPlugin.Log.Debug($"OpenVR Controller: {controller_name}");
                }
                else
                {
                    PCOffsetPlugin.Log.Error("No right hand detected from OpenVR nodes");
                }
            }
            else if (XRSettings.loadedDeviceName.IndexOf("oculus", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // try to deduce controller type based on headset
                OVRPlugin.SystemHeadset oculus_hmd = OVRPlugin.GetSystemHeadsetType();
                switch (oculus_hmd)
                {
                    // 1st gen oculus touch - can these be used on DK2?? idk
                    case OVRPlugin.SystemHeadset.Rift_DK2:
                    case OVRPlugin.SystemHeadset.Rift_CV1:
                        controller_name = "Oculus Touch";
                        break;
                    // 2nd gen oculus touch controllers
                    case OVRPlugin.SystemHeadset.Rift_S:
                    case OVRPlugin.SystemHeadset.Oculus_Quest:
                    case OVRPlugin.SystemHeadset.Oculus_Link_Quest:
                        controller_name = "Oculus Touch 2";
                        break;
                    // quest 2 controllers - no way to detect if they're pro or not :/
                    case OVRPlugin.SystemHeadset.Oculus_Quest_2:
                    case OVRPlugin.SystemHeadset.Oculus_Link_Quest_2:
                        controller_name = "Quest 2";
                        break;
                    default:
                        controller_name = $"OVR_{oculus_hmd}";
                        break;
                }
                has_controller_name = true;
                PCOffsetPlugin.Log.Debug($"Oculus Controller: {controller_name}");
            }
            else
            {
                PCOffsetPlugin.Log.Error($"Unknown loaded device name \"{XRSettings.loadedDeviceName}\"");
            }
        }
    }
}
