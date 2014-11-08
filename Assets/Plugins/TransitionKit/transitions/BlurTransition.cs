using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;


public class BlurTransition : TransitionKitDelegate
{
	public float duration = 0.5f;
	public int nextScene = -1;
	public float blurMin = 0.00001f;
	public float blurMax = 0.02f;


	#region TransitionKitDelegate implementation

	public Shader shaderForTransition()
	{
		return Shader.Find( "prime[31]/Transitions/Blur" );
	}


	public Mesh meshForDisplay()
	{
		return null;
	}


	public IEnumerator onScreenObscured( TransitionKit transitionKit )
	{
		transitionKit.transitionKitCamera.clearFlags = CameraClearFlags.Nothing;

		if( nextScene >= 0 )
			Application.LoadLevelAsync( nextScene );

		var elapsed = 0f;
		while( elapsed < duration )
		{
			elapsed += Time.deltaTime;
			var step = Mathf.Pow( elapsed / duration, 2f );
			var blurAmount = Mathf.Lerp( blurMin, blurMax, step );

			transitionKit.material.SetFloat( "_BlurAmount", blurAmount );

			yield return null;
		}

		// we dont transition back to the new scene unless it is loaded
		if( nextScene >= 0 )
			yield return transitionKit.StartCoroutine( transitionKit.waitForLevelToLoad( nextScene ) );

		transitionKit.cleanup();
	}


	public void onLevelWasLoaded( TransitionKit transitionKit, int level )
	{}

	#endregion

}
