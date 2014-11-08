using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;


public class SceneSwitcher : MonoBehaviour
{
	void OnGUI()
	{
		// this is a ghetto-style hack because we are using the terrible GUI class in this simple demo.
		// we just want to hide the GUI while TransitionKit is active
		if( GameObject.Find( "TransitionKit" ) )
			return;


		if( GUILayout.Button( "Fade to Scene" ) )
		{
			var fader = new FadeTransition()
			{
				nextScene = Application.loadedLevel == 0 ? 1 : 0,
				fadedDelay = 0.2f,
				fadeToColor = Color.white
			};
			TransitionKit.instance.transitionWithDelegate( fader );
		}


		if( GUILayout.Button( "Pixelate to Scene with Random Scale Effect" ) )
		{
			var enumValues = System.Enum.GetValues( typeof( PixelateTransition.PixelateFinalScaleEffect ) );
			var randomScaleEffect = (PixelateTransition.PixelateFinalScaleEffect)enumValues.GetValue( Random.Range( 0, enumValues.Length ) );

			var pixelater = new PixelateTransition()
			{
				nextScene = Application.loadedLevel == 0 ? 1 : 0,
				finalScaleEffect = randomScaleEffect,
				duration = 1.0f
			};
			TransitionKit.instance.transitionWithDelegate( pixelater );
		}


		if( GUILayout.Button( "Twirl via Component with No Scene Change" ) )
		{
			TransitionKit.instance.transitionWithDelegate( GetComponent<TwirlTransition>() );
		}


		if( GUILayout.Button( "Blur to Scene" ) )
		{
			var blur = new BlurTransition()
			{
				nextScene = Application.loadedLevel == 0 ? 1 : 0,
				duration = 2.0f,
				blurMax = 0.01f
			};
			TransitionKit.instance.transitionWithDelegate( blur );
		}


		if( GUILayout.Button( "Wacky Blur with No Scene Change" ) )
		{
			var blur = new BlurTransition()
			{
				duration = 1.0f,
				blurMax = 1f
			};
			TransitionKit.instance.transitionWithDelegate( blur );
		}


		if( GUILayout.Button( "Big Squares to Scene" ) )
		{
			var squares = new SquaresTransition()
			{
				nextScene = Application.loadedLevel == 0 ? 1 : 0,
				duration = 2.0f
			};
			TransitionKit.instance.transitionWithDelegate( squares );
		}


		if( GUILayout.Button( "Little Squares to Scene" ) )
		{
			var squares = new SquaresTransition()
			{
				nextScene = Application.loadedLevel == 0 ? 1 : 0,
				duration = 2.0f,
				squareSize = new Vector2( 64f, 45f ),
				squareColor = Color.yellow,
				smoothness = 0.1f
			};
			TransitionKit.instance.transitionWithDelegate( squares );
		}
	}
}
