using AP_Tinykin.Patches;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using Archipelago.MultiClient.Net;
using System;
using Archipelago.MultiClient.Net.Enums;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Linq;
using AP_Tinykin.Receiver;

namespace AP_Tinykin
{
    // TODO:
    // Fix throwing Tinykin at already obtained Small liftables causing them to be deleted
    // Pollen Clusters that are locked at the beginning are not unlocked when the player completes the necessary actions
    // Remove tutorial
    // Fix Pollen Clusters not sending location checks and behaving as normal
    // Fix being given all Small Liftables on startup, even if they have already been used
    // Shift items into the correct locations if they are placed in the same room they were originally in


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public ArchipelagoSession session { get; private set; }
        public Room[] Rooms { get; private set; }
        public ArchipelagoSlotData SlotData { get; private set; }
        public Receiver.ItemReceiver ItemReceiver { get; private set; }
        private void Awake()
        {
            Instance = this;

            ItemReceiver = new Receiver.ItemReceiver();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Debug.Log("LOADED");

            Rooms = new Room[1];
            Rooms[0] = new Room("02_LivingRoom_v4");
            Debug.Log(Rooms.Length);

            LocationPatches locationPatches = new LocationPatches();
            ItemPatches itemPatches = new ItemPatches();
            RoomPatches roomPatches = new RoomPatches();

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            session = ArchipelagoSessionFactory.CreateSession("-");

            Connect(session, "-", "-", "");
        }
        private void Update()
        {
            if (session != null && SlotData != null)
            {
                ItemReceiver.CheckNewItems();
            }

            if (Game.PLAYER != null) {
                if (LevelDescription.INSTANCE.sceneData.sceneName != null) {
                    foreach (Room room in Rooms) {
                        if (room.sceneName == LevelDescription.INSTANCE.sceneData.sceneName) {
                            room.TryToHoldItemQueue();
                            room.TryToCollectItemQueue();
                        }
                    }
                }
            }
        }

        private static void Connect(ArchipelagoSession session, string server, string user, string pass)
        {
            LoginResult result;

            try
            {
                result = session.TryConnectAndLogin("Tinykin", user, ItemsHandlingFlags.AllItems, version: new Version(0, 4, 4));
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server} as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                return; // Did not connect, show the user the contents of `errorMessage`
            }
            
            // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be used to interact with the server and the returned `LoginSuccessful` contains some useful information about the initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
            var loginSuccess = (LoginSuccessful)result;

            Instance.ProcessSlotData(loginSuccess);

            Debug.Log("Connected to Archipelago!");
        }

        public class ArchipelagoSlotData
        {
            public Dictionary<string, ItemLocation> locations;
            public ServerSettings settings;

            public ArchipelagoSlotData(Dictionary<string, ItemLocation> locations, ServerSettings settings)
            {
                this.locations = locations;
                this.settings = settings;
            }
        }

        private void ProcessSlotData(LoginSuccessful login)
        {
            var apLocations = ((JObject)login.SlotData["locations"]).ToObject<Dictionary<string, ItemLocation>>();
            var settings = ((JObject)login.SlotData["settings"]).ToObject<ServerSettings>() ?? new ServerSettings();
            SlotData = new ArchipelagoSlotData(apLocations, settings);
        }

        public void SendLocation(string inGameID)
        {
            session.Locations.CompleteLocationChecks(SlotData.locations[inGameID].ap_id);
        }
    }
}
