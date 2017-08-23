using UnityEngine;
using System.Collections.Generic;

namespace cakeslice
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class OutlineEffect : MonoBehaviour
    {
        private static OutlineEffect m_instance;
        public static OutlineEffect Instance
        {
            get
            {
                if(Equals(m_instance, null))
                {
                    return m_instance = FindObjectOfType(typeof(OutlineEffect)) as OutlineEffect;
                }

                return m_instance;
            }
        }
        private OutlineEffect() { }

		private readonly LinkedSet<Outline> outlines = new LinkedSet<Outline>();
        private const int outlineLayer = 6;

        [Range(1.0f, 6.0f)]
        public float lineThickness = 1.25f;
        public float lineIntensity = .5f;

        public Color lineColor = Color.yellow;

        public bool additiveRendering = false;

        public Camera sourceCamera;
        public Camera outlineCamera;

        Material outline1Material;
        Material outlineEraseMaterial;
        Shader outlineShader;
        Shader outlineBufferShader;
        Material outlineShaderMaterial;
        RenderTexture renderTexture;
        RenderTexture extraRenderTexture;
        
        Material[] outline1MaterialBuffer = new Material[20];
        Material[] eraseMaterialBuffer = new Material[20];

        Material[] GetMaterialBufferFromID(int ID)
        {
            if(ID == 0)
                return outline1MaterialBuffer;
            else
                return null;
        }

        Material CreateMaterial(Color emissionColor)
        {
            Material m = new Material(outlineBufferShader);
            m.SetColor("_Color", emissionColor);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
            return m;
        }

        private void Awake()
        {
            m_instance = this;
        }

        void Start()
        {
            CreateMaterialsIfNeeded();
            UpdateMaterialsPublicProperties();

            if(sourceCamera == null)
            {
                sourceCamera = GetComponent<Camera>();

                if(sourceCamera == null)
                    sourceCamera = Camera.main;
            }

            if(outlineCamera == null)
            {
                GameObject cameraGameObject = new GameObject("Outline Camera");
                cameraGameObject.transform.parent = sourceCamera.transform;
                outlineCamera = cameraGameObject.AddComponent<Camera>();
            }
            renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
            extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
            UpdateOutlineCameraFromSource();
        }

        private void OnEnable()
        {
            Outline[] o = FindObjectsOfType<Outline>();

            foreach(Outline oL in o)
            {
                oL.enabled = false;
                oL.enabled = true;
            }
        }

        void OnDestroy()
        {
            if(renderTexture != null)
                renderTexture.Release();
            if(extraRenderTexture != null)
                extraRenderTexture.Release();
            DestroyMaterials();
        }

        void OnPreRender()
        {
            if(renderTexture.width != sourceCamera.pixelWidth || renderTexture.height != sourceCamera.pixelHeight)
            {
                renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
                extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
                outlineCamera.targetTexture = renderTexture;
            }
            UpdateMaterialsPublicProperties();
            UpdateOutlineCameraFromSource();

            if(outlines != null)
            {
                foreach (Outline outline in outlines)
                {
                    LayerMask l = sourceCamera.cullingMask;

                    if(outline != null && l == (l | (1 << outline.originalLayer)))
                    {
                        outline.originalMaterials = outline.Renderer.sharedMaterials;

                        outline.originalLayer = outline.gameObject.layer;

						if(outline.eraseRenderer)
							outline.Renderer.sharedMaterials = eraseMaterialBuffer;
                        else
							outline.Renderer.sharedMaterials = GetMaterialBufferFromID(outline.color);

						for(int m = 0; m < outline.originalMaterials.Length; m++)
                        {
							if(outline.Renderer is MeshRenderer)
								outline.Renderer.sharedMaterials[m].mainTexture = outline.originalMaterials[m].mainTexture;
                        }

                        outline.gameObject.layer = outlineLayer;
                    }
                }
            }

            outlineCamera.Render();

            if(outlines != null)
            {
				foreach (Outline outline in outlines)
                {
                    LayerMask l = sourceCamera.cullingMask;
                    if(outline != null && l == (l | (1 << outline.originalLayer)))
                    {
                        for(int m = 0; m < outline.Renderer.sharedMaterials.Length; m++)
                        {
                            if(outline.Renderer is MeshRenderer)
                            {
                                outline.Renderer.sharedMaterials[m].mainTexture = null;
                            }
                        }
                        outline.Renderer.sharedMaterials = outline.originalMaterials;
                        outline.gameObject.layer = outline.originalLayer;
                    }
                }
            }
        }
        
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            outlineShaderMaterial.SetTexture("_OutlineSource", renderTexture);
            Graphics.Blit(source, destination, outlineShaderMaterial, 1);
        }
        
        private void CreateMaterialsIfNeeded()
        {
            if(outlineShader == null)
                outlineShader = Resources.Load<Shader>("OutlineShader");
            if(outlineBufferShader == null)
            {
                outlineBufferShader = Resources.Load<Shader>("OutlineBufferCullOffShader");
            }
            if(outlineShaderMaterial == null)
            {
                outlineShaderMaterial = new Material(outlineShader);
                outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
                UpdateMaterialsPublicProperties();
            }
            if(outlineEraseMaterial == null)
                outlineEraseMaterial = CreateMaterial(new Color(0, 0, 0, 0));
            if(outline1Material == null)
                outline1Material = CreateMaterial(new Color(1, 0, 0, 0));

            for(int i = 0; i < outline1MaterialBuffer.Length; i++)
            {
                outline1MaterialBuffer[i] = outline1Material;
            }
            for(int i = 0; i < eraseMaterialBuffer.Length; i++)
            {
                eraseMaterialBuffer[i] = outlineEraseMaterial;
            }
        }

        private void DestroyMaterials()
        {
            DestroyImmediate(outlineShaderMaterial);
            DestroyImmediate(outlineEraseMaterial);
            DestroyImmediate(outline1Material);
            outlineShader = null;
            outlineBufferShader = null;
            outlineShaderMaterial = null;
            outlineEraseMaterial = null;
            outline1Material = null;
        }

        public void UpdateMaterialsPublicProperties()
        {
            if(outlineShaderMaterial)
            {
                outlineShaderMaterial.SetFloat("_LineThicknessX", (lineThickness / 1000.0f) * (1.0f / Screen.width) * 1000.0f);
                outlineShaderMaterial.SetFloat("_LineThicknessY", (lineThickness / 1000.0f) * (1.0f / Screen.height) * 1000.0f);
                outlineShaderMaterial.SetFloat("_LineIntensity", lineIntensity);

                outlineShaderMaterial.SetColor("_LineColor1", lineColor * lineColor);
            }
        }

        //아웃라인 카메라 생성
        void UpdateOutlineCameraFromSource()
        {
            outlineCamera.CopyFrom(sourceCamera);
            outlineCamera.renderingPath = RenderingPath.Forward;
            outlineCamera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            outlineCamera.clearFlags = CameraClearFlags.SolidColor;
            outlineCamera.rect = new Rect(0, 0, 1, 1);
            outlineCamera.enabled = true;
            outlineCamera.cullingMask = 1 << outlineLayer; // UI layer
            outlineCamera.targetTexture = renderTexture;
        }

        public void AddOutline(Outline outline)
        {
            if(!outlines.Contains(outline))
			    outlines.Add(outline);
        }

        public void RemoveOutline(Outline outline)
        {
            if(outlines.Contains(outline))
                outlines.Remove(outline);
        }
    }
}