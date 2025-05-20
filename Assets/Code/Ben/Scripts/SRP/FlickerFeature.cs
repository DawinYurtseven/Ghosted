using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FlickerFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class OverlaySettings
    {
        public Shader shader;
        public Color color = new Color(0.2f, 0.2f, 0.2f);
        [Range(0,1)] public float opacity = 1.0f;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public OverlaySettings overlaySettings = new OverlaySettings();
    Material overlayMaterial;
    OverlayPass overlayPass;

    class OverlayPass : ScriptableRenderPass
    {
        Material overlayMaterial;
        Color color;
        float opacity;

        public OverlayPass(Material overlayMaterial, Color color, float opacity)
        {
            this.overlayMaterial = overlayMaterial;
            this.color = color;
            this.opacity = opacity;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            CommandBuffer cmd = CommandBufferPool.Get(name:"OverlayPass");


            var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            int tempRT = Shader.PropertyToID("_TempOverlayRT");
            cmd.GetTemporaryRT(tempRT, descriptor, FilterMode.Bilinear);

            // Copy the current camera color buffer into a temporary render texture.
            Blit(cmd, cameraColorTarget, tempRT);

            // Set the overlay color with transparency
            overlayMaterial.SetColor("_OverlayColor", new Color(color.r, color.g, color.b, opacity));

            // Make the original screen content available to the overlay shader via _MainTex.
            cmd.SetGlobalTexture("_MainTex", tempRT); // Let the shader access screen color

            // Composite the overlay on top of the original screen image.
            Blit(cmd, tempRT, cameraColorTarget, overlayMaterial); // Composite overlay

            cmd.ReleaseTemporaryRT(tempRT);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public override void Create()
    {
        if (overlaySettings.shader == null)
        {
            Debug.LogError("Missing overlay shader.");
            return;
        }

        overlayMaterial = CoreUtils.CreateEngineMaterial(overlaySettings.shader);
        overlayPass = new OverlayPass(overlayMaterial, overlaySettings.color, overlaySettings.opacity);
        overlayPass.renderPassEvent = overlaySettings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (overlayMaterial != null)
            renderer.EnqueuePass(overlayPass);
    }
}
