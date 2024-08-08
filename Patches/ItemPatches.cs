using HarmonyLib;
using UnityEngine;
using ScriptableObjectArchitecture;
using UnityEngine.SceneManagement;

namespace AP_Tinykin.Patches {
    class ItemPatches {
        [HarmonyPatch(typeof(StoryItemsProgress), "HasSoapboard")]
        class Soapboard {
            [HarmonyPostfix]
            static void SoapboardEnabled(StoryItemsProgress __instance, ref bool __result) {
                // If we don't have the item
                __result = true;
            }
        }

        [HarmonyPatch(typeof(StoryItemsProgress), "HasCatDoorChip")]
        class CatDoorChip {
            [HarmonyPostfix]
            static void CatDoorChipEnabled(StoryItemsProgress __instance, ref bool __result) {
                // If we don't have the item
                __result = false;
            }
        }

        [HarmonyPatch(typeof(PlayerController), "Update")]
        class ExtraBubbles {
            [HarmonyPrefix]
            static void ExtraBubblesPostfix(PlayerController __instance) {
                // Replace with number of items from server
                __instance.MaxFuel = new FloatReference(8f);
            }
        }
    }

}