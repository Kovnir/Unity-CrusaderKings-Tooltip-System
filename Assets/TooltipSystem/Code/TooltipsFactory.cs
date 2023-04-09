using System.Collections.Generic;
using System.Linq;
using Kovnir.TooltipSystem;
using UnityEngine;

namespace TooltipSystem.Code
{
    public sealed class TooltipsFactory
    {
        private readonly TooltipPopup prototype;
        private readonly Transform parent;
        private readonly Stack<TooltipPopup> pool = new();

        public TooltipsFactory(TooltipPopup tooltipPopup, Transform parentTransform)
        {
            prototype = tooltipPopup;
            parent = parentTransform;
        }

        public TooltipPopup Create()
        {
            TooltipPopup newPopup;
            if (pool.Any())
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
            newPopup.transform.position = position;
            return newPopup;
        }
        
        public void ReturnToPool(TooltipPopup tooltipPopup)
        {
            pool.Push(tooltipPopup);
        }
    }
}