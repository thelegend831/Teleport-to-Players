using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using Unity;
using UnityEngine;
using Harmony;
using NET_SDK;
using UnityEngine.SceneManagement;
using UnhollowerBaseLib;
using UnityEngine.Experimental.PlayerLoop;
using VRC.Core;
using VRC;
using System.Reflection;
using System.Runtime.InteropServices;
using VRCSDK2;
using Il2CppSystem;
using IntPtr = System.IntPtr;
using ConsoleColor = System.ConsoleColor;
using UnhollowerRuntimeLib;
using IL2CPP = UnhollowerBaseLib.IL2CPP;
using UnityEngine.UI;
using VRC.UI;

namespace Teleport
{
    internal class tpm : MelonMod
    {
        private Transform AddMenuButton(string butName, Transform parent, string butText, string tooltip, int butX, int butY, System.Action butAction)
        {
            Transform quickMenu = QuickMenu.prop_QuickMenu_0.transform;

            // clone of a standard button
            Transform butTransform = UnityEngine.Object.Instantiate(quickMenu.Find("CameraMenu/BackButton").gameObject).transform;

            // Set internal name of button
            butTransform.name = butName;

            // set button's parent to quick menu
            butTransform.SetParent(parent, false);

            // set button's text
            butTransform.GetComponentInChildren<Text>().text = butText;
            butTransform.GetComponent<UiTooltip>().text = tooltip;
            butTransform.GetComponent<UiTooltip>().alternateText = tooltip;

            // set position of new button based on existing menu buttons
            float buttonWidth = quickMenu.Find("UserInteractMenu/ForceLogoutButton").localPosition.x - quickMenu.Find("UserInteractMenu/BanButton").localPosition.x;
            float buttonHeight = quickMenu.Find("UserInteractMenu/ForceLogoutButton").localPosition.x - quickMenu.Find("UserInteractMenu/BanButton").localPosition.x;
            butTransform.localPosition = new Vector3(butTransform.localPosition.x + buttonWidth * butX, butTransform.localPosition.y + buttonHeight * butY, butTransform.localPosition.z);

            // Make it so the button does what we want
            butTransform.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            butTransform.GetComponent<Button>().onClick.AddListener(butAction);

            // enable it just in case
            butTransform.gameObject.SetActive(true);

            return butTransform;
        }

        public static Player GetPlayer(string UserID)
        {
            var Players = PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
            Player FoundPlayer = null;
            for (int i = 0; i < Players.Count; i++)
            {
                var player = Players[i];
                if (player.field_Private_APIUser_0.id == UserID)
                {
                    FoundPlayer = player;
                }
            }

            return FoundPlayer;
        }

        public override void OnApplicationStart()
        {
            MelonModLogger.Log("Teleport to Players mod started");
        }

        public override void VRChat_OnUiManagerInit()
        {
            var playerManager = PlayerManager.Method_Public_Static_PlayerManager_0();

            this.AddMenuButton("tpQuickMenu", QuickMenu.prop_QuickMenu_0.transform.Find("UserInteractMenu"), "<color=white>Teleport to Player</color>", "Teleports you to the selected player", 0, 0, new System.Action(() =>
            {
                MelonModLogger.Log("Teleporting to player");
                var player = VRCPlayer.field_Internal_Static_VRCPlayer_0;
                var APIUser = QuickMenu.prop_QuickMenu_0.field_Private_APIUser_0;
                var SelectedPlayer = GetPlayer(APIUser.id);
                player.transform.position = SelectedPlayer.transform.position;
            }));

            GameObject screens = GameObject.Find("Screens");
            Transform playlistsPanel = screens.transform.Find("UserInfo/User Panel/Playlists");
            Transform ourBut = UnityEngine.Object.Instantiate(screens.transform.Find("UserInfo/User Panel/Playlists/PlaylistsButton").gameObject).transform;
            ourBut.name = "tpSocialMenu";
            ourBut.SetParent(playlistsPanel, false);
            ourBut.GetComponentInChildren<Text>().text = "<color=white>Teleport to</color>";
            ourBut.localPosition = new Vector3(ourBut.localPosition.x, ourBut.localPosition.y + 76.0f, ourBut.localPosition.z);
            ourBut.transform.Find("Image/Icon_New").transform.gameObject.SetActive(false);
            ourBut.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ourBut.GetComponent<Button>().onClick.AddListener(new System.Action(() =>
            {
                MelonModLogger.Log("Teleporting to player");
                var player = VRCPlayer.field_Internal_Static_VRCPlayer_0;
                player.transform.position = GetPlayer(screens.transform.Find("UserInfo").transform.GetComponentInChildren<PageUserInfo>().user.id).transform.position;
                VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_Boolean_0(false);
            }));
            ourBut.gameObject.SetActive(true);
        }
    }
}