using HarmonyLib;
using UnityEngine;
using ScriptableObjectArchitecture;
using UnityEngine.SceneManagement;
using AP_Tinykin;

namespace AP_Tinykin.Patches {
    class LocationPatches {
        [HarmonyPatch(typeof(CollectibleContainer), "OnTriggerEnter")]
        class SoapClusterPatch {
            [HarmonyPrefix]
            static bool SoapClusterDisableCollect(CollectibleContainer __instance) {
                // THIS IS CURRENTLY NOT FUNCTIONING
                Debug.Log(__instance.gameObject.transform.parent.gameObject.name);
                if (__instance.gameObject.transform.parent.gameObject.name.Contains("_AP")) {
                    __instance.gameObject.transform.parent.gameObject.SetActive(false);
                    Plugin.Instance.SendLocation(__instance.gameObject.transform.parent.gameObject.name.Replace("_AP", ""));
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SmallLiftable), "StartLift")]
        class SmallLiftablePatch2 {
            [HarmonyPrefix]
            static bool SmallLiftablePatchPrefix(SmallLiftable __instance) {
                if (__instance.gameObject.name.Contains("_AP")) {
                    __instance.gameObject.SetActive(false);
                    Plugin.Instance.SendLocation(__instance.gameObject.name.Replace("_AP", ""));
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(SbirzCarrier), "CollideWith")]
        class CollisionTinykin {
            [HarmonyPrefix]
            static bool CollisionTinykinPrefix(SbirzCarrier __instance, InteractionTrigger _interaction) {
                SmallLiftable smallLiftable = _interaction as SmallLiftable;
                if (smallLiftable != null)
                {
                    smallLiftable.StartLift();
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(UniqueCollectible), "Collect")]
        class CollectibleLoc {
            [HarmonyPrefix]
            static bool SendLocOnCollectible(UniqueCollectible __instance) {
                // Figure out how to hijack and still perform animation with Archipelago text
                return false;
            }
        }

        [HarmonyPatch(typeof(LiftableBase), "LoadState")]
        class LoadStatePatch {
            [HarmonyPrefix]
            static bool LoadStatePrefix(LiftableBase __instance) {
                if (__instance.gameObject.name.Contains("_AP")) {
                    return false;
                }
                return true;
            }
        }
    }

}