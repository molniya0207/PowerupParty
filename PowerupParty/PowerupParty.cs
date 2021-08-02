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
            packets.HandleServer("SendPowerupToClient", ServerSendPowerupToClient);
        }

        public static void ServerSendPowerupToClient(int fromClient, BinaryReader br)
        {
            Debug.Log("JJJJJJJJJJJJJJJJJ");
            int idTo = br.ReadInt32();
            Debug.Log(idTo);
            int powerupId = br.ReadInt32();
            Debug.Log(idTo);
            using (packets.WriteToClient("GetPowerupFromServer", idTo, out var writer, P2PSend.Reliable))
            {
                Debug.Log(idTo);
                writer.Write(fromClient);
                writer.Write(powerupId);
                Debug.Log(idTo);
            }
        }

        public static void ClientGetPowerupFromServer(BinaryReader br)
        {
            int idFrom = br.ReadInt32();
            int powerupId = br.ReadInt32();
            ClientSend.SendChatMessage("<color=#00FF00>" + NetworkController.Instance.playerNames[idFrom] + " gived powerup " + ItemManager.Instance.allPowerups[powerupId].name + " to " + NetworkController.Instance.playerNames[LocalClient.instance.myId] + "!");
            ChatBox.Instance.AppendMessage(-1, "<color=#00FF00>You received powerup " + ItemManager.Instance.allPowerups[powerupId].name + " from " + NetworkController.Instance.playerNames[idFrom] + "!", "");
        }
    }
}
