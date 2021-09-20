/*
using UnityEngine;

namespace Fahrenheit
{
    public class NightVisionManager : MonoBehaviour
    {
        public Shader shader;

        public Color luminence;

        public float noiseFactor = 0.005f;

        private Material mat;

        private void Start()
        {
            shader = Shader.Find("Image Effects/Night Vision");
            mat = new Material(shader);
            mat.SetVector(ShaderPropertyID.lum, new Vector4(luminence.g, luminence.g, luminence.g, luminence.g));
            mat.SetFloat(ShaderPropertyID.noiseFactor, noiseFactor);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {

            if (Main.NightVisionEnabled)
            {
                mat.SetFloat(ShaderPropertyID.time, Mathf.Sin(Time.time * Time.deltaTime));
                Graphics.Blit(source, destination, mat);
            }

            
        }
    }
}
*/