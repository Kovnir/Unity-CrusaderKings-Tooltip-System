using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kovnir.TooltipSystem
{
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
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
        
        private Action<TooltipKeys> onEnterFixedPopup;
        private TooltipKeys currentKey;
        private Action<TooltipKeys> onExitFixedPopup;

        private void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
            canvasGroup = GetComponent<CanvasGroup>();
            graphicRaycaster = GetComponent<GraphicRaycaster>();
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

        public void Show(TooltipsBase.TooltipRecord tooltip, TooltipKeys key)
        {
            canvasGroup.alpha = unfixedAlpha;
            graphicRaycaster.enabled = false;
            onEnterFixedPopup = null;
            onExitFixedPopup = null;
            
            currentKey = key;
            gameObject.SetActive(true);
            title.SetText(tooltip.Title);
            description.text = tooltip.Description;
            Resize();
            gameObject.name = $"{NAME} - Shown";
        }
        
        public void MakePreFixed(Action<TooltipKeys> onEnterFixedPopup, Action<TooltipKeys> onExitFixedPopup)
        {
            this.onExitFixedPopup = onExitFixedPopup;
            this.onEnterFixedPopup = onEnterFixedPopup;
            canvasGroup.alpha = fixedAlpha;
            graphicRaycaster.enabled = true;
            gameObject.name = $"{NAME} - PreFixed";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onEnterFixedPopup?.Invoke(currentKey);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExitFixedPopup?.Invoke(currentKey);
        }

        public void MakeFixed()
        {
            gameObject.name = $"{NAME} - Fixed";
        }
    }
}