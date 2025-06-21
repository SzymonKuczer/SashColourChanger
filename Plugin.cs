using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using System.Reflection;

namespace SashColourChanger;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private int currentIndex = 0;

    private MethodInfo getCustomizationDataMethod;
    private MethodInfo setCustomizationDataMethod;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var customizationType = typeof(CharacterCustomization);

        getCustomizationDataMethod = customizationType.GetMethod("GetCustomizationData", BindingFlags.NonPublic | BindingFlags.Static);
        setCustomizationDataMethod = customizationType.GetMethod("SetCustomizationData", BindingFlags.NonPublic | BindingFlags.Static);

        if (getCustomizationDataMethod == null || setCustomizationDataMethod == null)
        {
            Logger.LogError("Borked");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))  
        {
            var player = PhotonNetwork.LocalPlayer;
            if (player == null)
            {
                Logger.LogError("No local player found :( ");
                return;
            }

            if (getCustomizationDataMethod == null || setCustomizationDataMethod == null)
            {
                Logger.LogError("Customization methods not found :(");
                return;
            }

            
            var customizationData = (CharacterCustomizationData)getCustomizationDataMethod.Invoke(null, new object[] { player });

            customizationData.currentSash = currentIndex;

            
            setCustomizationDataMethod.Invoke(null, new object[] { customizationData, player });

            Logger.LogInfo($"Sash color changed to index {currentIndex}");

            currentIndex = (currentIndex + 1) % 9;
        }
    }
}