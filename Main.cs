using MelonLoader;
using UnityEngine;
using VRC.Core;
using VRC;
using UnityEngine.UI;
using VRC.UI;

namespace Teleport
{
    internal class Tpm : MelonMod
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
        public static void ShowDialog(string title, string message)
        {
            Resources.FindObjectsOfTypeAll<VRCUiPopupManager>()[0].Method_Public_Void_String_String_Single_0(title, message, 10f);
        }
        public static void CloseMenu()
        {
            VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_Boolean_4(false);
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

            this.AddMenuButton("tpQuickMenu", QuickMenu.prop_QuickMenu_0.transform.Find("UserInteractMenu"), "<color=white>Teleport to Player</color>", "Teleports you to the selected player", -1, -1, new System.Action(() =>
            {
                VRCPlayer player = VRCPlayer.field_Internal_Static_VRCPlayer_0;
                APIUser APIUser = QuickMenu.prop_QuickMenu_0.field_Private_APIUser_0;
                if (player.prop_Player_0.field_Private_APIUser_0.id == APIUser.id)
                {
                    ShowDialog("<color=red>Error!</color>", "<color=white>You can't teleport to yourself! You're already here!</color>");
                    return;
                }
                Player foundPlayer = GetPlayer(APIUser.id);
                if (foundPlayer == null)
                {
                    ShowDialog("<color=red>Error!</color>", "<color=white>Player is not in the current instance.</color>");
                    return;
                }
                MelonModLogger.Log("Teleporting to player");
                player.transform.position = foundPlayer.transform.position;
            }));

            GameObject screens = GameObject.Find("Screens");
            Transform playlistsPanel = screens.transform.Find("UserInfo/User Panel/Playlists");
            Transform ourBut = UnityEngine.Object.Instantiate(screens.transform.Find("UserInfo/User Panel/Playlists/PlaylistsButton").gameObject).transform;
            ourBut.name = "tpSocialMenu";
            ourBut.SetParent(playlistsPanel, false);
            ourBut.GetComponentInChildren<Text>().text = "<color=white>Teleport to</color>";
            ourBut.localPosition = new Vector3(ourBut.localPosition.x - 1267.0f, ourBut.localPosition.y - 315.0f, ourBut.localPosition.z);
            ourBut.transform.Find("Image/Icon_New").transform.gameObject.SetActive(false);
            RectTransform ourButRectTransform = ourBut.GetComponentInChildren<RectTransform>();
            ourButRectTransform.sizeDelta = new Vector2(ourButRectTransform.sizeDelta.x - 75, ourButRectTransform.sizeDelta.y);
            ourBut.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ourBut.GetComponent<Button>().onClick.AddListener(new System.Action(() =>
            {
                VRCPlayer player = VRCPlayer.field_Internal_Static_VRCPlayer_0;
                string toPlayerId = screens.transform.Find("UserInfo").transform.GetComponentInChildren<PageUserInfo>().user.id;
                if (player.prop_Player_0.field_Private_APIUser_0.id == toPlayerId)
                {
                    ShowDialog("<color=red>Error!</color>", "<color=white>You can't teleport to yourself! You're already here!</color>");
                    return;
                }
                Player foundPlayer = GetPlayer(toPlayerId);
                if (foundPlayer == null)
                {
                    ShowDialog("<color=red>Error!</color>", "<color=white>Player is not in the current instance.</color>");
                    return;
                }
                MelonModLogger.Log("Teleporting to player");
                player.transform.position = foundPlayer.transform.position;
                CloseMenu();
            }));
            ourBut.gameObject.SetActive(true);
        }
    }
}