using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;


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


		/// <summary>
		/// if true, TransitionKit will not remove itself when the transition is complete. Instead, it will just disable itself and free up used resources.
		/// </summary>
		public static bool keepTransitionKitInstance = false;

		/// <summary>
		/// the layer we will use for TK. The camera will be set to only render this layer.
		/// </summary>
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
		/// sets whether TransitionKit will use unscaledDeltaTime or standard deltaTime
		/// </summary>
		public bool useUnscaledDeltaTime = false;

		/// <summary>
		/// helper property for use by all TransitionKitDelegates so they use the proper deltaTime
		/// </summary>
		/// <value>The delta time.</value>
		public float deltaTime
		{
			get { return useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime; }
		}

		/// <summary>
		/// stick whatever you want in there so that when the events fire you can grab it and avoid the Action allocations
		/// </summary>
		public object context;


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


		#region Private

		T getOrAddComponent<T>() where T : Component
		{
			var component = gameObject.GetComponent<T>();
			if( component == null )
				component = gameObject.AddComponent<T>();

			return component;
		}


		void initialize()
		{
			// create the MeshFilter
			var meshFilter = getOrAddComponent<MeshFilter>();
			meshFilter.mesh = _transitionKitDelegate.meshForDisplay() ?? generateQuadMesh();

			// create the Material
			material = getOrAddComponent<MeshRenderer>().material;
			material.shader = _transitionKitDelegate.shaderForTransition() ?? Shader.Find( "prime[31]/Transitions/Texture With Alpha" );
			material.color = Color.white; // reset to fully white

			// snapshot the main camera before proceeding
			_instance.StartCoroutine( _instance.setupCameraAndTexture() );
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
			transitionKitCamera = getOrAddComponent<Camera>();

			// always reset these in case a transition messed with them
			transitionKitCamera.orthographic = true;
			transitionKitCamera.nearClipPlane = -1f;
			transitionKitCamera.farClipPlane = 1f;
			transitionKitCamera.depth = float.MaxValue;
			transitionKitCamera.cullingMask = 1 << _transitionKitLayer;
			transitionKitCamera.clearFlags = CameraClearFlags.Nothing;
			transitionKitCamera.enabled = true;

			if( onScreenObscured != null )
				onScreenObscured();

			yield return StartCoroutine( _transitionKitDelegate.onScreenObscured( this ) );

			cleanup();
		}


		Texture2D getScreenshotTexture()
		{
			var screenSnapshot = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false, false );
			screenSnapshot.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0, false );
			screenSnapshot.Apply();

			return screenSnapshot;
		}


		/// <summary>
		/// this method signals a cleanup (duh) and notifies event listeners
		/// </summary>
		private void cleanup()
		{
			if( _instance == null )
				return;

			if( onTransitionComplete != null )
				onTransitionComplete();

			_transitionKitDelegate = null;
			context = null;

			// if we are keeping TK alive we only need to free resources and not delete ourself completely
			if( keepTransitionKitInstance )
			{
				GetComponent<MeshRenderer>().material.mainTexture = null;
				GetComponent<MeshFilter>().mesh = null;
				gameObject.SetActive( false );
				transitionKitCamera.enabled = false;
			}
			else
			{
				Destroy( gameObject );
				_instance = null;
			}
		}

		#endregion


		#region Public

		/// <summary>
		/// starts up a transition with the given delegate
		/// </summary>
		/// <param name="transitionKitDelegate">Transition kit delegate.</param>
		public void transitionWithDelegate( TransitionKitDelegate transitionKitDelegate )
		{
			gameObject.SetActive( true );
			_transitionKitDelegate = transitionKitDelegate;
			initialize();
		}


		/// <summary>
		/// makes a single pixel Texture2D with a transparent pixel and sets it on the current Material. Useful for fading from obscured to a
		/// new scene. Note that of course your shader must support transparency for this to be useful
		/// </summary>
		public void makeTextureTransparent()
		{
			var tex = new Texture2D( 1, 1 );
			tex.SetPixel( 0, 0, Color.clear );
			tex.Apply();

			material.mainTexture = tex;
		}


		/// <summary>
		/// helper for delegates that returns control back when the given level has loaded. Very handy when using async loading.
		/// </summary>
		/// <returns>The for level to load.</returns>
		/// <param name="level">Level.</param>
		public IEnumerator waitForLevelToLoad( int level )
		{
			while( SceneManager.GetActiveScene().buildIndex != level )
				yield return null;
		}


		/// <summary>
		/// the most common type of transition seems to be one that ticks progress from 0 - 1. This method takes care of that for you
		/// if your transition needs to have a _Progress property ticked after the scene loads.
		/// </summary>
		/// <param name="duration">duration</param>
		/// <param name="reverseDirection">if true, _Progress will go from 1 to 0. If false, it goes form 0 to 1</param>
		public IEnumerator tickProgressPropertyInMaterial( float duration, bool reverseDirection = false )
		{
			var start = reverseDirection ? 1f : 0f;
			var end = reverseDirection ? 0f : 1f;

			var elapsed = 0f;
			while( elapsed < duration )
			{
				elapsed += deltaTime;
				var step = Mathf.Lerp( start, end, Mathf.Pow( elapsed / duration, 2f ) );
				material.SetFloat( "_Progress", step );

				yield return null;
			}
		}

		#endregion


	}
}