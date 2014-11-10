using UnityEngine;
using System.Collections;


namespace Prime31.TransitionKit
{
	/// <summary>
	/// this is the interface an object must conform to for use with TransitionKit. Delegates can return custom shaders, meshes
	/// and textures if needed.
	/// </summary>
	public interface TransitionKitDelegate
	{
		/// <summary>
		/// if the transition needs a custom shader return it here otherwise return null which will use the TextureWithAlpha shader
		/// </summary>
		/// <returns>The for transition.</returns>
		Shader shaderForTransition();


		/// <summary>
		/// if the transition needs a custom Mesh return it here otherwise return null which will use a full screen quad.
		/// The Mesh should be centered in screen-space. See the TransitionKit.generateQuadMesh for an example.
		/// </summary>
		/// <returns>the Mesh</returns>
		Mesh meshForDisplay();


		/// <summary>
		/// if the transition needs a custom Texture2D return it here otherwise return null which will use a screenshot
		/// </summary>
		/// <returns>the Texture2D.</returns>
		Texture2D textureForDisplay();


		/// <summary>
		/// called when the screen is fully obscured. You can now load a new scene or modify the current one and it will be fully obscured from view.
		/// Note that when control returns from this method TransitionKit will kill itself.
		/// </summary>
		IEnumerator onScreenObscured( TransitionKit transitionKit );
	}
}