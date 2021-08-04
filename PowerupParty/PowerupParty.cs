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
    [BepInPlugin("st.powerupparty", "PowerupParty", "1.1.0")]
    [BepInDependency(Terrain.Packets.Plugin.Main.Guid)]
    class PowerupParty : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("PowerupParty");
        public static OffroadPackets packets = new OffroadPackets("st.powerupparty");

        public void Awake()
        {
            Logger.LogInfo("PowerupParty loaded!");

            harmony.PatchAll();
            packets.Handle("GetPowerupFromServer", ClientGetPowerupFromServer);
            packets.Handle("AddPowerupChatMessageFromServer", ClientAddPowerupChatMessageFromServer);
            packets.Handle("SendPowerupToClient", ServerSendPowerupToClient);
        }

        public static void ServerSendPowerupToClient(int fromClient, BinaryReader br)
        {
            int idTo = br.ReadInt32();
            Debug.Log(idTo);
            int powerupId = br.ReadInt32();
            using (var packet = packets.WriteToClient("GetPowerupFromServer", idTo, P2PSend.Reliable))
            {
                packet.Write(fromClient);
                packet.Write(powerupId);
                packet.Send();
            }
            using (var packet = packets.WriteToAll("AddPowerupChatMessageFromServer", P2PSend.Reliable))
            {
                packet.Write(fromClient);
                packet.Write(idTo);
                packet.Write(powerupId);
                packet.Send();
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
