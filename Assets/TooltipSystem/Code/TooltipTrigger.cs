using UnityEngine;
using UnityEngine.EventSystems;

namespace Kovnir.TooltipSystem
{
    public sealed class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TooltipKeys key;
        public void OnPointerEnter(PointerEventData eventData) => TooltipSystem.Show(key);
        public void OnPointerExit(PointerEventData eventData) => TooltipSystem.TryHide(key);
    }
}
