using System.Collections.Generic;
using UnityEngine;

namespace BZCommon.Helpers
{
    public class ColorizationHelper
    {
        public void AddColorCustomizerToGameObject(GameObject gameObject)
        {
            ColorCustomizer colorCustomizer = gameObject.AddComponent<ColorCustomizer>();

            colorCustomizer.isBase = false;

            List<Renderer> Renderers = new List<Renderer>();

            gameObject.GetComponentsInChildren(true, Renderers);

            int materials = 0;

            for (int i = 0; i < Renderers.Count; i++)
            {
                materials += Renderers[i].materials.Length;
            }

            colorCustomizer.colorDatas = new ColorCustomizer.ColorData[materials];

            int m = 0;

            for (int j = 0; j < Renderers.Count; j++)
            {
                for (int k = 0; k < Renderers[j].sharedMaterials.Length; k++)
                {
                    colorCustomizer.colorDatas[m] = new ColorCustomizer.ColorData
                    {
                        renderer = Renderers[j],
                        materialIndex = k
                    };

                    m++;
                }
            }
        }
    }
}
