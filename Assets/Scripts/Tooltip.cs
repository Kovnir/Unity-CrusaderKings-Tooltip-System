using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kovnir.TooltipSystem
{
    [RequireComponent(typeof(LayoutElement))]
    public sealed class Tooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;

        [SerializeField] private uint characterWrapLimit = 50;

        private LayoutElement layoutElement;

        private void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
        }

        private void Update()
        {
            if (title == null || description == null)
            {
                return;
            }

            if (layoutElement == null)
            {
                return;
            }

            // title.text = "Title";
            // description.text = "Description";

            int titleLength = title.text.Length;
            int descriptionLength = description.text.Length;
            int totalLength = Mathf.Max(titleLength, descriptionLength);
            layoutElement.enabled = totalLength > characterWrapLimit;
        }
    }
}