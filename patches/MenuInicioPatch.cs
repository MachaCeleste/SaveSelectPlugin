using AssetBundleTools;
using HarmonyLib;
using SaveSelectPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

[HarmonyPatch]
public class MenuInicioPatch
{
    public static GameObject save;

    [HarmonyPatch(typeof(MenuInicio), "OnClickOpenMenu")]
    class OnClickOpenMenuPatch
    {
        static void Postfix()
        {
            if (!Networking.IsSinglePlayer()) return;
            if (save == null)
            {
                var help = GameObject.Find("MenuInicio/Help");
                save = Object.Instantiate(help, help.transform.parent);
                save.name = "Snapshot";
                var imageObj = save.transform.Find("Image");
                Object.DestroyImmediate(imageObj.GetComponent<Image>());
                var image = imageObj.gameObject.AddComponent<Image>();
                image.sprite = BundleTool.GetSprite("Assets/Export/snapshot.png");
                image.color = Color.black;
                var oldButton = save.GetComponent<Button>();
                var colors = oldButton.colors;
                Object.DestroyImmediate(oldButton);
                var button = save.AddComponent<Button>();
                button.colors = colors;
                button.onClick.AddListener(() =>
                {
                    help.transform.parent.gameObject.SetActive(false);
                    DataUtils.LoadGreyDbName();
                    DataUtils.SaveGame(DataUtils.lastSave);
                    OS.ShowError($"Saved snapshot \"{DataUtils.lastSave}\"!");
                });
                var text = save.transform.Find("TextMeshPro Text");
                Object.DestroyImmediate(text.GetComponent<ItemUITranslation>());
                text.GetComponent<TextMeshProUGUI>().text = "Snapshot";
            }
        }
    }
}