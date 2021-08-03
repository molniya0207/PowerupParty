using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain.Packets;
using UnityEngine;

// test seed -1461761351

namespace PowerupParty
{
    [BepInPlugin("st.powerupparty", "PowerupParty", "1.0.0")]
    class PowerupParty : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("PowerupParty");
        public static OffroadPackets packets = new OffroadPackets("st.powerupparty");

        public void Awake()
        {
            Logger.LogInfo("PowerupParty loaded!");

            harmony.PatchAll();
            packets.HandleClient("GetPowerupFromServer", ClientGetPowerupFromServer);
            packets.HandleClient("AddPowerupChatMessageFromServer", ClientAddPowerupChatMessageFromServer);
            packets.HandleServer("SendPowerupToClient", ServerSendPowerupToClient);
        }

        public static void ServerSendPowerupToClient(int fromClient, BinaryReader br)
        {
            int idTo = br.ReadInt32();
            Debug.Log(idTo);
            int powerupId = br.ReadInt32();
            using (packets.WriteToClient("GetPowerupFromServer", idTo, out var writer, P2PSend.Reliable))
            {
                writer.Write(fromClient);
                writer.Write(powerupId);
            }
            using (packets.WriteToAll("AddPowerupChatMessageFromServer", out var writer, P2PSend.Reliable))
            {
                writer.Write(fromClient);
                writer.Write(idTo);
                writer.Write(powerupId);
            }
        }

        public static void ClientGetPowerupFromServer(BinaryReader br)
        {
            int idFrom = br.ReadInt32();
            int powerupId = br.ReadInt32();
            PowerupInventory.Instance.powerups[powerupId]++;
            PowerupUI.Instance.AddPowerup(powerupId);
            //ClientSend.SendChatMessage("<color=#00FF00>" + NetworkController.Instance.playerNames[idFrom] + " gived " + ItemManager.Instance.allPowerups[powerupId].name + " to " + NetworkController.Instance.playerNames[LocalClient.instance.myId] + "!");
            ChatBox.Instance.AppendMessage(-1, "<color=#00FF00>You received " + ItemManager.Instance.allPowerups[powerupId].name + " from " + NetworkController.Instance.playerNames[idFrom] + "!", "");
        }

        public static void ClientAddPowerupChatMessageFromServer(BinaryReader br)
        {
            int idFrom = br.ReadInt32();
            int idTo = br.ReadInt32();
            int powerupId = br.ReadInt32();
            ChatBox.Instance.AppendMessage(-1, "<color=#00FF00>" + NetworkController.Instance.playerNames[idFrom] + " gived " + ItemManager.Instance.allPowerups[powerupId].name + " to " + NetworkController.Instance.playerNames[idTo] + "!", "");
        }
    }
}
