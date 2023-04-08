using System;
using System.Collections.Generic;
using TooltipSystem.Code;
using UnityEngine;

namespace Kovnir.TooltipSystem
{
    public sealed class TooltipSystem : MonoBehaviour
    {
        [SerializeField] private TooltipPopup tooltipPopup;

//        [SerializeField] private float offset;
        [SerializeField] private float showDelay = 1;
        [SerializeField] private float makeFixedDelay = 1;

        private static TooltipSystem instance;
        private TooltipsBase tooltipsBase;

        private readonly Dictionary<TooltipKeys, float> preparingTooltips = new();
        private readonly Dictionary<TooltipKeys, (TooltipPopup Popup, float MakeFixedAt)> shownTooltips = new();
        TooltipsFactory tooltipsFactory;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            tooltipsFactory = new TooltipsFactory(tooltipPopup, this.transform);
            tooltipsBase = TooltipsBase.Load();
        }

        public static void Show(TooltipKeys key) => instance.ShowInstance(key);

        public static void TryHide(TooltipKeys key) => instance.TryHideInstance(key);


        private void ShowInstance(TooltipKeys key)
        {
            preparingTooltips[key] = Time.time + showDelay;
        }

        private void ShowImmediate(TooltipKeys key)
        {
            if (tooltipsBase.TryGetTooltip(key, out TooltipsBase.TooltipRecord tooltip))
            {
                TooltipPopup newPopup = tooltipsFactory.Create();
                newPopup.Show(tooltip);
                shownTooltips[key] = (newPopup, Time.time + makeFixedDelay);
            }
            else
            {
                Debug.LogError($"Tooltip with key {key} not found");
            }
        }

        private void Update()
        {
            ProcessPreparing();
            foreach (KeyValuePair<TooltipKeys, (TooltipPopup Popup, float MakeFixedAt)> popupData in shownTooltips)
            {
                if (popupData.Value.MakeFixedAt <= Time.time)
                {
                    popupData.Value.Popup.MakeFixed();
                }
            }
        }

        private void ProcessPreparing()
        {
            List<TooltipKeys> toShow = new();
            foreach (var (key, timeToShow) in preparingTooltips)
            {
                if (preparingTooltips[key] <= Time.time)
                {
                    toShow.Add(key);
                }
            }

            foreach (TooltipKeys key in toShow)
            {
                ShowImmediate(key);
                preparingTooltips.Remove(key);
            }
        }

        private void TryHideInstance(TooltipKeys key)
        {
            if (preparingTooltips.ContainsKey(key))
            {
                preparingTooltips.Remove(key);
            }
            else
            {
                if (shownTooltips.ContainsKey(key))
                {
                    shownTooltips[key].Popup.Hide();
                    tooltipsFactory.ReturnToPool(shownTooltips[key].Popup);
                    shownTooltips.Remove(key);
                }
            }
        }
    }
}