using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;


namespace Prime31.TransitionKit
{
	public class FishEyeTransition : TransitionKitDelegate
	{
		public float duration = 0.5f;
		public int nextScene = -1;
		public float size = 0.2f;
		public float zoom = 100.0f;
		public float colorSeparation = 0.2f;


		#region TransitionKitDelegate implementation

		public Shader shaderForTransition()
		{
			return Shader.Find( "prime[31]/Transitions/Fish Eye" );
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

			// set some material properties
			transitionKit.material.SetFloat( "_Size", size );
			transitionKit.material.SetFloat( "_Zoom", zoom );
			transitionKit.material.SetFloat( "_ColorSeparation", zoom );

			// we dont transition back to the new scene unless it is loaded
			if( nextScene >= 0 )
			{
				SceneManager.LoadSceneAsync( nextScene );
				yield return transitionKit.StartCoroutine( transitionKit.waitForLevelToLoad( nextScene ) );
			}

			yield return transitionKit.StartCoroutine( transitionKit.tickProgressPropertyInMaterial( duration ) );
		}

		#endregion

	}
}