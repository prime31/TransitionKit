using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


namespace Prime31.TransitionKit
{
	public class TransitionKit : MonoBehaviour
	{
		private static bool _isInitialized = false;
		private const int _transitionKitLayer = 31;

		private Texture2D _screenSnapshot;
		private TransitionKitDelegate _transitionKitDelegate;

		public Camera transitionKitCamera;
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
			_instance.StartCoroutine( _instance.captureScreenshotAndSetupCamera() );

			_isInitialized = true;
		}


		Mesh generateQuadMesh()
		{
			var halfHeight = 0.5f * 2f * 5f;
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


		IEnumerator captureScreenshotAndSetupCamera()
		{
			yield return new WaitForEndOfFrame();

			_screenSnapshot = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false, false );
			_screenSnapshot.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0, false );
			_screenSnapshot.Apply();

			// load up the texture
			material.mainTexture = _screenSnapshot;

			// create our camera to cover the screen
			transitionKitCamera = gameObject.AddComponent<Camera>();
			transitionKitCamera.isOrthoGraphic = true;
			transitionKitCamera.nearClipPlane = -1f;
			transitionKitCamera.farClipPlane = 1f;
			transitionKitCamera.depth = float.MaxValue;
			transitionKitCamera.cullingMask = 1 << _transitionKitLayer;

			if( _transitionKitDelegate != null )
				StartCoroutine( _transitionKitDelegate.onScreenObscured( this ) );
		}


		void OnLevelWasLoaded( int level )
		{
			if( _transitionKitDelegate != null )
				_transitionKitDelegate.onLevelWasLoaded( this, level );
		}


		#region Public

		public void transitionWithDelegate( TransitionKitDelegate transitionKitDelegate )
		{
			_transitionKitDelegate = transitionKitDelegate;
			initialize();
		}


		public void cleanup()
		{
			if( _instance == null )
				return;

			Destroy( _screenSnapshot );
			_instance._screenSnapshot = null;

			Destroy( gameObject );
			_instance = null;
			_isInitialized = false;
		}


		public IEnumerator waitForLevelToLoad( int level )
		{
			while( Application.loadedLevel != level )
				yield return null;
		}

		#endregion


	}
}