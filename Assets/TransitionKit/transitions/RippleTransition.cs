using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;


namespace Prime31.TransitionKit
{
	public class RippleTransition : TransitionKitDelegate
	{
		public float duration = 0.5f;
		public int nextScene = -1;
		public float speed = 50.0f;
		public float amplitude = 100.0f;


		#region TransitionKitDelegate implementation

		public Shader shaderForTransition()
		{
			return Shader.Find( "prime[31]/Transitions/Ripple" );
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
			transitionKit.material.SetFloat( "_Speed", speed );
			transitionKit.material.SetFloat( "_Amplitude", amplitude );

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