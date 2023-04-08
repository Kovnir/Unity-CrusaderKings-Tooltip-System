using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Kovnir.TooltipSystem.Edtior
{
    [CustomEditor(typeof(TooltipsBase))]
    public sealed class TooltipsBaseEditor : Editor
    {
        private const string TOOLTIP_KEYS_FILE = "TooltipKeys.cs"; 
        public override void OnInspectorGUI()
        {
            var tooltipsBase = target as TooltipsBase;
            string errors = tooltipsBase.Validate();
            if (!string.IsNullOrEmpty(errors))
            {
                EditorGUILayout.HelpBox(errors, MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("No errors", MessageType.Info);
            }

            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(!string.IsNullOrEmpty(errors));
            if (GUILayout.Button("Generate Code"))
            {
                GeneratedCode(tooltipsBase);
            }

            EditorGUI.EndDisabledGroup();
        }

        private void GeneratedCode(TooltipsBase tooltipsBase)
        {
            List<string> keys = tooltipsBase.GetKeys();

            StringBuilder code = new StringBuilder();
            code.AppendLine("//Don't edit this file, it's generated automatically");
            code.AppendLine("namespace Kovnir.TooltipSystem");
            code.AppendLine("{");
            code.AppendLine("    public enum TooltipKeys");
            code.AppendLine("    {");
            foreach (string key in keys)
            {
                code.AppendLine($"        {key},");
            }

            code.AppendLine("    }");
            code.AppendLine("}");

            string[] foundAssets = AssetDatabase.FindAssets(TOOLTIP_KEYS_FILE.Replace(".cs", ""));
            string path;
            if (foundAssets.Length == 0)
            {
                path = EditorUtility.SaveFilePanel($"Save {TOOLTIP_KEYS_FILE}", Application.dataPath, TOOLTIP_KEYS_FILE,
                    "cs");
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                path = path.Substring(0, path.LastIndexOf('/'));
                path += $"/{TOOLTIP_KEYS_FILE}";
            }
            else
            {
                path = foundAssets[0];
                path = AssetDatabase.GUIDToAssetPath(path);
            }

            System.IO.File.WriteAllText(path, code.ToString());
            AssetDatabase.Refresh();
        }
    }
}