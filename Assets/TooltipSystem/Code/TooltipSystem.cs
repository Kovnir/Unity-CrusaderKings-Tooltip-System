using UnityEngine;

namespace Kovnir.TooltipSystem
{
    public sealed class TooltipSystem : MonoBehaviour
    {
        [SerializeField]
        private TooltipPopup tooltipPopup;
        
        private static TooltipSystem instance;
        private TooltipsBase tooltipsBase;
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            tooltipsBase = TooltipsBase.Load();
            tooltipPopup.Hide();
        }
        
        public static void Show(TooltipKeys key) => instance.ShowInstance(key);
        
        public static void TryHide(TooltipKeys key) => instance.TryHideInstance(key);


        private void ShowInstance(TooltipKeys key)
        {
            if (tooltipsBase.TryGetTooltip(key, out TooltipsBase.TooltipRecord tooltip))
            {
                tooltipPopup.Show(tooltip);
                tooltipPopup.transform.position = Input.mousePosition;
            }
            else
            {
                Debug.LogError($"Tooltip with key {key} not found");
            }
        }
        
        private void TryHideInstance(TooltipKeys key)
        {
            tooltipPopup.Hide();
        }
    }
}