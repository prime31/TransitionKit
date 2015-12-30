using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;


/// <summary>
/// the maskTexture will show the background image (screen grab) where it is transparent and the backgroundColor where it is not.
/// it zooms to a point in the center of the screen then fades back in after the new scene loads.
/// </summary>
using UnityEngine.SceneManagement;


namespace Prime31.TransitionKit
{
	public class ImageMaskTransition : TransitionKitDelegate
	{
		public Texture2D maskTexture;
		public Color backgroundColor = Color.black;
		public float duration = 0.9f;
		public int nextScene = -1;


		#region TransitionKitDelegate

		public Shader shaderForTransition()
		{
			return Shader.Find( "prime[31]/Transitions/Mask" );
		}


		public Mesh meshForDisplay()
		{
			return null;
		}


		public Texture2D textureForDisplay()
		{
			return null;
		}


		public IEnumerator onScreenObscured( TransitionKit transitionKit )
		{
			transitionKit.transitionKitCamera.clearFlags = CameraClearFlags.Nothing;
			transitionKit.material.color = backgroundColor;
			transitionKit.material.SetTexture( "_MaskTex", maskTexture );

			if( nextScene >= 0 )
				SceneManager.LoadSceneAsync( nextScene );

			// this does the zoom/rotation
			yield return transitionKit.StartCoroutine( transitionKit.tickProgressPropertyInMaterial( duration ) );

			// we dont transition back to the new scene unless it is loaded
			if( nextScene >= 0 )
				yield return transitionKit.StartCoroutine( transitionKit.waitForLevelToLoad( nextScene ) );

			// now that the new scene is loaded we zoom the mask back out
			transitionKit.makeTextureTransparent();

			yield return transitionKit.StartCoroutine( transitionKit.tickProgressPropertyInMaterial( duration, true ) );
		}

		#endregion

	}
}