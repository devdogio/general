using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.General
{
    public static class TransformExtensionMethods
    {
        /// <summary>
        /// Reset the transform rotation and scale of this transform
        /// </summary>
        /// <param name="transform"></param>
        public static void ResetTRS(this Transform transform)
        {
            Assert.IsNotNull(transform, "Transform given is null");

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Reset the transform, rotation, scale and rect transform anchor of this transform.
        /// </summary>
        /// <param name="transform"></param>
        public static void ResetTRSRect(this Transform transform)
        {
            transform.ResetTRS();

            var rectTransform = transform.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}
