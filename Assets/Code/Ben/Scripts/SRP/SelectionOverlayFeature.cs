using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Applies selection overlay material to any object within "Selectable" layer.
public class SelectionOverlayFeature : ScriptableRendererFeature
{
    public Material mat;

    class SelectionOverlayPass: ScriptableRenderPass
    {
        Material mat;
        FilteringSettings filtering;
        ShaderTagId tag = new ShaderTagId("UniversalForward");

        public SelectionOverlayPass(Material mat)
        {
            this.mat = mat;
            filtering = new FilteringSettings(RenderQueueRange.all, LayerMask.GetMask("Selectable"));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var drawingSettings = CreateDrawingSettings(tag, ref renderingData, SortingCriteria.CommonTransparent);
            drawingSettings.overrideMaterial = mat;
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filtering);
        }
    }

    SelectionOverlayPass selectionOverlayPass;

    public override void Create()
    {
        selectionOverlayPass = new SelectionOverlayPass(mat);
        selectionOverlayPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(selectionOverlayPass);
    }

}
