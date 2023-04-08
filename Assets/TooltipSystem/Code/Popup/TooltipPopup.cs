using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kovnir.TooltipSystem
{
    [RequireComponent(typeof(LayoutElement))]
    public sealed class TooltipPopup : MonoBehaviour
    {
        [SerializeField] private TooltipTitle title;
        [SerializeField] private TextMeshProUGUI description;

        [SerializeField] private uint characterWrapLimit = 50;

        private LayoutElement layoutElement;

        private void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
            if (title == null || description == null)
            {
                Debug.LogError("Title or description is null in TooltipPopup");
            }

            if (layoutElement == null)
            {
                Debug.LogError("LayoutElement is null in TooltipPopup");
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
            gameObject.SetActive(false);
        }

        public void Show(TooltipsBase.TooltipRecord tooltip)
        {
            gameObject.SetActive(true);
            title.SetText(tooltip.Title);
            description.text = tooltip.Description;
            Resize();
        }
    }
}