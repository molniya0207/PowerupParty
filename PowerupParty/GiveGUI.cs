using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Terrain.Packets;

namespace PowerupParty
{
    [HarmonyPatch]
    class GiveGUI
    {
        static GameObject playerSelectUI;
        static int currPUI;

        [HarmonyPatch(typeof(PowerupUI), "AddPowerup")]
        [HarmonyPostfix]
        static void AddPowerupToUI(ref int powerupId)
        {
            /**/
            foreach (KeyValuePair<int, GameObject> i in PowerupUI.Instance.powerups)
            {
                Button btnComponent;
                if (!i.Value.TryGetComponent(out btnComponent))
                {
                    i.Value.AddComponent<Button>();
                    i.Value.TryGetComponent(out btnComponent);
                }
                Navigation nav = new Navigation();
                nav.mode = Navigation.Mode.None;
                btnComponent.navigation = nav;
                btnComponent.onClick.RemoveAllListeners();
                btnComponent.onClick.AddListener(delegate ()
                {
                    currPUI = i.Key;
                    playerSelectUI.SetActive(true);
                });
            }
        }

        [HarmonyPatch(typeof(GameManager), "StartGame")]
        [HarmonyPostfix]
        static void GameStartPatch()
        {
            playerSelectUI = new GameObject("playerSelectUI", new[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage), typeof(VerticalLayoutGroup) });
            playerSelectUI.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            playerSelectUI.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 500);
            playerSelectUI.layer = 10;
            playerSelectUI.transform.parent = GameObject.Find("/UI (1)").transform;
            playerSelectUI.GetComponent<RawImage>().color = new Color32(80, 80, 80, 180);
            playerSelectUI.SetActive(false);
            playerSelectUI.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(15, 15, 15, 15);
            playerSelectUI.GetComponent<VerticalLayoutGroup>().spacing = 8;
            playerSelectUI.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
            playerSelectUI.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
            GameObject selt = new GameObject("plybtn", new[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage) });
            selt.transform.parent = playerSelectUI.transform;
            selt.GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 0.4f, 0.5f);
            GameObject selText = new GameObject("selText", new[] { typeof(TextMeshProUGUI) });
            selText.transform.parent = selt.transform;
            selText.GetComponent<TextMeshProUGUI>().text = "What player to give powerup?";
            selText.GetComponent<TextMeshProUGUI>().fontSize = 32;
            selText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
            selText.GetComponent<TextMeshProUGUI>().autoSizeTextContainer = true;
            selText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
            for (int y = 0; y < NetworkController.Instance.nPlayers; y++)
            {
                if (y == LocalClient.instance.myId) continue;
                int z = y;
                GameObject plybtn = new GameObject("plybtn", new[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage), typeof(Button) });
                plybtn.transform.parent = playerSelectUI.transform;
                plybtn.GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
                GameObject plybtnText = new GameObject("plybtnText", new[] { typeof(TextMeshProUGUI) });
                plybtnText.transform.parent = plybtn.transform;
                plybtnText.GetComponent<TextMeshProUGUI>().text = NetworkController.Instance.playerNames[y] + " (" + y.ToString() + ")";
                plybtnText.GetComponent<TextMeshProUGUI>().fontSize = 16;
                plybtnText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
                plybtnText.GetComponent<TextMeshProUGUI>().autoSizeTextContainer = true;
                plybtnText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
                plybtn.GetComponent<Button>().onClick.RemoveAllListeners();
                plybtn.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    if (PowerupInventory.Instance.powerups[currPUI] > 0)
                    {
                        PowerupInventory.Instance.powerups[currPUI]--;
                        TextMeshProUGUI componentInChildren = PowerupUI.Instance.powerups[currPUI].GetComponentInChildren<TextMeshProUGUI>();
                        int num = int.Parse(componentInChildren.text);
                        num--;
                        componentInChildren.text = num.ToString();
                        // TODO: Send add powerup packet
                        using (var packet = PowerupParty.packets.WriteToServer("SendPowerupToClient"))
                        {
                            Debug.Log(z);
                            packet.Write(z);
                            packet.Write(currPUI);
                            packet.Send();
                        }
                    }
                });
            }
            GameObject closebtn = new GameObject("closebtn", new[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage), typeof(Button) });
            closebtn.transform.parent = playerSelectUI.transform;
            closebtn.GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
            GameObject closebtnText = new GameObject("closebtnText", new[] { typeof(TextMeshProUGUI) });
            closebtnText.transform.parent = closebtn.transform;
            closebtnText.GetComponent<TextMeshProUGUI>().text = "Close";
            closebtnText.GetComponent<TextMeshProUGUI>().fontSize = 16;
            closebtnText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
            closebtnText.GetComponent<TextMeshProUGUI>().autoSizeTextContainer = true;
            closebtnText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
            closebtn.GetComponent<Button>().onClick.RemoveAllListeners();
            closebtn.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                playerSelectUI.SetActive(false);
            });
        }
    }
}
