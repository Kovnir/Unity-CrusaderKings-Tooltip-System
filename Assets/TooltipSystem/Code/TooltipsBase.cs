using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kovnir.TooltipSystem
{
    [CreateAssetMenu(menuName = "Create TooltipsBase", fileName = "TooltipsBase", order = 0)]
    public sealed class TooltipsBase : ScriptableObject
    {
        [Serializable]
        private struct TooltipData
        {
            public string Key;
            public string Title;
            [TextArea]
            public string Description;
        }

        public sealed record TooltipRecord(string Title, string Description);

        [SerializeField] private List<TooltipData> tooltipsData;

        private static TooltipsBase instance;
        private Dictionary<TooltipKeys, TooltipRecord> tooltipsRuntimeData;

        
        public string Validate()
        {
            var errors = new List<string>();
            var tooltips = new HashSet<string>();
            for (int index = 0; index < tooltipsData.Count; index++)
            {
                TooltipData tooltipData = tooltipsData[index];
                if (string.IsNullOrEmpty(tooltipData.Key))
                {
                    errors.Add($"Key is empty for index {index}");
                    continue;
                }

                if (string.IsNullOrEmpty(tooltipData.Description))
                {
                    errors.Add($"Description is empty for key {tooltipData.Key}");
                    continue;
                }

                if (tooltips.Contains(tooltipData.Key))
                {
                    errors.Add($"Key {tooltipData.Key} is duplicated");
                    continue;
                }

                tooltips.Add(tooltipData.Key);
            }

            return string.Join(Environment.NewLine, errors);
        }

        public List<string> GetKeys()
        {
            List<string> keys = new();
            foreach (TooltipData tooltipData in tooltipsData)
            {
                keys.Add(tooltipData.Key);
            }

            return keys;
        }

        private void Init()
        {
            tooltipsRuntimeData = new Dictionary<TooltipKeys, TooltipRecord>();
            foreach (TooltipData tooltipData in tooltipsData)
            {
                TooltipKeys key = (TooltipKeys) Enum.Parse(typeof(TooltipKeys), tooltipData.Key);
                tooltipsRuntimeData.Add(key, new TooltipRecord(tooltipData.Title, tooltipData.Description));
            }
        }
        
        public static TooltipsBase Load()
        {
            if (instance != null)
            {
                return instance;
            }
            instance = Resources.Load<TooltipsBase>("TooltipsBase");
            instance.Init();
            return instance;
        }

        public bool TryGetTooltip(TooltipKeys key, out TooltipRecord tooltip)
        {
            return tooltipsRuntimeData.TryGetValue(key, out tooltip);
        }
    }
}