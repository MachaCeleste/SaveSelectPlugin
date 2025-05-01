using HarmonyLib;
using SaveSelectPlugin;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch]
public class BiosMenuPatch
{
    [HarmonyPatch(typeof(BiosMenu), "Start")]
    class StartPatch
    {
        static void Postfix(BiosMenu __instance)
        {
            var myPanel = DataUtils.GetPrefab("Assets/Export/SavePanel.prefab");
            var savePanel = UnityEngine.Object.Instantiate(myPanel, GameObject.Find("MainMenu/MainPanel/MidPanel/PanelGame/PanelOptions/PlayGame").transform);
            savePanel.name = "SavePanel";
            savePanel.AddComponent<SavePanel>();
            var saveAddButton = savePanel.transform.Find("ScrollView/Viewport/Content/AddSave").GetComponent<Button>();
            saveAddButton.onClick.AddListener(() => OnAddSaveClick());
        }

        private static void OnAddSaveClick()
        {
            var savePanel = GameObject.Find("MainMenu/MainPanel/MidPanel/PanelGame/PanelOptions/PlayGame/SavePanel").GetComponent<SavePanel>();
            if (savePanel.items.Count > 0)
            {
                var newSave = savePanel.items.FirstOrDefault(x => x.isNewSave == true);
                if (newSave != null)
                    return;
            }
            savePanel.items.Add(new SavePanelItem(selected: true));
        }
    }

    [HarmonyPatch(typeof(BiosMenu), "OnSelectSinglePlayer")]
    class OnSelectSinglePlayerPatch
    {
        static void Postfix(BiosMenu __instance)
        {
            var deletePanel = GameObject.Find("MainMenu/MainPanel/MidPanel/PanelGame/PanelOptions/PlayGame/InfoDeletePlayer");
            deletePanel.SetActive(false);
            var test = GameObject.FindObjectOfType<SavePanel>(true).gameObject;
            var anim = test.GetComponent<Animator>();
            anim.ResetTrigger("Reset");
            anim.SetTrigger("SlideIn");
        }
    }

    [HarmonyPatch(typeof(BiosMenu), "OnSelectOnlineMode")]
    class OnSelectOnlineModePatch
    {
        static void Postfix(BiosMenu __instance)
        {
            var deletePanel = GameObject.Find("MainMenu/MainPanel/MidPanel/PanelGame/PanelOptions/PlayGame/InfoDeletePlayer");
            deletePanel.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(BiosMenu), "OnBackOnlineMode")]
    class OnBackOnlineModePatch
    {
        static void Postfix(BiosMenu __instance)
        {
            var savePanel = GameObject.FindObjectOfType<SavePanel>(true).gameObject;
            if (savePanel != null)
                savePanel.GetComponent<Animator>().SetTrigger("Reset");
        }
    }

    [HarmonyPatch(typeof(BiosMenu), "OnSinglePlayer")]
    class OnSinglePlayerPatch
    {
        static bool Prefix(BiosMenu __instance)
        {
            var savePanel = GameObject.Find("MainMenu/MainPanel/MidPanel/PanelGame/PanelOptions/PlayGame/SavePanel").GetComponent<SavePanel>();
            if (savePanel.items != null && savePanel.items.Count > 0)
            {
                var save = savePanel.items.First(x => x.isSelected.isOn == true);
                if (save != null)
                {
                    PlayerPrefs.SetInt("user_force_gameover", save.shouldWipe.isOn ? 1 : 0);
                    var saveFile = save._save;
                    if (saveFile == null)
                    {
                        if (!save.GenSave())
                        {
                            __instance.panelsOptions[6].SetActive(true);
                            __instance.connect_text.text = "Error: Snapshot name cannot be blank!";
                            __instance.objBackButtonConn.SetActive(true);
                            return false;
                        }
                        return true;
                    }
                    new MyDatabase();
                    if (!DataUtils.CheckVersion(saveFile.FilePath, out string error))
                    {
                        __instance.panelsOptions[6].SetActive(true);
                        __instance.connect_text.text = $"Error: {error}";
                        __instance.objBackButtonConn.SetActive(true);
                        MyDatabase.Singleton = null;
                        return false;
                    }
                    MyDatabase.Singleton = null;
                    DataUtils.lastSave = saveFile.FileName;
                    DataUtils.SaveGreyDbName();
                    if (!saveFile.Current)
                        DataUtils.LoadGame(saveFile.FilePath);
                    return true;
                }
                __instance.panelsOptions[6].SetActive(true);
                __instance.connect_text.text = "Error: No snapshot selected!";
                __instance.objBackButtonConn.SetActive(true);
                return false;
            }
            return true;
        }
    }
}