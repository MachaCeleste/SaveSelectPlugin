using BepInEx;
using System.IO;
using System.Reflection;
using UnityEngine;
using SaveSelectPlugin;

namespace AssetBundleTools
{
    public class BundleTool
    {
        public static AssetBundle bundle;

        public static void LoadBundle()
        {
            string bundlePath = Path.Combine(Paths.PluginPath, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), MyPluginInfo.PLUGIN_NAME + ".bundle");
            if (!File.Exists(bundlePath))
            {
                Debug.LogError($"[Info   :{MyPluginInfo.PLUGIN_NAME}] Bundle not found!");
                return;
            }
            bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                Debug.LogError($"[Info   :{MyPluginInfo.PLUGIN_NAME}] Failed to load bundle!");
                return;
            }
            Debug.Log($"[Info   :{MyPluginInfo.PLUGIN_NAME}] Loaded bundle");
        }

        public static GameObject GetPrefab(string path)
        {
            var prefab = bundle.LoadAsset<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[Info   :{MyPluginInfo.PLUGIN_NAME}] Failed to load prefab {path}");
                return null;
            }
            return prefab;
        }

        public static AudioClip GetClip(string path)
        {
            var clip = bundle.LoadAsset<AudioClip>(path);
            if (clip == null)
            {
                Debug.LogError($"[Info   :{MyPluginInfo.PLUGIN_NAME}] Failed to load audio clip {path}");
                return null;
            }
            return clip;
        }

        public static Sprite GetSprite(string path)
        {
            var sprite = bundle.LoadAsset<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError($"[Info   :{MyPluginInfo.PLUGIN_NAME}] Failed to load sprite {path}");
                return null;
            }
            return sprite;
        }
    }
}