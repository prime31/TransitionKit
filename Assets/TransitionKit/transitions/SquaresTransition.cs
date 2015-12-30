using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;


namespace Prime31.TransitionKit
{
	public class SquaresTransition : TransitionKitDelegate
	{
		public Color squareColor = Color.black;
		public float duration = 1.0f;
		public float fadedDelay = 0f;
		public int nextScene = -1;
		public Vector2 squareSize = new Vector2( 13f, 9f );
		public float smoothness = 0.5f;


		#region TransitionKitDelegate

		public Shader shaderForTransition()
		{
			return Shader.Find( "prime[31]/Transitions/Squares" );
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
			transitionKit.material.color = squareColor;
			transitionKit.material.SetFloat( "_Smoothness", smoothness );
			transitionKit.material.SetVector( "_Size", squareSize );

			if( nextScene >= 0 )
				SceneManager.LoadSceneAsync( nextScene );

			yield return transitionKit.StartCoroutine( transitionKit.tickProgressPropertyInMaterial( duration ) );

			transitionKit.makeTextureTransparent();

			if( fadedDelay > 0 )
				yield return new WaitForSeconds( fadedDelay );

			// we dont transition back to the new scene unless it is loaded
			if( nextScene >= 0 )
				yield return transitionKit.StartCoroutine( transitionKit.waitForLevelToLoad( nextScene ) );

			yield return transitionKit.StartCoroutine( transitionKit.tickProgressPropertyInMaterial( duration, true ) );
		}

		#endregion

	}
}