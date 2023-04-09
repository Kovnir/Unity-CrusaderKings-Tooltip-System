using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TooltipSystem.Code;
using TooltipSystem.Code.PopupState;
using UnityEngine;

namespace Kovnir.TooltipSystem
{
    public sealed class TooltipSystem : MonoBehaviour
    {
        private const int DEFAULT_SORTING = 32000;
        [SerializeField] private TooltipPopup tooltipPopup;

        [SerializeField] private float showDelay = 1;
        [SerializeField] private float makeFixedDelay = 1;

        private static TooltipSystem instance;
        private TooltipsBase tooltipsBase;

        private readonly Stack<(TooltipKeys Key, TooltipPopup Popup, TooltipState.TooltipStateBase state)>
            shownTooltips = new();

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

        public static void TryHide(TooltipKeys key)
        {
            instance.StartCoroutine(NextFrame(() =>
            {
                instance.TryHideInstance(key);
            }));
        }

        //todo make it more elegant
        //let UI be recalculated
        private static IEnumerator NextFrame(Action nextFrameCallback)
        {
            yield return null;
            nextFrameCallback();
        }


        private void ShowInstance(TooltipKeys key)
        {
            shownTooltips.Push((key, null, TooltipState.CreatePreparing(Time.time + showDelay)));
        }

        private void ShowImmediate(TooltipKeys key)
        {
            if (tooltipsBase.TryGetTooltip(key, out TooltipsBase.TooltipRecord tooltip))
            {
                TooltipPopup newPopup = tooltipsFactory.Create();
                newPopup.Show(tooltip, key, DEFAULT_SORTING + shownTooltips.Count + 1);
                shownTooltips.Push((key, newPopup, TooltipState.CreateShown(Time.time + makeFixedDelay)));
            }
            else
            {
                Debug.LogError($"Tooltip with key {key} not found");
            }
        }

        private void Update()
        {
            ProcessPreparing();
            ProcessShown();
        }

        private void ProcessShown()
        {
            if (shownTooltips.Any())
            {
                (TooltipKeys Key, TooltipPopup Popup, TooltipState.TooltipStateBase state) data = shownTooltips.Peek();

                if (data.state is TooltipState.Shown shown)
                {
                    if (shown.MakePreFixedAt <= Time.time)
                    {
                        shownTooltips.Pop();
                        shownTooltips.Push((data.Key, data.Popup, TooltipState.CreatePreFixed()));
                        data.Popup.MakePreFixed(OnEnterFixedPopup, OnExitFixedPopup);
                    }
                }
            }
        }

        private void OnEnterFixedPopup(TooltipKeys tooltipKeys)
        {
            (TooltipKeys Key, TooltipPopup Popup, TooltipState.TooltipStateBase state) data = shownTooltips.Peek();
            if (data.Key == tooltipKeys)
            {
                shownTooltips.Pop();
                data.Popup.MakeFixed((s) =>
                {
                    if (TooltipKeys.TryParse(s, out TooltipKeys key))
                    {
                        ShowInstance(key);
                    }
                    else
                    {
                        Debug.LogError($"Tooltip key is not valid {s}");
                    }
                }, (s) =>
                {
                    if (TooltipKeys.TryParse(s, out TooltipKeys key))
                    {
                        TryHideInstance(key);
                    }
                    else
                    {
                        Debug.LogError($"Tooltip key is not valid {s}");
                    }
                });
                shownTooltips.Push((data.Key, data.Popup, TooltipState.CreateFixed()));
            }
        }

        private void OnExitFixedPopup(TooltipKeys tooltipKeys)
        {
            (TooltipKeys Key, TooltipPopup Popup, TooltipState.TooltipStateBase state) data = shownTooltips.Peek();
            if (data.Key == tooltipKeys)
            {
                HideAndRemovePopup(data.Popup);
            }
        }

        private void HideAndRemovePopup(TooltipPopup popup)
        {
            shownTooltips.Pop();
            popup.Hide();
            tooltipsFactory.ReturnToPool(popup);
            if (shownTooltips.Any())
            {
                StartCoroutine(NextFrame(TryHideNextFixed));
            }
        }

        private void ProcessPreparing()
        {
            if (shownTooltips.Any())
            {
                (TooltipKeys Key, TooltipPopup Popup, TooltipState.TooltipStateBase state) data = shownTooltips.Peek();

                if (data.state is TooltipState.Preparing preparing)
                {
                    if (preparing.ShowAt <= Time.time)
                    {
                        shownTooltips.Pop();
                        ShowImmediate(data.Key);
                    }
                }
            }
        }

        private void TryHideInstance(TooltipKeys key)
        {
            if (shownTooltips.Any())
            {
                (TooltipKeys Key, TooltipPopup Popup, TooltipState.TooltipStateBase State) data = shownTooltips.Peek();
                if (data.Key == key)
                {
                    if (data.State is TooltipState.Preparing)
                    {
                        shownTooltips.Pop();
                    }
                    else if (data.State is TooltipState.Shown || data.State is TooltipState.PreFixed)
                    {
                        HideAndRemovePopup(data.Popup);
                    }
                }
            }
        }  
        private void TryHideNextFixed()
        {
            if (shownTooltips.Any())
            {
                (TooltipKeys Key, TooltipPopup Popup, TooltipState.TooltipStateBase State) data = shownTooltips.Peek();
                if (!data.Popup.Focused)
                {
                    if (data.State is TooltipState.Fixed)
                    {
                        HideAndRemovePopup(data.Popup);
                    }
                }
            }
        }
    }
}