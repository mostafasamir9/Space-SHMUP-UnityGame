using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum BoundsTest
{
	center,         // is the center of the gameObject on screen?
	onScreen,       // are bounds entirely on screen
	offScreen       //are bounds entirely off screen
}

public class Utils : MonoBehaviour
{
	// create bounds that expand to hold the two bounds passed in
	public static Bounds BoundsUnion(Bounds b0, Bounds b1)
	{
		if (b0.size == Vector3.zero && b1.size != Vector3.zero)
		{
			return (b1);
		}
		else if (b0.size != Vector3.zero && b1.size == Vector3.zero)
		{
			return (b0);
		}
		else if (b0.size == Vector3.zero && b1.size == Vector3.zero)
		{
			return (b0);
		}

		// else combine them
		b0.Encapsulate(b1.min);
		b0.Encapsulate(b1.max);
		return (b0);
	}


	public static Bounds CombineBoundsOfChildren(GameObject go)
	{
		Bounds b = new Bounds(Vector3.zero, Vector3.zero);
		if (go.GetComponent<Renderer>() != null) //renderer
		{
			b = BoundsUnion(b, go.GetComponent<Renderer>().bounds); // go.renderer.bounds);
		}

		if (go.GetComponent<Collider>() != null) //collider
		{
			b = BoundsUnion(b, go.GetComponent<Renderer>().bounds);
		}

		foreach (Transform t in go.transform)
		{
			b = BoundsUnion(b, CombineBoundsOfChildren(t.gameObject));
		}

		return (b);

	}


	// PRIVATE VARIABLE
	static private Bounds _camBounds;

	//PROPERTY
	static public Bounds camBounds
	{
		get
		{
			if (_camBounds.size == Vector3.zero)
			{
				SetCameraBounds();
			}
			return (_camBounds);
		} // end of get
	}

	//function used by camBound property and also may be called directly

	public static void SetCameraBounds(Camera cam = null)
	{
		// use the main camera as default if none passed in.
		if (cam == null)
			cam = Camera.main;

		// assuming camera is orthographic and does not have any rotation applied to it	
		// get top left and bottomRight

		Vector3 topLeft = new Vector3(0, 0, 0);
		Vector3 bottomRight = new Vector3(Screen.width, Screen.height, 0f);

		Vector3 boundTLN = cam.ScreenToWorldPoint(topLeft);
		Vector3 boundBRF = cam.ScreenToWorldPoint(bottomRight);

		boundTLN.z = cam.nearClipPlane;
		boundBRF.z = cam.farClipPlane;

		Vector3 center = (boundTLN + boundBRF) / 2f;

		_camBounds = new Bounds(center, Vector3.zero);
		_camBounds.Encapsulate(boundTLN);
		_camBounds.Encapsulate(boundBRF);
	} // end setCameraBounds

	// checks to see whether the bounds bnd are within the camBounds
	public static Vector3 ScreenBoundsCheck(Bounds bnd, BoundsTest test = BoundsTest.center)
	{
		return (BoundsInBoundsCheck(camBounds, bnd, test));
	}

	// Checks to see if bounds lilb are within Bounds bigB
	public static Vector3 BoundsInBoundsCheck(Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen)
	{
		// behavior needs to be different depending on the test selected

		Vector3 pos = lilB.center;      // use center for measurement
		Vector3 off = Vector3.zero;     // offset is 0,0,0 to start

		switch (test)
		{
			// what is offset to move center of lilB back inside bigB
			case BoundsTest.center:
				// trivial case - we are already inside
				if (bigB.Contains(pos))
				{
					return (Vector3.zero);   //no need to move
				}

				//otherwise adjust x,y,z components as needed
				if (pos.x > bigB.max.x)
				{
					off.x = pos.x - bigB.max.x;
				}
				else if (pos.x < bigB.min.x)
				{
					off.x = pos.x - bigB.min.x;
				}

				if (pos.y > bigB.max.y)
				{
					off.y = pos.y - bigB.max.y;
				}
				else if (pos.y < bigB.min.y)
				{
					off.y = pos.y - bigB.min.y;
				}

				if (pos.z > bigB.max.z)
				{
					off.z = pos.z - bigB.max.z;
				}
				else if (pos.z < bigB.min.z)
				{
					off.z = pos.z - bigB.min.z;
				}

				return (off);

			//-------------------------
			// what is the offset to keep ALL of lilB inside bigB
			case BoundsTest.onScreen:
				// trivial case - we are already inside
				if (bigB.Contains(lilB.max) && bigB.Contains(lilB.min))
				{
					return (Vector3.zero);   //no need to move
				}

				if (lilB.max.x > bigB.max.x)
				{
					off.x = lilB.max.x - bigB.max.x;
				}
				else if (lilB.min.x < bigB.min.x)
				{
					off.x = lilB.min.x - bigB.min.x;
				}

				if (lilB.max.y > bigB.max.y)
				{
					off.y = lilB.max.y - bigB.max.y;
				}
				else if (lilB.min.y < bigB.min.y)
				{
					off.y = lilB.min.y - bigB.min.y;
				}

				if (lilB.max.z > bigB.max.z)
				{
					off.z = lilB.max.z - bigB.max.z;
				}
				else if (lilB.min.z < bigB.min.z)
				{
					off.z = lilB.min.z - bigB.min.z;
				}

				return (off);

			//-------------------------
			// what is the offset to keep ALL of lilB outside of bigB					
			case BoundsTest.offScreen:
				bool cMin = bigB.Contains(lilB.min);
				bool cMax = bigB.Contains(lilB.max);

				if (cMin || cMax)
				{
					return (Vector3.zero);
				}


				if (lilB.min.x > bigB.max.x)
				{
					off.x = lilB.min.x - bigB.max.x;
				}
				else if (lilB.max.x < bigB.min.x)
				{
					off.x = lilB.max.x - bigB.min.x;
				}

				if (lilB.min.y > bigB.max.y)
				{
					off.y = lilB.min.y - bigB.max.y;
				}
				else if (lilB.max.y < bigB.min.y)
				{
					off.y = lilB.max.y - bigB.min.y;
				}

				if (lilB.min.z > bigB.max.z)
				{
					off.z = lilB.min.z - bigB.max.z;
				}
				else if (lilB.max.z < bigB.min.z)
				{
					off.z = lilB.max.z - bigB.min.z;
				}

				return (off);
		} // end switch BoundsTest

		return (Vector3.zero);  // if we get here something went wrong

	} // end BoundsInBoundsCheck



}// End of Util Class