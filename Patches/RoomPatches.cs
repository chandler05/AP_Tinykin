using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AP_Tinykin.Patches {
    class RoomPatches {
        [HarmonyPatch(typeof(PlayerController), "Awake")]
        class LoadLiftablesPatch {
            [HarmonyPrefix]
            static void LoadLiftablesToRooms(PlayerController __instance) {
                int sceneCount = SceneManager.sceneCount;
                for (int i = 0; i < sceneCount; i++) {
                    Scene scene = SceneManager.GetSceneAt(i);
                    string sceneName = scene.name;
                    foreach (Room room in Plugin.Instance.Rooms) {
                        if (sceneName == room.sceneName) {
                            room.GetAllSmallLiftables();
                            room.CloneAllSmallLiftables();

                            room.GetAllRareCollectibles();
                            room.CloneAllRareCollectibles();
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerController), "Awake")]
        class DequeueCollectiblesPatch {
            [HarmonyPostfix]
            static void Deque(PlayerController __instance) {
                int sceneCount = SceneManager.sceneCount;
                for (int i = 0; i < sceneCount; i++) {
                    Scene scene = SceneManager.GetSceneAt(i);
                    string sceneName = scene.name;
                    foreach (Room room in Plugin.Instance.Rooms) {
                        if (sceneName == room.sceneName) {
                            room.DequeueCollectibles();
                        }
                    }
                }
            }
        }
    }

}
