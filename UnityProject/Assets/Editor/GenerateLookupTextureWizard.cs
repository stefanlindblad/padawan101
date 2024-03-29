// GenerateLookupTexturesWizard.cs
// Place this script in Editor folder

// Original code by Jon Moore, published in Gamasutra
// http://www.gamasutra.com/view/news/39446/InDepth_Skin_Shading_In_Unity3D.php#.UNc0Pm92zcM
// Unlike the original it generates a single texture with diffuse scattering in RGB channels
// and Beckmann in alpha channel.
using UnityEditor;
using UnityEngine;
 
using System.IO;
 
class GenerateLookupTextureWizard : ScriptableWizard {
	public int width = 256;
	public int height = 256;
 
    [MenuItem ("GameObject/Pre-Integrated Skin Shader/Generate Lookup Texture")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard<GenerateLookupTextureWizard>("PreIntegrate Lookup Textures", "Create");
    }
 
	float PHBeckmann(float ndoth, float m) {
		float alpha = Mathf.Acos(ndoth);
		float ta = Mathf.Tan(alpha);
		float val = 1f/(m*m*Mathf.Pow(ndoth,4f)) * Mathf.Exp(-(ta * ta) / (m * m));
		return val;
	}
 
	Vector3 IntegrateDiffuseScatteringOnRing(float cosTheta, float skinRadius) {
		// Angle from lighting direction
		float theta = Mathf.Acos(cosTheta);
		Vector3 totalWeights = Vector3.zero;
		Vector3 totalLight = Vector3.zero;
 
		float a = -(Mathf.PI/2.0f);
 
		const float inc = 0.05f;
 
		while (a <= (Mathf.PI/2.0f)) {
			float sampleAngle = theta + a;
			float diffuse = Mathf.Clamp01( Mathf.Cos(sampleAngle) );
 
			// Distance
			float sampleDist = Mathf.Abs( 2.0f * skinRadius * Mathf.Sin(a * 0.5f) );
 
			// Profile Weight
			Vector3 weights = Scatter(sampleDist);
 
			totalWeights += weights;
			totalLight += diffuse * weights;
			a+=inc;
		}
 
		Vector3 result = new Vector3(totalLight.x / totalWeights.x, totalLight.y / totalWeights.y, totalLight.z / totalWeights.z);
		return result;
	}
	
	float Gaussian (float v, float r) {
		return 1.0f / Mathf.Sqrt(2.0f * Mathf.PI * v) * Mathf.Exp(-(r * r) / (2 * v));
	}
 
	Vector3 Scatter (float r) {
		// Values from GPU Gems 3 "Advanced Skin Rendering"
		// Originally taken from real life samples
		return Gaussian(0.0064f * 1.414f, r) * new Vector3(0.233f, 0.455f, 0.649f)
			+ Gaussian(0.0484f * 1.414f, r) * new Vector3(0.100f, 0.336f, 0.344f)
			+ Gaussian(0.1870f * 1.414f, r) * new Vector3(0.118f, 0.198f, 0.000f)
			+ Gaussian(0.5670f * 1.414f, r) * new Vector3(0.113f, 0.007f, 0.007f)
			+ Gaussian(1.9900f * 1.414f, r) * new Vector3(0.358f, 0.004f, 0.00001f)
			+ Gaussian(7.4100f * 1.414f, r) * new Vector3(0.078f, 0.00001f, 0.00001f);
	}
 
    void OnWizardCreate () {
		string path = Application.dataPath + "/PreIntegratedSkinShader/skin_lookup_diffspec.png";
		string pathRel = "Assets/PreIntegratedSkinShader/skin_lookup_diffspec.png";
		
		Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		try {
			for (int j = 0; j < height; ++j) {
				for (int i = 0; i < width; ++i) {
					// Diffuse Scattering in RGB
					// Lookup by:
					// x: NDotL
					// y: 1 / r
					float y = 2.0f * 1f / ((j + 1) / (float) height);
					Vector3 diff = IntegrateDiffuseScatteringOnRing(Mathf.Lerp(-1f, 1f, i/(float) width), y);
					
					// Beckmann Texture for specular in alpha channel
					float spec = 0.5f * Mathf.Pow(PHBeckmann(i/(float) width, 1.0f-j/(float)height), 0.1f);
					
					texture.SetPixel(i, j, new Color(diff.x,diff.y,diff.z,spec));
				}
				
				float progress = (float)j / (float)height;
				bool canceled = EditorUtility.DisplayCancelableProgressBar("generating lookup texture", "", progress);
				if (canceled)
					return;					
			}
			texture.Apply();

			byte[] bytes = texture.EncodeToPNG();
			File.WriteAllBytes(path, bytes);
		} finally {
			DestroyImmediate(texture);
			EditorUtility.ClearProgressBar();
		}
		
		// not set import settings for the texture
		// It needs to be clamped and it shouldn't be compressed.
		TextureImporter textureImporter = TextureImporter.GetAtPath(pathRel) as TextureImporter;
		textureImporter.textureFormat = TextureImporterFormat.ARGB32;
		textureImporter.textureType = TextureImporterType.Advanced;
		textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        textureImporter.mipmapEnabled = false;
        textureImporter.linearTexture = true;
		textureImporter.filterMode = FilterMode.Bilinear;
		textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.maxTextureSize = Mathf.Max(width, height);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
		
		// no matter what i try the texture importer doesn't refresh in Unity 4.0 :(
    }
 
    void OnWizardUpdate () {
        helpString = "Press Create to create lookup textures. You have to set wrap mode to clamp manually for correct results.";
    }
}