using UnityEngine;

namespace Kovnir.TooltipSystem
{
    public sealed class TooltipSystem : MonoBehaviour
    {
        [SerializeField]
        private TooltipPopup tooltipPopup;
        [SerializeField] private float offset;

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

                Vector3 position = Input.mousePosition;
                RectTransform rectTransform = tooltipPopup.GetComponent<RectTransform>();
                
                rectTransform.pivot = new Vector2(position.x / Screen.width, position.y / Screen.height);
//                rectTransform.pivot = new Vector2(-offset, 1 + offset);
                tooltipPopup.transform.position = position;
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