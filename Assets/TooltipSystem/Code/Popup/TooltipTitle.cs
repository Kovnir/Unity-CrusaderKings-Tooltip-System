using TMPro;
using UnityEngine;

namespace Kovnir.TooltipSystem
{
    public sealed class TooltipTitle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        public int Length => title.text.Length;

        public void SetText(string titleText)
        {
            gameObject.SetActive(!string.IsNullOrEmpty(titleText));
            title.text = titleText;
        }
    }
}