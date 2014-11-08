using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;


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


		public IEnumerator onScreenObscured( TransitionKit transitionKit )
		{
			transitionKit.transitionKitCamera.clearFlags = CameraClearFlags.Nothing;
			transitionKit.material.color = squareColor;
			transitionKit.material.SetFloat( "_Smoothness", smoothness );
			transitionKit.material.SetVector( "_Size", squareSize );

			if( nextScene >= 0 )
				Application.LoadLevelAsync( nextScene );

			var elapsed = 0f;
			while( elapsed < duration )
			{
				elapsed += Time.deltaTime;
				var step = Mathf.Pow( elapsed / duration, 2f );
				transitionKit.material.SetFloat( "_Progress", step );

				yield return null;
			}

			var tex = new Texture2D( 1, 1 );
			tex.SetPixel( 0, 0, Color.clear );
			tex.Apply();

			transitionKit.material.mainTexture = tex;

			if( fadedDelay > 0 )
				yield return new WaitForSeconds( fadedDelay );

			// we dont transition back to the new scene unless it is loaded
			if( nextScene >= 0 )
				yield return transitionKit.StartCoroutine( transitionKit.waitForLevelToLoad( nextScene ) );

			while( elapsed > 0f )
			{
				elapsed -= Time.deltaTime;
				var step = Mathf.Pow( elapsed / duration, 2f );
				transitionKit.material.SetFloat( "_Progress", step );

				yield return null;
			}

			transitionKit.cleanup();
		}


		public void onLevelWasLoaded( TransitionKit transitionKit, int level )
		{}

		#endregion

	}
}