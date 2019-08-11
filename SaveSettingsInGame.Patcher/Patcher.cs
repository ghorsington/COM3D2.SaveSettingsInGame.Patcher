using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace SaveSettingsInGame.Patcher
{
    public static class Patcher
    {
        public static readonly string[] TargetAssemblyNames = { "Assembly-CSharp.dll" };

        public static void Patch(AssemblyDefinition ad)
        {
            var md = ad.MainModule;
            var methodDef = md.GetType("ConfigMgr").Methods.FirstOrDefault(m => m.Name == "CloseConfigPanel");
            var ins = methodDef.Body.Instructions.Last();
            var il = methodDef.Body.GetILProcessor();
            il.InsertBefore(ins, il.Create(OpCodes.Call, md.ImportReference(typeof(Hooks).GetMethod(nameof(Hooks.SaveConfig)))));

            methodDef = md.GetType("CMSystem").Methods.FirstOrDefault(m => m.Name == "SetEditColorPresetData");
            ins = methodDef.Body.Instructions.Last();
            il = methodDef.Body.GetILProcessor();
            il.InsertBefore(ins, il.Create(OpCodes.Call, md.ImportReference(typeof(Hooks).GetMethod(nameof(Hooks.SaveSystemConfig)))));

            methodDef = md.GetType("CMSystem").Methods.FirstOrDefault(m => m.Name == "SetSystemVers");
            ins = methodDef.Body.Instructions.Last();
            il = methodDef.Body.GetILProcessor();
            il.InsertBefore(ins, il.Create(OpCodes.Call, md.ImportReference(typeof(Hooks).GetMethod(nameof(Hooks.SaveSystemConfig)))));
        }
    }

    public static class Hooks
    {
        public static void SaveSystemConfig()
        {
            Debug.Log("Saving system settings");
            GameMain.Instance.CMSystem.SaveSystem();
        }

        public static void SaveConfig()
        {
            Debug.Log("Saving settings");
            GameMain.Instance.CMSystem.SaveIni();
        }
    }
}
