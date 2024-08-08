using System.Linq;
using AP_Tinykin;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AP_Tinykin.Receiver {
    public class ItemReceiver {
        public NetworkItem[] previousItems = {};

        public void CollectItem(NetworkItem networkItem) {
            var itemReceivedID = networkItem.Item;

            Debug.Log("Item Received: " + itemReceivedID);

            previousItems = previousItems.AddToArray(networkItem);

            if (ItemMap.SmallLiftables.ContainsKey(itemReceivedID)) {
                foreach (var room in Plugin.Instance.Rooms) {
                    if (room.sceneName == ItemMap.ItemsPerRoom[itemReceivedID]) {
                        Debug.Log("Queued Small Liftable: " + ItemMap.SmallLiftables[itemReceivedID]);
                        room.queuedItems = room.queuedItems.AddToArray(ItemMap.SmallLiftables[itemReceivedID]);
                    }
                }
            } else if (ItemMap.Mail.ContainsKey(itemReceivedID)) {
                int mailCount = 0;
                foreach (var item in Plugin.Instance.session.Items.AllItemsReceived) {
                    if (item.Item == itemReceivedID) {
                        mailCount++;
                    }
                }
                foreach (var room in Plugin.Instance.Rooms) {
                    if (room.sceneName == ItemMap.ItemsPerRoom[itemReceivedID]) {
                        Debug.Log("Queued Mail: " + ItemMap.Mail[itemReceivedID][mailCount - 1]);
                        room.queuedItems = room.queuedItems.AddToArray(ItemMap.Mail[itemReceivedID][mailCount - 1]);
                    }
                }
            } else if (ItemMap.Pollen.ContainsKey(itemReceivedID)) {
                int pollenCount = 0;
                foreach (var item in Plugin.Instance.session.Items.AllItemsReceived) {
                    if (item.Item == itemReceivedID) {
                        pollenCount++;
                    }
                }
                foreach (var room in Plugin.Instance.Rooms) {
                    if (room.sceneName == ItemMap.ItemsPerRoom[itemReceivedID]) {
                        Debug.Log("Queued Pollen: " + ItemMap.Pollen[itemReceivedID][pollenCount - 1]);
                        room.queuedItems = room.queuedItems.AddToArray(ItemMap.Pollen[itemReceivedID][pollenCount - 1]);
                    }
                }
            }

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

        public void CheckNewItems() {
            if (previousItems != Plugin.Instance.session.Items.AllItemsReceived.ToArray()) {
                foreach (var item in Plugin.Instance.session.Items.AllItemsReceived) {
                    if (!previousItems.Contains(item)) {
                        CollectItem(item);
                    }
                }
            }
        }
    }
}