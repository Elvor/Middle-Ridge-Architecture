
using UnityEngine;

namespace MiddleRidge.DI
{
    public static class ObjectUtils
    {
        public static Transform Find(Transform source, string childName)
        {
            var res = source.Find(childName);
            if (res != null) return res;
            for (var i = 0; i < source.childCount; i++)
            {
                var childRes = Find(source.GetChild(i), childName);
                if (childRes != null)
                {
                    return childRes;
                }
            }

            return res;
        }
    }
}