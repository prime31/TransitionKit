using UnityEngine;
using System.Collections;


namespace Prime31.TransitionKit
{
	public interface TransitionKitDelegate
	{
		/// <summary>
		/// if the transition needs a custom shader return it here otherwise return null which will use the Unlit/Texture shader
		/// </summary>
		/// <returns>The for transition.</returns>
		Shader shaderForTransition();


		/// <summary>
		/// if the transition needs a custom Mesh return it here otherwise return null which will use a full screen quad
		/// </summary>
		/// <returns>The for.</returns>
		Mesh meshForDisplay();


		/// <summary>
		/// called when the screen is fully obscured. You can now load a new scene or modify the current one and it will be fully obscured from view
		/// </summary>
		IEnumerator onScreenObscured( TransitionKit transitionKit );


		/// <summary>
		/// called when Unity's MonoBehaviourOnLevelWasLoaded is called. When doing a level load this is where you would want to start your transition
		/// </summary>
		/// <param name="transitionKit">Transition kit.</param>
		void onLevelWasLoaded( TransitionKit transitionKit, int level );
	}
}