using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;


namespace Prime31.TransitionKit
{
	/// <summary>
	/// the Doorway shader can have _Progress run from 0 to -1 or 0 to 1. The runEffectInReverse controls that.
	/// </summary>
	public class DoorwayTransition : TransitionKitDelegate
	{
		public float duration = 0.5f;
		public int nextScene = -1;
		public float perspective = 1.5f;
		public float depth = 3.0f;
		public bool runEffectInReverse = false;


		#region TransitionKitDelegate implementation

		public Shader shaderForTransition()
		{
			return Shader.Find( "prime[31]/Transitions/Doorway" );
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
			transitionKit.material.SetFloat( "_Perspective", perspective );
			transitionKit.material.SetFloat( "_Depth", depth );
			transitionKit.material.SetInt( "_Direction", runEffectInReverse ? 1 : 0 );

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
