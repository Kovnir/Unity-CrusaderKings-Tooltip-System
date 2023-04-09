using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kovnir.TooltipSystem
{
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Canvas))]
    public sealed class TooltipPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const string NAME = "TooltipPopup";
        [SerializeField] private TooltipTitle title;
        [SerializeField] private TextMeshProUGUI description;

        [SerializeField] private uint characterWrapLimit = 50;
        
        [Range(0, 1)]
        [SerializeField] private float unfixedAlpha = 0.8f;
        [Range(0, 1)]
        [SerializeField] private float fixedAlpha = 1f;
        
        private LayoutElement layoutElement;
        private CanvasGroup canvasGroup;
        private GraphicRaycaster graphicRaycaster;
        private Canvas canvas;
        
        private Action<TooltipKeys> onEnter;
        private Action<TooltipKeys> onExit;
        private Action<string> onLinkHover;
        private Action<string> onLinkUnHover;

        private TooltipKeys currentKey;
        private string lastLinkId = string.Empty;
        public bool Focused { get; private set; }
        
        private void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
            canvasGroup = GetComponent<CanvasGroup>();
            graphicRaycaster = GetComponent<GraphicRaycaster>();
            canvas = GetComponent<Canvas>();
            if (title == null || description == null)
            {
                Debug.LogError("Title or description is null in TooltipPopup");
            }

            if (layoutElement == null)
            {
                Debug.LogError("LayoutElement is null in TooltipPopup");
            }
            
            if (canvasGroup == null)
            {
                Debug.LogError("CanvasGroup is null in TooltipPopup");
            }
            
            if (graphicRaycaster == null)
            {
                Debug.LogError("GraphicRaycaster is null in TooltipPopup");
            }
            
            if (canvas == null)
            {
                Debug.LogError("Canvas is null in TooltipPopup");
            }
        }

        private void Resize()
        {
            int titleLength = title.Length;
            int descriptionLength = description.text.Length;
            int totalLength = Mathf.Max(titleLength, descriptionLength);
            layoutElement.enabled = totalLength > characterWrapLimit;
        }

        public void Hide()
        {
            gameObject.name = $"{NAME} - Hidden";
            gameObject.SetActive(false);
        }

        public void Show(TooltipsBase.TooltipRecord tooltip, TooltipKeys key, int sortingOrder)
        {
            canvasGroup.alpha = unfixedAlpha;
            graphicRaycaster.enabled = false;
            onEnter = null;
            onExit = null;
            onLinkHover = null;
            onLinkUnHover = null;
            lastLinkId = string.Empty;
            Focused = false;
            
            canvas.sortingOrder = sortingOrder;
            currentKey = key;
            gameObject.SetActive(true);
            title.SetText(tooltip.Title);

            string descriptionText = tooltip.Description;
            Regex regex = new Regex(@"\[(.*?)\]\{(.*?)\}");
            if (regex.IsMatch(descriptionText))
            {
                descriptionText = regex.Replace(descriptionText, "<b><link=$2><#0000FF>$1</color></link></b>");
            }

            description.text = descriptionText;
            
            Resize();
            gameObject.name = $"{NAME} - Shown";
        }

        public void Update()
        {
            if (graphicRaycaster.enabled)
            {
                var linkIndex = TMP_TextUtilities.FindIntersectingLink(description, Input.mousePosition, null);
                if (linkIndex >= 0)
                {
                    var linkInfo = description.textInfo.linkInfo[linkIndex];
                    string linkID = linkInfo.GetLinkID();
                    if (linkID != lastLinkId)
                    {
                        lastLinkId = linkID;
                        onLinkHover?.Invoke(linkID);
                    }
                }
                else
                {
                    if (lastLinkId != string.Empty)
                    {
                        onLinkUnHover?.Invoke(lastLinkId);
                        lastLinkId = string.Empty;
                    }
                }
            }
        }

        public void MakePreFixed(Action<TooltipKeys> onEnter, Action<TooltipKeys> onExit)
        {
            this.onExit = onExit;
            this.onEnter = onEnter;
            canvasGroup.alpha = fixedAlpha;
            graphicRaycaster.enabled = true;
            gameObject.name = $"{NAME} - PreFixed";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Focused = true;
            onEnter?.Invoke(currentKey);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Focused = false;
            onExit?.Invoke(currentKey);
        }

        public void MakeFixed(Action<string> onLinkHover, Action<string> onLinkUnHover)
        {
            this.onLinkHover = onLinkHover;
            this.onLinkUnHover = onLinkUnHover;
            gameObject.name = $"{NAME} - Fixed";
        }
    }
}