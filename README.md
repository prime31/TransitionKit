TransitionKit
=============

Modular, extensible transitions in-scene and between scenes. TransitionKit aims to make transitions easy. The main gist of how it works is that when invoked it snaps a screenshot and sticks it on a quad (by default but you can override this behavior). This means that you can reuse any of your full screen image effects with TransitionKit since it works in fundamentally the exact same way.


In-Scene Transitions
-----

Various situations require that you obscure what is happening such as when a player dies in a platformer and you want to reposition back to the level beginning. With TransitionKit, you can just start a transition which will obscure the screen then make any scene/camera modifications that you need to so that when the transition completes everything will be ready for the player to see.


Between Scene Transitions
-----

Switching scenes with Unity in a classy-looking fashion can be tricky. TransitionKit aims to simplify the process. All of the built in transitions allow you to specify a new scene to load and they will wait for it to load before the screen is unobscured.


Custom Transitions
-----

TransitionKit comes with a few transitions to get you started. The demo scene shows how to make a transition with an inspector for those that prefer going that route. There are also transitions that don't subclass MonoBehaviour as well. The basic idea is that you just subclass TransitionKitDelegate and implement a few methods. Your TransitionKitDelegate class has a couple optional features:

- you can provide a custom Shader by returning it via the shaderForTransition method
- you can provide a custom Mesh by returning it via the meshForDisplay method (a quad will be used if you return null)

TransitionKit will call your TransitionKitDelegate's onScreenObscured method as a coroutine and that is where you can setup your transition animations. When dealing with scene-to-scene transitions, you will not want to unobscure the screen until the new scene is loaded. To deal with that situation, the TransitionKit class has a waitForLevelToLoad method that you can yield on and it will return when the level is loaded.


License
-----

[Attribution-NonCommercial-ShareAlike 3.0 Unported](http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode) with [simple explanation](http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US) with the attribution clause waived. You are free to use StateKit in any and all games that you make. You cannot sell StateKit directly or as part of a larger game asset.
