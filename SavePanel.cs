using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaveSelectPlugin
{
    public class SavePanel : MonoBehaviour
    {
        public List<SavePanelItem> items;

        private List<GameSave> saves;

        public SavePanel()
        {
            items = new List<SavePanelItem>(); 
            saves = DataUtils.GetSaves();
            if (saves != null)
            {
                foreach (var save in saves)
                {
                    items.Add(new SavePanelItem(save));
                }
            }
        }
    }

    public class SavePanelItem
    {
        public Toggle isSelected;
        public Toggle shouldWipe;
        public GameSave _save;
        public bool isNewSave = true;

        private GameObject _saveItem;
        private TMP_InputField _saveNameInputField;
        private readonly string format = "MM/dd/yyyy HH:mm";

        public SavePanelItem(GameSave save = null, bool selected = false)
        {
            var parent = GameObject.Find("SavePanel/ScrollView/Viewport/Content").transform;
            var template = GameObject.Find("SavePanel/ScrollView/Viewport/Content/SaveTemplate");
            _saveItem = Object.Instantiate(template, parent);
            _saveItem.SetActive(true);
            var addSave = GameObject.Find("SavePanel/ScrollView/Viewport/Content/AddSave");
            addSave.transform.SetAsLastSibling();
            isSelected = _saveItem.transform.Find("Top/Select").GetComponent<Toggle>();
            if (selected) isSelected.isOn = true;
            _saveNameInputField = _saveItem.transform.Find("Top/NameInput").GetComponent<TMP_InputField>();
            shouldWipe = _saveItem.transform.Find("Top/Wipe").GetComponent<Toggle>();
            _saveItem.transform.Find("Top/Delete").GetComponent<Button>().onClick.AddListener(() => { OnDeleteWorldClicked(); });
            if (save != null)
            {
                isNewSave = false;
                _save = save;
                _saveNameInputField.text = save.FileName;
                _saveNameInputField.interactable = false;
                if (_save.Current) isSelected.transform.Find("Background").GetComponent<Image>().color = Color.red;
                _saveItem.transform.Find("Bottom/DateCreated").GetComponent<TextMeshProUGUI>().text = _save.Created.ToString(format);
                _saveItem.transform.Find("Bottom/DateSaved").GetComponent<TextMeshProUGUI>().text = _save.LastSave.ToString(format);
            }
        }

        public bool GenSave()
        {
            var name = _saveNameInputField.text;
            if (string.IsNullOrEmpty(name)) return false;
            _save = DataUtils.NewGame(name);
            DataUtils.lastSave = name;
            DataUtils.SaveGreyDbName();
            return true;
        }

        private void OnDeleteWorldClicked()
        {
            if (_save != null)
            {
                DataUtils.DeleteSave(_save.FileName, _save.Current);
            }
            Object.Destroy(_saveItem);
            var savePanel = GameObject.Find("MainMenu/MainPanel/MidPanel/PanelGame/PanelOptions/PlayGame/SavePanel").GetComponent<SavePanel>();
            savePanel.items.Remove(this);
        }
    }
}