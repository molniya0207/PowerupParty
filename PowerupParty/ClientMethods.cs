/*
 using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PowerupParty
{
    [HarmonyPatch]
    internal class ClientMethods
    {
        [HarmonyPatch(typeof(LocalClient), "InitializeClientData")]
        static void Postfix()
        {
            Debug.Log("CLIENT DATA INITIALIZED");

            LocalClient.packetHandlers.Add(200, new LocalClient.PacketHandler(ReceivePowerup));
        }

        private static void ReceivePowerup(Packet packet)
        {
            PowerupInventory.Instance.powerups[3]++;
            PowerupUI.Instance.AddPowerup(3);
            

        }

        private static void SendTCPData(Packet _packet)
{
    ClientSend.bytesSent += _packet.Length();
    ClientSend.packetsSent++;
    _packet.WriteLength();
    if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
    {
        LocalClient.instance.tcp.SendData(_packet);
        return;
    }
    SteamPacketManager.SendPacket(LocalClient.instance.serverHost.Value, _packet, P2PSend.Reliable, SteamPacketManager.NetworkChannel.ToServer);
}


    }
}
*/