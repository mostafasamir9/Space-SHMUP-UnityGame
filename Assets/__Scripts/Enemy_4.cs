using UnityEngine;
using System.Collections;

// Part is another serializable data storage class just like WeaponDefinition
[System.Serializable]
public class Part
{
	// These three fields need to be defined in the Inspector pane
	public string name;
	public float health;
	public string[] protectedBy;

	// These two fields are set automatically in Start()
	public GameObject go;
	public Material mat;
}

public class Enemy_4 : Enemy
{
	// Enemy_4 will start offscreen and then pick a random point on screen to
	// move to. Once it has arrived, it will pick another random point and 
	// continue until the player has shot it down

	public Vector3[] points;
	public float timeStart;
	public float duration = 4;

	public Part[] parts;

	void Start()
	{
		points = new Vector3[2];
		// There is already an initial position chosen by Main.SpawnEnemy()
		// so add it to points as the initial p0 & p1
		points[0] = pos;
		points[1] = pos;

		InitMovement();

		// Cache GameObject & Material of each Part in parts
		Transform t;
		foreach (Part prt in parts)
		{
			t = transform.Find(prt.name);
			if (t != null)
			{
				prt.go = t.gameObject;
				prt.mat = prt.go.GetComponent<Renderer>().material;
			}
		}
	}

	void InitMovement()
	{
		// Pick a new point to move to that is on screen
		Vector3 p1 = Vector3.zero;
		float esp = Main.S.enemySpawnPadding;
		Bounds cBounds = Utils.camBounds;
		p1.x = Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
		p1.y = Random.Range(cBounds.min.y + esp, cBounds.max.y - esp);

		points[0] = points[1];
		points[1] = p1;

		// Reset the time
		timeStart = Time.time;
	}

	public override void Move()
	{
		// This completely overrides Enemy.Move() with a linear interpolation

		float u = (Time.time - timeStart) / duration;
		if (u >= 1)
		{
			InitMovement();
			u = 0;
		}

		u = 1 - Mathf.Pow(1 - u, 2);
		pos = (1 - u) * points[0] + u * points[1];
	}

	// This will override the OnCollisionEnter that is part of Enemy.cs
	// Because of the way that MonoBehavior declares common Unity functions
	// like OnCollisionEnter(), the override keyword is not necessary.
	void OnCollisionEnter(Collision coll)
	{
		GameObject other = coll.gameObject;
		switch (other.tag)
		{
			case "ProjectileHero":
				Projectile p = other.GetComponent<Projectile>();
				// Enemies don't take damage unless they're on screen
				// This stops the player from shooting them before they are visible
				bounds.center = transform.position + boundsCenterOffset;
				if (bounds.extents == Vector3.zero ||
				   Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero)
				{
					Destroy(other);
					break;
				}

				// Hurt the Enemy
				GameObject goHit = coll.contacts[0].thisCollider.gameObject;
				Part prtHit = FindPart(goHit);
				if (prtHit == null)
				{
					goHit = coll.contacts[0].otherCollider.gameObject;
					prtHit = FindPart(goHit);
				}
				// Check whether this part is still protected
				if (prtHit.protectedBy != null)
				{
					foreach (string s in prtHit.protectedBy)
					{
						if (!Destroyed(s))
						{
							Destroy(other);
							return;
						}
					}
				}

				prtHit.health -= Main.W_DEFS[p.type].damageOnHit;

				ShowLocalizedDamage(prtHit.mat);
				if (prtHit.health <= 0)
				{
					prtHit.go.SetActive(false);
				}

				bool allDestroyed = true;
				foreach (Part prt in parts)
				{
					if (!Destroyed(prt))
					{
						allDestroyed = false;
						break;
					}
				}

				if (allDestroyed)
				{
					int flag = 0;
					Main.S.ShipDestroyed(this,flag);
					Destroy(this.gameObject);
				}

				Destroy(other);
				break;
		}

	}

	Part FindPart(string n)
	{
		foreach (Part prt in parts)
		{
			if (prt.name == n)
			{
				return (prt);
			}
		}
		return (null);
	}

	Part FindPart(GameObject go)
	{
		foreach (Part prt in parts)
		{
			if (prt.go == go)
			{
				return (prt);
			}
		}

		return (null);
	}

	bool Destroyed(GameObject go)
	{
		return (Destroyed(FindPart(go)));
	}

	bool Destroyed(string n)
	{
		return (Destroyed(FindPart(n)));
	}

	bool Destroyed(Part prt)
	{
		if (prt == null)
		{
			return (true);
		}
		return (prt.health <= 0);
	}

	void ShowLocalizedDamage(Material m)
	{
		m.color = Color.red;
		remainingDamageFrames = showDamageForFrames;
	}
}
