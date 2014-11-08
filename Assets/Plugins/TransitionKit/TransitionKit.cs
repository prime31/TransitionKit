using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


namespace Prime31.TransitionKit
{
	public class TransitionKit : MonoBehaviour
	{
		/// <summary>
		/// fired when the screen has been fully obscured. You are clear to "do stuff" if need be when this fires
		/// </summary>
		public static event Action onScreenObscured;

		/// <summary>
		/// fired when the transition is complete and TransitionKit has destroyed all of its objects
		/// </summary>
		public static event Action onTransitionComplete;


		private bool _isInitialized = false;
		private const int _transitionKitLayer = 31;

		private TransitionKitDelegate _transitionKitDelegate;

		/// <summary>
		/// provides easy access to the camera used to obscure the screen. Handy when you want to change the clear flags for example.
		/// </summary>
		public Camera transitionKitCamera;

		/// <summary>
		/// material access for delegates so they can mess with shader/material properties
		/// </summary>
		public Material material;


		/// <summary>
		/// holds the instance while we are transitioning
		/// </summary>
		private static TransitionKit _instance;
		public static TransitionKit instance
		{
			get
			{
				if( !_instance )
				{
					// check if there is a TransitionKit instance already available in the scene graph before creating one
					_instance = FindObjectOfType( typeof( TransitionKit ) ) as TransitionKit;

					if( !_instance )
					{
						var obj = new GameObject( "TransitionKit" );
						obj.layer = _transitionKitLayer;
						obj.transform.position = new Vector3( 99999f, 99999f, 99999f );

						_instance = obj.AddComponent<TransitionKit>();
						DontDestroyOnLoad( obj );
					}
				}
				return _instance;
			}
		}


		void initialize()
		{
			if( _isInitialized )
				return;

			// create the MeshFilter
			gameObject.AddComponent<MeshFilter>().mesh = _transitionKitDelegate.meshForDisplay() ?? generateQuadMesh();

			// create the Material
			material = gameObject.AddComponent<MeshRenderer>().material;
			material.shader = _transitionKitDelegate.shaderForTransition() ?? Shader.Find( "Unlit/Texture" );

			// snapshot the main camera before proceeding
			_instance.StartCoroutine( _instance.setupCameraAndTexture() );

			_isInitialized = true;
		}


		Mesh generateQuadMesh()
		{
			var halfHeight = 5f; // 5 is the camera.orthoSize which is the half height
			var halfWidth = halfHeight * ( (float)Screen.width / (float)Screen.height );

			var mesh = new Mesh();
			mesh.vertices = new Vector3[]
			{
				new Vector3( -halfWidth, -halfHeight, 0 ),
				new Vector3( -halfWidth, halfHeight, 0 ),
				new Vector3( halfWidth, -halfHeight, 0 ),
				new Vector3( halfWidth, halfHeight, 0 )
			};
			mesh.uv = new Vector2[]
			{
				new Vector2( 0, 0 ),
				new Vector2( 0, 1 ),
				new Vector2( 1, 0 ),
				new Vector2( 1, 1 )
			};
			mesh.triangles = new int[] { 0, 1, 2, 3, 2, 1 };

			return mesh;
		}


		IEnumerator setupCameraAndTexture()
		{
			yield return new WaitForEndOfFrame();

			// load up the texture
			material.mainTexture = _transitionKitDelegate.textureForDisplay() ?? getScreenshotTexture();

			// create our camera to cover the screen
			transitionKitCamera = gameObject.AddComponent<Camera>();
			transitionKitCamera.isOrthoGraphic = true;
			transitionKitCamera.nearClipPlane = -1f;
			transitionKitCamera.farClipPlane = 1f;
			transitionKitCamera.depth = float.MaxValue;
			transitionKitCamera.cullingMask = 1 << _transitionKitLayer;

			if( _transitionKitDelegate != null )
				StartCoroutine( _transitionKitDelegate.onScreenObscured( this ) );

			if( onScreenObscured != null )
				onScreenObscured();
		}


		Texture2D getScreenshotTexture()
		{
			var screenSnapshot = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false, false );
			screenSnapshot.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0, false );
			screenSnapshot.Apply();

			return screenSnapshot;
		}


		#region Public

		/// <summary>
		/// starts up a transition with the given delegate
		/// </summary>
		/// <param name="transitionKitDelegate">Transition kit delegate.</param>
		public void transitionWithDelegate( TransitionKitDelegate transitionKitDelegate )
		{
			_transitionKitDelegate = transitionKitDelegate;
			initialize();
		}


		/// <summary>
		/// makes a single pixel Texture2D with a transparent pixel. Useful for fading from obscured to a new scene. Note that of course
		/// your shader must support transparency for this to be useful
		/// </summary>
		public void makeTextureTransparent()
		{
			var tex = new Texture2D( 1, 1 );
			tex.SetPixel( 0, 0, Color.clear );
			tex.Apply();

			material.mainTexture = tex;
		}


		/// <summary>
		/// delegates MUST call this when they are done with their transition! It signals a cleanup (duh) and notifies event listeners
		/// </summary>
		public void cleanup()
		{
			if( _instance == null )
				return;

			if( onTransitionComplete != null )
				onTransitionComplete();

			Destroy( gameObject );
			_instance = null;
			_isInitialized = false;
		}


		/// <summary>
		/// helper for delegates that returns control back when the given level has loaded. Very handy when using async loading.
		/// </summary>
		/// <returns>The for level to load.</returns>
		/// <param name="level">Level.</param>
		public IEnumerator waitForLevelToLoad( int level )
		{
			while( Application.loadedLevel != level )
				yield return null;
		}

		#endregion


	}
}