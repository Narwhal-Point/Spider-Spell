using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RendererFeatures
{
    public class ScreenSpaceOutlines : ScriptableRendererFeature {

        [System.Serializable]
        private class ScreenSpaceOutlineSettings {

            [Header("General Outline Settings")]
            public Color outlineColor = Color.black;
            [Range(0.0f, 20.0f)]
            public float outlineScale = 1.0f;
        
            [Header("Depth Settings")]
            [Range(0.0f, 100.0f)]
            public float depthThreshold = 1.5f;
            [Range(0.0f, 500.0f)]
            public float robertsCrossMultiplier = 100.0f;

            [Header("Normal Settings")]
            [Range(0.0f, 1.0f)]
            public float normalThreshold = 0.4f;

            [Header("Depth Normal Relation Settings")]
            [Range(0.0f, 2.0f)]
            public float steepAngleThreshold = 0.2f;
            [Range(0.0f, 500.0f)]
            public float steepAngleMultiplier = 25.0f;
        
            [Header("General Scene View Space Normal Texture Settings")]
            public RenderTextureFormat colorFormat;
            public int depthBufferBits;
            public FilterMode filterMode;
            public Color backgroundColor = Color.clear;

            [Header("View Space Normal Texture Object Draw Settings")]
            public PerObjectData perObjectData;
            public bool enableDynamicBatching;
            public bool enableInstancing;

        }

        private class ScreenSpaceOutlinePass : ScriptableRenderPass {
        
            private readonly Material _screenSpaceOutlineMaterial;
            private readonly ScreenSpaceOutlineSettings _settings;

            private readonly FilteringSettings _filteringSettings;

            private readonly List<ShaderTagId> _shaderTagIdList;
            private readonly Material _normalsMaterial;

            private RTHandle _normals;
            private RendererList _normalsRenderersList;

            RTHandle _temporaryBuffer;

            public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent, LayerMask layerMask,
                ScreenSpaceOutlineSettings settings) {
                this._settings = settings;
                this.renderPassEvent = renderPassEvent;

                _screenSpaceOutlineMaterial = new Material(Shader.Find("Hidden/Outlines"));
                _screenSpaceOutlineMaterial.SetColor(OutlineColor, settings.outlineColor);
                _screenSpaceOutlineMaterial.SetFloat(OutlineScale, settings.outlineScale);

                _screenSpaceOutlineMaterial.SetFloat(DepthThreshold, settings.depthThreshold);
                _screenSpaceOutlineMaterial.SetFloat(RobertsCrossMultiplier, settings.robertsCrossMultiplier);

                _screenSpaceOutlineMaterial.SetFloat(NormalThreshold, settings.normalThreshold);

                _screenSpaceOutlineMaterial.SetFloat(SteepAngleThreshold, settings.steepAngleThreshold);
                _screenSpaceOutlineMaterial.SetFloat(SteepAngleMultiplier, settings.steepAngleMultiplier);
            
                _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

                _shaderTagIdList = new List<ShaderTagId> {
                    new ShaderTagId("UniversalForward"),
                    new ShaderTagId("UniversalForwardOnly"),
                    new ShaderTagId("LightweightForward"),
                    new ShaderTagId("SRPDefaultUnlit")
                };

                _normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormals"));
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
                // Normals
                RenderTextureDescriptor textureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                textureDescriptor.colorFormat = _settings.colorFormat;
                textureDescriptor.depthBufferBits = _settings.depthBufferBits;
                RenderingUtils.ReAllocateIfNeeded(ref _normals, textureDescriptor, _settings.filterMode);
            
                // Color Buffer
                textureDescriptor.depthBufferBits = 0;
                RenderingUtils.ReAllocateIfNeeded(ref _temporaryBuffer, textureDescriptor, FilterMode.Bilinear);

                ConfigureTarget(_normals, renderingData.cameraData.renderer.cameraDepthTargetHandle);
                ConfigureClear(ClearFlag.Color, _settings.backgroundColor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                if (!_screenSpaceOutlineMaterial || !_normalsMaterial || 
                    renderingData.cameraData.renderer.cameraColorTargetHandle.rt == null || _temporaryBuffer.rt == null)
                    return;

                CommandBuffer cmd = CommandBufferPool.Get();
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                // Normals
                DrawingSettings drawSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawSettings.perObjectData = _settings.perObjectData;
                drawSettings.enableDynamicBatching = _settings.enableDynamicBatching;
                drawSettings.enableInstancing = _settings.enableInstancing;
                drawSettings.overrideMaterial = _normalsMaterial;
            
                RendererListParams normalsRenderersParams = new RendererListParams(renderingData.cullResults, drawSettings, _filteringSettings);
                _normalsRenderersList = context.CreateRendererList(ref normalsRenderersParams);
                cmd.DrawRendererList(_normalsRenderersList);
            
                // Pass in RT for Outlines shader
                cmd.SetGlobalTexture(Shader.PropertyToID("_SceneViewSpaceNormals"), _normals.rt);
            
                using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines"))) {

                    Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, _temporaryBuffer, _screenSpaceOutlineMaterial, 0);
                    Blitter.BlitCameraTexture(cmd, _temporaryBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public void Release(){
                CoreUtils.Destroy(_screenSpaceOutlineMaterial);
                CoreUtils.Destroy(_normalsMaterial);
                _normals?.Release();
                _temporaryBuffer?.Release();
            }

        }

        [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
        [SerializeField] private LayerMask outlinesLayerMask;
    
        [SerializeField] private ScreenSpaceOutlineSettings outlineSettings = new ScreenSpaceOutlineSettings();

        private ScreenSpaceOutlinePass screenSpaceOutlinePass;
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineScale = Shader.PropertyToID("_OutlineScale");
        private static readonly int DepthThreshold = Shader.PropertyToID("_DepthThreshold");
        private static readonly int RobertsCrossMultiplier = Shader.PropertyToID("_RobertsCrossMultiplier");
        private static readonly int NormalThreshold = Shader.PropertyToID("_NormalThreshold");
        private static readonly int SteepAngleThreshold = Shader.PropertyToID("_SteepAngleThreshold");
        private static readonly int SteepAngleMultiplier = Shader.PropertyToID("_SteepAngleMultiplier");

        public override void Create() {
            if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
                renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

            screenSpaceOutlinePass = new ScreenSpaceOutlinePass(renderPassEvent, outlinesLayerMask, outlineSettings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            renderer.EnqueuePass(screenSpaceOutlinePass);
        }

        protected override void Dispose(bool disposing){
            if (disposing)
            {
                screenSpaceOutlinePass?.Release();
            }
        }

    }
}
