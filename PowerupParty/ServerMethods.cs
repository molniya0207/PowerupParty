/*
 * using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PowerupParty
{
    [HarmonyPatch]
    internal class ServerMethods
    {
        [HarmonyPatch(typeof(Server), "InitializeServerPackets")]
        [HarmonyPostfix]
        static void InitializeCustomPackets()
        {
            Debug.Log("SERVER DATA INITIALIZED");

            Server.PacketHandlers.Add(200, new Server.PacketHandler(ReceiveNewPowerup));
            Server.PacketHandlers.Add(201, new Server.PacketHandler(ReceiveSendPowerup));
        }

        private static void ReceiveSendPowerup(int fromClient, Packet packet)
        {

        }

        private static void ReceiveNewPowerup(int fromClient, Packet packet)
        {

        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

            _packet.WriteLength();
            if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
            {
                for (int i = 1; i < Server.MaxPlayers; i++)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
                return;
            }
            foreach (Client client in Server.clients.Values)
            {
                if (((client != null) ? client.player : null) != null)
                {
                    Debug.Log("Sending packet to id: " + client.id);

                    var tcpVariant = typeof(ServerSend).GetField("TCPvariant", flags);

                    SteamPacketManager.SendPacket(client.player.steamId.Value, _packet, (P2PSend)tcpVariant.GetValue("TCPVariant"), SteamPacketManager.NetworkChannel.ToClient);
                }
            }
        }

        private static void SendTCPDataToAll(int exceptClient, Packet _packet)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

            _packet.WriteLength();
            if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
            {
                for (int i = 1; i < Server.MaxPlayers; i++)
                {
                    if (i != exceptClient)
                    {
                        Server.clients[i].tcp.SendData(_packet);
                    }
                }
                return;
            }
            foreach (Client client in Server.clients.Values)
            {
                if (((client != null) ? client.player : null) != null && SteamLobby.steamIdToClientId[client.player.steamId.Value] != exceptClient)
                {
                    var tcpVariant = typeof(ServerSend).GetField("TCPvariant", flags);

                    SteamPacketManager.SendPacket(client.player.steamId.Value, _packet, (P2PSend)tcpVariant.GetValue("TCPVariant"), SteamPacketManager.NetworkChannel.ToClient);
                }
            }
        }

        private static void SendTCPData(int toClient, Packet _packet)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            var tcpVariant = typeof(ServerSend).GetField("TCPvariant", flags);

            Packet packet2 = new Packet();
            packet2.SetBytes(_packet.CloneBytes());
            packet2.WriteLength();
            if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
            {
                Server.clients[toClient].tcp.SendData(packet2);
                return;
            }
            SteamPacketManager.SendPacket(Server.clients[toClient].player.steamId.Value, packet2, (P2PSend)tcpVariant.GetValue("TCPVariant"), SteamPacketManager.NetworkChannel.ToClient);
        }
    }
}

 */