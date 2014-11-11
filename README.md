TransitionKit
=============

Modular, extensible transitions in-scene and between scenes. TransitionKit aims to make transitions easy. The main gist of how it works is that when invoked it snaps a screenshot and sticks it on a quad (by default but you can override this behavior). This means that you can reuse any of your full screen image effects with TransitionKit since it works in fundamentally the exact same way.

Side notes: if you make any neat transitions feel free to send over a pull request so we can build up a nice library of useful transitions! If the shaders used in your transitions (or the included transitions) are not used anywhere else you have to tell Unity to still included them in the build. You can do this by opening Edit -> Project Settings -> Graphics and stick the shaders in the "Always Included Shaders": ![Graphics Settings](http://cl.ly/YTh4/Screen%20Shot%202014-11-11%20at%209.11.14%20AM.png)



In-Scene Transitions
-----

Various situations require that you obscure what is happening such as when a player dies in a platformer and you want to reposition back to a spawn point or level start. With TransitionKit, you can just start a transition which will obscure the screen then make any scene/camera modifications that you need to so that when the transition completes everything will be ready for the player to see.



Between Scene Transitions
-----

Switching scenes with Unity in a classy-looking fashion can be tricky. TransitionKit aims to simplify the process. All of the built in transitions allow you to specify an optional new scene to load and they will wait for it to load before the screen is unobscured.



Show Me Some Code!
-----

This is an example of doing a scene-to-scene transition using the included FadeTransition. The FadeTransition will fade to the specified Color and then fade out to the newly loaded scene. If you wanted to do this transition in-scene, you would just omit setting the nextScene field.

	var fader = new FadeTransition()
	{
		nextScene = 3,
		fadeToColor = Color.white
	};
	TransitionKit.instance.transitionWithDelegate( fader );


The PixelatorTransition is a bit different than the others. Rather than pixelate both out and then in it will pixelate out the current scene and then uses an animation to wipe/zoom the old scene out of the way. The reason it does this is because pixelating the new scene back in would result in a jarring change between the first and second scenes. It provides a bit of variety as well and shows how you can do animations on the quad displaying the screenshot.

	var pixelater = new PixelateTransition()
	{
		nextScene = 2,
		finalScaleEffect = PixelateTransition.PixelateFinalScaleEffect.ToPoint,
		duration = 1.0f
	};
	TransitionKit.instance.transitionWithDelegate( pixelater );



Custom Transitions
-----

TransitionKit comes with a few transitions to get you started. The demo scene shows how to make a transition with an inspector for those that prefer going that route (it uses the standard Unity whirl image effect so you can also see how to convert your image effects to TransitionKit). There are also transitions that don't subclass MonoBehaviour as well. The basic idea is that you just implement TransitionKitDelegate interface and implement a few methods. Your TransitionKitDelegate class has a couple optional features:

- you can provide a custom Shader by returning it via the shaderForTransition method (the Unlit/Texture shader will be used if you return null)
- you can provide a custom Mesh by returning it via the meshForDisplay method (a quad will be used if you return null)
- you can provide a custom Texture by returning it via the textureForDisplay method (a screenshot will be used if you return null)

TransitionKit will call your TransitionKitDelegate's onScreenObscured method as a coroutine and that is where you can setup your transition animations. When dealing with scene-to-scene transitions, you will not want to unobscure the screen until the new scene is loaded. To deal with that situation, the TransitionKit class has a waitForLevelToLoad method that you can yield on and it will return when the level is loaded.



Events
-----

TransitionKit provides two events as well: onScreenObscured and onTransitionComplete. I'm pretty sure you can figure out when they are fired but if you can't just load up the demo scene and watch the logs.



License
-----

Some of the transition shaders were adapted from [GLSL-Transition](https://github.com/glslio/glsl-transition).

[Attribution-NonCommercial-ShareAlike 3.0 Unported](http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode) with [simple explanation](http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US) with the attribution clause waived. You are free to use StateKit in any and all games that you make. You cannot sell StateKit directly or as part of a larger game asset.
