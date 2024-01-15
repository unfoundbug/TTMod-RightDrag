using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace RightDrag
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class RightMod : BaseUnityPlugin
    {
        public const string pluginGuid = "rightmod.nhickling.co.uk";
        public const string pluginName = "RightDrag";
        public const string pluginVersion = "0.0.0.3";
        private static IInputSystem inputSystem;
        private static BepInEx.Logging.ManualLogSource ModLogger;
        private static List<int> activatedSlots = new List<int>();
        public void Awake()
        {
            Logger.LogInfo("RightDrag: started");
            inputSystem = BepInEx.UnityInput.Current;
            Logger.LogInfo("RightDrag: Attached to input state");

            Harmony harmony = new Harmony(pluginGuid);
            Logger.LogInfo("RightDrag: Fetching patch references");
            MethodInfo original = AccessTools.Method(typeof(InventoryGridUI), "MouseOverCallback", new Type[] { typeof(int), typeof(bool)});
            MethodInfo patch = AccessTools.Method(typeof(RightMod), "OnPointerEnter_MyPatch");
            Logger.LogInfo("RightDrag: Starting Patch");
            harmony.Patch(original, null, new HarmonyMethod(patch));
            Logger.LogInfo("RightDrag: Patched");

            ModLogger = Logger;
        }

        public static void OnPointerEnter_MyPatch(InventoryGridUI __instance, int slotIndex, bool isEntering)
        {
            if (inputSystem.GetMouseButton(1))
            {
                if (activatedSlots.Contains(slotIndex))
                    return;
                else
                {
                    __instance.HandleClickSlot(slotIndex, false);
                    activatedSlots.Add(slotIndex);
                }
            }
            else
            {
                activatedSlots.Clear();
            }
        }
    }
}
