using HarmonyLib;
using ModestTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PerControllerOffset
{
    [HarmonyPatch(typeof(MainSettingsModelSO), nameof(MainSettingsModelSO.Load))]
    class SettingsLoad
    {
        static void Postfix(MainSettingsModelSO __instance)
        {
            PCOffsetPlugin.UpdateControllerName();
            
            if (!PCOffsetPlugin.has_controller_name)
            {
                PCOffsetPlugin.Log.Error($"SettingsLoad: No controller name available.");
                return;
            }

            if (ControllerConfigManager.HasControllerConfig(PCOffsetPlugin.controller_name))
            {
                ControllerOffsetConfig config = ControllerConfigManager.GetControllerConfig(PCOffsetPlugin.controller_name);
                if (config == null)
                {
                    PCOffsetPlugin.Log.Error("SettingsLoad: Config was null");
                    return;
                }
                PCOffsetPlugin.Log.Info($"Loaded offset config for {PCOffsetPlugin.controller_name}");
                __instance.controllerPosition.value = new Vector3(Mathf.Clamp(config.controllerPositionX, -0.1f, 0.1f), Mathf.Clamp(config.controllerPositionY, -0.1f, 0.1f), Mathf.Clamp(config.controllerPositionZ, -0.1f, 0.1f));
                __instance.controllerRotation.value = new Vector3(Mathf.Clamp(config.controllerRotationX, -180f, 180f), Mathf.Clamp(config.controllerRotationY, -180f, 180f), Mathf.Clamp(config.controllerRotationZ, -180f, 180f));
            } else
            {
                PCOffsetPlugin.Log.Info($"No offset config found for {PCOffsetPlugin.controller_name}");
            }
        }
    }

    [HarmonyPatch(typeof(MainSettingsModelSO), nameof(MainSettingsModelSO.Save))]
    class SettingsSave
    {
        static void Postfix(MainSettingsModelSO __instance)
        {
            if (!PCOffsetPlugin.has_controller_name)
            {
                PCOffsetPlugin.Log.Error($"SettingsSave: No controller name available.");
                return;
            }

            ControllerOffsetConfig config = new ControllerOffsetConfig
            {
                controllerPositionX = __instance.controllerPosition.value.x,
                controllerPositionY = __instance.controllerPosition.value.y,
                controllerPositionZ = __instance.controllerPosition.value.z,
                controllerRotationX = __instance.controllerRotation.value.x,
                controllerRotationY = __instance.controllerRotation.value.y,
                controllerRotationZ = __instance.controllerRotation.value.z,
            };
            ControllerConfigManager.SaveControllerConfig(PCOffsetPlugin.controller_name, config);
            PCOffsetPlugin.Log.Info($"Saved offset config for {PCOffsetPlugin.controller_name}");
        }
    }
}
