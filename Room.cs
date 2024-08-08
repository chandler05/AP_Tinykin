using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AP_Tinykin {
    public class Room {
        public SmallLiftable[] smallLiftables;
        public RareCollectible[] rareCollectibles;
        public string[] queuedItems = {};
        public SmallLiftable[] queuedForGrabbing = {};
        public RareCollectible[] queuedForCollecting = {};
        public string sceneName;
        public bool finalized = false;

        public Room(string sceneName) {
            this.sceneName = sceneName;
        }

        public void GetAllSmallLiftables() {
            smallLiftables = GameObject.FindObjectsOfType<SmallLiftable>();
        }

        public void GetAllRareCollectibles() {
            rareCollectibles = GameObject.FindObjectsOfType<RareCollectible>();
        }

        public void CloneAllSmallLiftables() {
            for (int i = 0; i < smallLiftables.Length; i++) {
                if (!smallLiftables[i].gameObject.name.Contains("_AP")) {
                    GameObject clone = GameObject.Instantiate(smallLiftables[i].gameObject);
                    clone.name = smallLiftables[i].gameObject.name + "_AP";
                    clone.transform.position = smallLiftables[i].transform.position;
                    clone.transform.parent = smallLiftables[i].transform.parent;
                    if (smallLiftables[i].gameObject.name.Contains("Mail")) {
                        long ap_id = ItemMap.Mail.FirstOrDefault(x => x.Value.Contains(smallLiftables[i].gameObject.name)).Key;
                        int listPosition = ItemMap.Mail[ap_id].ToList().IndexOf(smallLiftables[i].gameObject.name);
                        if (Plugin.Instance.session.Items.AllItemsReceived.Where(x => x.Item == ap_id).Count() < listPosition + 1) {
                            smallLiftables[i].gameObject.SetActive(false);
                        }
                    } else {
                        long ap_id = ItemMap.SmallLiftables.FirstOrDefault(x => x.Value == smallLiftables[i].gameObject.name).Key;
                        if (!Plugin.Instance.session.Items.AllItemsReceived.Any(x => x.Item == ap_id)) {
                            smallLiftables[i].gameObject.SetActive(false);
                        }
                    }

                    if (Plugin.Instance.session.Locations.AllLocationsChecked.Any(x => x == Plugin.Instance.SlotData.locations[smallLiftables[i].gameObject.name].ap_id)) {
                        clone.SetActive(false);
                    }
                }
            }
        }

        public void CloneAllRareCollectibles() {
            for (int i = 0; i < rareCollectibles.Length; i++) {
                if (!rareCollectibles[i].gameObject.name.Contains("_AP")) {
                    GameObject clone = GameObject.Instantiate(rareCollectibles[i].gameObject);
                    clone.name = rareCollectibles[i].gameObject.name + "_AP";
                    clone.transform.position = rareCollectibles[i].transform.position;
                    clone.transform.parent = rareCollectibles[i].transform.parent;
                    if (rareCollectibles[i].gameObject.name.Contains("Soap") || rareCollectibles[i].gameObject.name.Contains("Pollen")) {
                        long ap_id = ItemMap.Pollen.FirstOrDefault(x => x.Value.Contains(rareCollectibles[i].gameObject.name)).Key;
                        int listPosition = ItemMap.Pollen[ap_id].ToList().IndexOf(rareCollectibles[i].gameObject.name);
                        if (Plugin.Instance.session.Items.AllItemsReceived.Where(x => x.Item == ap_id).Count() < listPosition + 1) {
                            rareCollectibles[i].gameObject.SetActive(false);
                        }
                    } else {
                        rareCollectibles[i].gameObject.SetActive(false);
                        clone.SetActive(false);
                        continue;
                    }

                    if (Plugin.Instance.session.Locations.AllLocationsChecked.Any(x => x == Plugin.Instance.SlotData.locations[rareCollectibles[i].gameObject.name].ap_id)) {
                        clone.SetActive(false);
                    }
                }
            }
        }

        public void EnableAllSmallLiftables() {
            for (int i = 0; i < smallLiftables.Length; i++) {
                smallLiftables[i].gameObject.SetActive(true);
            }
        }

        public void DequeueCollectibles() {
            Debug.Log("Dequeueing Collectibles");
            for (int i = 0; i < queuedItems.Length; i++) {
                if (queuedItems[i] == null) {
                    continue;
                }
                var smallLiftable = smallLiftables.FirstOrDefault(x => x.gameObject.name == queuedItems[i]);
                if (smallLiftable != null && !smallLiftable.ArrivedAtDestination) {
                    queuedForGrabbing = queuedForGrabbing.AddToArray(smallLiftable);
                } else {
                    var rareCollectible = rareCollectibles.FirstOrDefault(x => x.gameObject.name == queuedItems[i]);
                    if (rareCollectible != null && !rareCollectible.Collected) {
                        queuedForCollecting = queuedForCollecting.AddToArray(rareCollectible);
                    }
                }
            }
            queuedItems = new string[] {};
        }

        public void TryToHoldItemQueue() {
            for (int i = 0; i < queuedForGrabbing.Length; i++) {
                if (queuedForGrabbing[i] == null) {
                    continue;
                }
                var smallLiftable = queuedForGrabbing[i];
                if (!smallLiftable.gameObject.activeInHierarchy) {
                    smallLiftable.gameObject.SetActive(true);
                }
                SbirzCarrier sbirzCarrier = Game.PLAYER.FindTinykinByType<SbirzCarrier>((SbirzCarrier sbirz) => sbirz.CarriedObject == null);
                if (sbirzCarrier != null)
                {
                    sbirzCarrier.FollowingPlayer = false;
                    sbirzCarrier.PickSmallLiftableUp(smallLiftable);
                    queuedForGrabbing[i] = null;
                }
            }
        }

        public void TryToCollectItemQueue() {
            for (int i = 0; i < queuedForCollecting.Length; i++) {
                if (queuedForCollecting[i] == null) {
                    continue;
                }
                var rareCollectible = queuedForCollecting[i];
                if (!rareCollectible.gameObject.activeInHierarchy) {
                    rareCollectible.gameObject.SetActive(true);
                }
                if (Game.PLAYER != null) {
                    rareCollectible.gameObject.transform.position = Game.PLAYER.transform.position;
                    queuedForCollecting[i] = null;
                }
            }
        }
    }
}