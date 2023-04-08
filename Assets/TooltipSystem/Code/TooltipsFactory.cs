using System.Collections.Generic;
using Kovnir.TooltipSystem;
using UnityEngine;

namespace TooltipSystem.Code
{
    public class TooltipsFactory
    {
        private readonly TooltipPopup prototype;
        private readonly Transform parent;
        private readonly Stack<TooltipPopup> pool = new Stack<TooltipPopup>();

        public TooltipsFactory(TooltipPopup tooltipPopup, Transform parentTransform)
        {
            prototype = tooltipPopup;
            parent = parentTransform;
        }

        public TooltipPopup Create()
        {
            TooltipPopup newPopup;
            if (pool.Count > 0)
            {
                newPopup = pool.Pop();
            }
            else
            {
                newPopup = Object.Instantiate(prototype, parent);
            }
            

            Vector3 position = Input.mousePosition;
            RectTransform rectTransform = newPopup.GetComponent<RectTransform>();

            rectTransform.pivot = new Vector2(position.x / Screen.width, position.y / Screen.height);
//                rectTransform.pivot = new Vector2(-offset, 1 + offset);
            newPopup.transform.position = position;
            return newPopup;
        }
        
        public void ReturnToPool(TooltipPopup tooltipPopup)
        {
            pool.Push(tooltipPopup);
        }
    }
}