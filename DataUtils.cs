using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SaveSelectPlugin
{
    public class DataUtils
    {
        public static string lastSave;
        private static string saveFolder = Path.Combine(Application.dataPath, "Saves");

        public static void SaveGreyDbName()
        {
            var path = Path.Combine(saveFolder, "LastDb.txt");
            File.WriteAllText(path, lastSave);
        }

        public static void LoadGreyDbName()
        {
            var path = Path.Combine(saveFolder, "LastDb.txt");
            if (File.Exists(path))
                lastSave = File.ReadAllText(path);
        }

        public static string GetPathFromName(string fileName)
        {
            return Path.Combine(saveFolder, $"{fileName}.db");
        }

        public static List<GameSave> GetSaves()
        {
            CheckSaveFolder();
            List<GameSave> saves = new List<GameSave>();
            var current = GetCurrentSave();
            if (current != null) saves.Add(current);
            foreach (string save in Directory.GetFiles(saveFolder, "*.db"))
            {
                saves.Add(new GameSave(save));
            }
            if (saves.Count == 0)
                return null;
            return saves;
        }

        public static GameSave GetCurrentSave()
        {
            var file = Path.Combine(Application.dataPath, "GreyHackDB.db");
            if (File.Exists(file))
            {
                var current = new GameSave(file);
                current.Current = true;
                LoadGreyDbName();
                if (!string.IsNullOrEmpty(lastSave))
                    current.FileName = lastSave;
                return current;
            }
            return null;
        }

        public static GameSave NewGame(string fileName)
        {
            CheckSaveFolder();
            var file = Path.Combine(Application.dataPath, "GreyHackDB.db");
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogError(ex.ToString());
                }
            }
            return new GameSave(GetPathFromName(fileName));
        }

        public static void LoadGame(string filePath)
        {
            CheckSaveFolder();
            try
            {
                File.Copy(filePath, Path.Combine(Application.dataPath, "GreyHackDB.db"), true);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError(ex.ToString());
            }
        }

        public static void SaveGame(string fileName)
        {
            CheckSaveFolder();
            var file = Path.Combine(Application.dataPath, "GreyHackDB.db");
            try
            {
                File.Copy(file, GetPathFromName(fileName), true);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError(ex.ToString());
            }
        }

        public static void DeleteSave(string fileName, bool current = false)
        {
            CheckSaveFolder();
            var file = GetPathFromName(fileName);
            if (current)
                file = Path.Combine(Application.dataPath, "GreyHackDB.db");
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError(ex.ToString());
            }
        }

        private static void CheckSaveFolder()
        {
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
        }

        public static bool CheckVersion(string savePath, out string error)
        {
            Type gameConfigType = AccessTools.TypeByName("Util.GameConfig");
            FieldInfo versionDeleteDBField = gameConfigType?.GetField("VersionDeleteDB", BindingFlags.Public | BindingFlags.Static);
            int clientDbVer = (int)versionDeleteDBField.GetValue(null);
            error = "";
            int saveVer = MyDatabase.Singleton.GetDbVersion(savePath);
            if (clientDbVer != saveVer)
            {
                error = $"This save is not for this version of the game!\nClientDB: v{clientDbVer} - SaveDB: v{saveVer}";
                return false;
            }
            return true;
        }
    }

    public class GameSave
    {
        public bool Current { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastSave { get; set; }

        public GameSave(string path)
        {
            FilePath = path;
            FileName = Path.GetFileNameWithoutExtension(path);
            if (File.Exists(path))
            {
                Created = File.GetCreationTime(path);
                LastSave = File.GetLastWriteTime(path);
            }
        }
    }
}