using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// Animation keyframe generation modes
public enum AnimationMode
{
	Linear,
	Curve
};

// States used to send callback to delegates
public enum AnimState
{	
	Update,
	Complete,
	Interrupted,
};

// Animation Delegate to receive callback events
public interface IAnimationDelegate
{
	void AnimCallback(string animId, AnimState animState, System.Object data = null);
	void RemoveAnims();
};

// Animation data structure
// Base class for anim type
public class Anim
{
	public GameObject 			targetObj;						// Object on which animation is to be played
	public float				loopCount;						// number of loops for animation
	public int					curLoopIndex;					// Variable used to keep track of loops completed
	public float				startDelay;						// start delay for animation
	public float				initTime;						// variable used to record the time when the anim is initialized
	
	public Anim()
	{
		targetObj = null;
		loopCount = 1;
		curLoopIndex = 1;
		startDelay = 0;
		initTime = -1;
	}	
	
	// Returns float value specifying time elapsed since initialization
	public float GetElapsedTime()
	{
		return Time.time - initTime;
	}
};

public class TransformAnim : Anim
{
	public Vector3[] 			path;							// Array containing animation key points
	public float				animSpeed;						// Speed of animation
	
	public int					curTargetIndex;					// Variable used to keep track of next key point in animation path
	
	public TransformAnim()
	{
		animSpeed = 0;
		curTargetIndex = 1;
	}
};

public class MoveTo : TransformAnim
{
	public AnimationMode 		animMode;						// Animation mode - Curve/Linear
	public BezierGenerator		bezier;							// Bezier object specifying the animation path - not applicable if animMode is Linear
	
	public MoveTo()
	{
		animMode = AnimationMode.Linear;
	}
};

public class RotateTo : TransformAnim
{
	public Vector3				angle;							// rotation angle
	public Vector3				axis;							// axis around which rotation has to take place - To be implemented
};

public class ScaleTo : TransformAnim
{
	// To be implemented
};

public class UnityAnim : Anim
{
	public string animName;										// Animation clip name
	
	public UnityAnim()
	{
		animName = "";
		curLoopIndex = 0;
	}
};

public class PrefabAnim: Anim
{
	public string animName;										// Prefab name
	public float animDuration;									// Duration of prefab anim
	
	public PrefabAnim()
	{
		animName = "";
	}
};

// Base class for anim playback
// Acts as a container for an anim/list of anims
public class AnimBase
{
	public string				m_id;							// String id for the anim
	public List<Anim>			m_anims;						// List of anims to be played
	public IAnimationDelegate	m_animDelegate;					// Delegate class to which callback events are to be sent
	public float				m_animStartDelay;				// Initial start delay for anims list
	public float				m_animInitTime;					// Keeps track of the time when anim was initialized to calculate time elapsed
	
	public AnimBase()
	{
		m_anims = new List<Anim>();
		m_animDelegate = null;
		m_animStartDelay = 0;
	}
	
	// Initialize a single anim
	public AnimBase(Anim anim, IAnimationDelegate animDelegate) : this()
	{
		m_anims.Add(anim);
		m_animDelegate = animDelegate;
	}
	
	// 
	public AnimBase(List<Anim> animList, IAnimationDelegate	animDelegate) : this()
	{
		m_anims = animList;
		m_animDelegate = animDelegate;
	}	
	
	public float GetElapsedTime()
	{
		return Time.time - m_animInitTime;
	}
};

// Derived type for playback of simultaneous animations
public class Spawn: AnimBase
{	
	public Spawn(List<Anim> animList, IAnimationDelegate animDelegate) : base(animList, animDelegate)
	{
	}
	// Add extra functionality later, if required
};

// Derived type for playback of sequential animations
public class Sequence: AnimBase
{
	public int m_currentAnimIndex;
	
	public Sequence(List<Anim> animList, IAnimationDelegate	animDelegate) : base(animList, animDelegate)
	{
		m_currentAnimIndex = 0;
	}
	
	public Anim GetCurrentAnim()
	{
		return m_anims[m_currentAnimIndex];
	}
	// Add extra functionality later, if required	
};

// Animation manager class
public class AnimationManager
{
	const float BEZIER_SPEED_FACTOR = 0.1f;					// Constant used to control the speed of bezier MoveTo anim
	
	static List<AnimBase> m_animList = new List<AnimBase>();
	
	// Create and return MoveTo object
	// targetObj - Object on which the animation clip has to be played
	// targetPos - target position to translate the targetObj to
	// animSpeed - animation speed
	public static MoveTo MoveTo(GameObject targetObj, Vector3 targetPos, float animSpeed)
	{		
		// Create a path from current pos to target pos
		Vector3[] path = {targetObj.transform.position, targetPos};		
		return MoveTo(targetObj, path, animSpeed, 1, 0);
	}
	
	// Create and return MoveTo object
	// targetObj - Object on which the animation clip has to be played
	// targetPos - target position to translate the targetObj to
	// animSpeed - animation speed
	// loopCount - loopCount for animation
	// startDelay - delay after which animation should be started
	public static MoveTo MoveTo(GameObject targetObj, Vector3 targetPos, float animSpeed, int loopCount, float startDelay)
	{		
		// Create a path from current pos to target pos
		Vector3[] path = {targetObj.transform.position, targetPos};		
		return MoveTo(targetObj, path, animSpeed, loopCount, startDelay);
	}
			
	// Create and return MoveTo object
	// targetObj - Object on which the animation clip has to be played
	// path -  array of 3d points specifying path for translation
	// animSpeed - animation speed
	// loopCount - loopCount for animation
	// startDelay - delay after which animation should be started
	public static MoveTo MoveTo(GameObject targetObj, Vector3[] path, float animSpeed, int loopCount, float startDelay)
	{
		MoveTo newAnim = new MoveTo();
		newAnim.targetObj = targetObj;
		newAnim.path = path;
		newAnim.animMode = AnimationMode.Linear;
		newAnim.animSpeed = animSpeed;
		newAnim.loopCount = loopCount;
		newAnim.startDelay = startDelay;
		newAnim.curTargetIndex = 1;
		
		return newAnim;
	}
		
	// Create and return MoveTo object (uses anim duration instead of anim speed)
	// targetObj - Object on which the animation clip has to be played
	// targetPos - target position to translate the targetObj to.
	// animDuration - animation duration
	// loopCount - loopCount for animation
	// startDelay - delay after which animation should be started
	public static MoveTo MoveToTimeBased(GameObject targetObj, Vector3 targetPos, float animDuration, int loopCount, float startDelay)
	{		
		// Create a path from current pos to target pos
		Vector3[] path = {targetObj.transform.position, targetPos};		
		return MoveToTimeBased(targetObj, path, animDuration, loopCount, startDelay);
	}
	
	// Create and return MoveTo object (uses anim duration instead of anim speed)
	// targetObj - Object on which the animation clip has to be played
	// path - array of 3d points specifying path for translation
	// animDuration - animation duration
	// loopCount - loopCount for animation
	// startDelay - delay after which animation should be started
	public static MoveTo MoveToTimeBased(GameObject targetObj, Vector3[] path, float animDuration, int loopCount, float startDelay)
	{
		MoveTo newAnim = new MoveTo();
		newAnim.targetObj = targetObj;
		newAnim.path = path;
		newAnim.animMode = AnimationMode.Linear;
		Vector3 displacementVector;		
		float distance = 0;
		for(int i = 0 ; i < path.Length - 1 ; i++)
		{
			displacementVector = path[i+1] - path[i];
			distance += displacementVector.magnitude;
		}
		newAnim.animSpeed = distance/animDuration;
		newAnim.loopCount = loopCount;
		newAnim.startDelay = startDelay;
		newAnim.curTargetIndex = 1;
		
		return newAnim;
	}
	
	// Create and return MoveTo object
	// targetObj - Object on which the animation clip has to be played
	// bezier - bezier path for translation
	// animSpeed - animation speed
	// loopCount - loopCount for animation
	// startDelay - delay after which animation should be started
	public static MoveTo MoveToBezier(GameObject targetObj, BezierGenerator bezier, float animSpeed, int loopCount, float startDelay)
	{
		MoveTo newAnim = new MoveTo();
		newAnim.targetObj = targetObj;
		newAnim.animMode = AnimationMode.Curve;
		newAnim.animSpeed = animSpeed * BEZIER_SPEED_FACTOR;
		newAnim.loopCount = loopCount;
		newAnim.startDelay = startDelay;
		newAnim.curTargetIndex = 1;
		newAnim.bezier = bezier;
		newAnim.path = new Vector3[2]{bezier.startPoint, bezier.endPoint};
		
		return newAnim;
	}
		
	// Create and return RotateTo object
	// targetObj - Object on which the animation clip has to be played
	// angle - rotation angle
	// animSpeed - animation speed
	// loopCount - loopCount for animation
	public static RotateTo RotateTo(GameObject targetObj, Vector3 angle, float animSpeed, int loopCount, float startDelay = 0.0f)
	{		
		RotateTo newAnim = new RotateTo();
		newAnim.targetObj = targetObj;
		newAnim.path = new Vector3[3];
		// NOTE : Unity always rotates an object in the direction of the acute angle. Therefore, we have to split the path into 3 vectors for rotations
		// greater than 180 degrees. The intermediate angle is to ensure that if that target angle is 360 degrees, a complete rotation happens instead of
		// 180 degrees back and forth
		// Fill same values for (x,y,z) in all the 3 path vectors for rotations less than 180 degrees
		if(angle.x >= 180)
		{
			newAnim.path[0].x = -180;
			newAnim.path[1].x = angle.x - 360 - 1;
			newAnim.path[2].x = angle.x - 360;
		}
		else
		{
			newAnim.path[0].x = newAnim.path[1].x = newAnim.path[2].x = angle.x;
		}
		
		if(angle.y >= 180)
		{
			newAnim.path[0].y = -180;
			newAnim.path[1].y = angle.y - 360 - 1;
			newAnim.path[2].y = angle.y - 360;
		}
		else
		{
			newAnim.path[0].y = newAnim.path[1].y = newAnim.path[2].y = angle.y;
		}
		
		if(angle.z >= 180)
		{
			newAnim.path[0].z = -180;
			newAnim.path[1].z = angle.z - 360 - 1;
			newAnim.path[2].z = angle.z - 360;
		}
		else
		{
			newAnim.path[0].z = newAnim.path[1].z = newAnim.path[2].z = angle.z;
		}
		
//		newAnim.axis = rotateAround;
		newAnim.animSpeed = animSpeed;
		newAnim.loopCount = loopCount;
		newAnim.startDelay = startDelay;
		newAnim.curTargetIndex = 0;
		
		return newAnim;
	}
	
//	public static RotateTo RotateAroundAxis(GameObject targetObj, Vector3 angle, Vector3 axis, float animSpeed, int loopCount)
//	{
//		return null;
//	}
	
	// Create and return UnityAnim object
	// targetObj - Object on which the animation clip has to be played
	// animName - animation clip name
	// loopCount - loopCount for animation
	// startDelay - delay after which the animation clip should be played
	public static UnityAnim UnityAnim(GameObject targetObj, string animName, int loopCount, float startDelay)
	{
		UnityAnim newAnim = new UnityAnim();
		newAnim.targetObj = targetObj;
		newAnim.animName = animName;
		newAnim.loopCount = loopCount;
		newAnim.startDelay = startDelay;
		
		Animation animComponent = targetObj.GetComponent<Animation>();
		// If there is no Animation component on targetObj, add one.
		if(animComponent == null)
			animComponent = targetObj.AddComponent<Animation>();
		
		return newAnim;
	}
	

	// Create and return PrefabAnim object
	// animName - name of the prefab
	// prefabPath - asset path for the prefab
	// pos - target position for initialized prefab
	// startDelay - delay after which the prefab should be activated
	// animDuration - duration of prefabAnim, callback for anim complete will be send after this duration along with destroying the object
	public static PrefabAnim PrefabAnim(string animName, string prefabPath, Vector3 pos, float startDelay, float animDuration = 5.0f)
	{
		StringBuilder prefabName =  new StringBuilder(prefabPath);
		prefabName.Append(animName);
		GameObject vfxPrefab = Object.Instantiate(Resources.Load(prefabName.ToString()), pos, Quaternion.identity) as GameObject;
		vfxPrefab.SetActive(false);
		PrefabAnim newAnim = new PrefabAnim();
		newAnim.targetObj = vfxPrefab;
		newAnim.startDelay = startDelay;
		newAnim.animDuration = animDuration;
		
		return newAnim;
	}
	
	// Helper method used to create a list of sequential anims
	public static Sequence Sequence(List<Anim> animList)
	{
		return new Sequence(animList, null);
	}
	
	// Helper method used to create a list of simultaneous anims
	public static Spawn Spawn(List<Anim> animList)
	{
		return new Spawn(animList, null);
	}
	
	// Play a single anim type
	public static void Play(Anim anim, IAnimationDelegate animDelegate = null, float startDelay = 0, string animId = "")
	{
		List<Anim> animList = new List<Anim>();
		animList.Add(anim);
		Spawn spawn = new Spawn(animList, animDelegate);
		spawn.m_animStartDelay = startDelay;
		spawn.m_id = animId;
		m_animList.Add(spawn);
		spawn.m_animInitTime = Time.time;
	}
	
	// Play a list of anims in a sequence
	public static void Play(Sequence sequence, IAnimationDelegate animDelegate = null, float startDelay = 0, string animId = "")
	{
		sequence.m_animDelegate = animDelegate;
		sequence.m_animStartDelay = startDelay;
		sequence.m_id = animId;
		m_animList.Add(sequence);		
		sequence.m_animInitTime = Time.time;
	}
	
	// Play a list of anims simultaneously
	public static void Play(Spawn spawn, IAnimationDelegate animDelegate = null, float startDelay = 0, string animId = "")
	{
		spawn.m_animDelegate = animDelegate;
		spawn.m_animStartDelay = startDelay;
		spawn.m_id = animId;
		m_animList.Add(spawn);		
		spawn.m_animInitTime = Time.time;
	}
	
	// Remove all anims linked to targetOnj
	public static void RemoveAllAnims(GameObject targetObj)
	{		
		for(int animBaseIndex = m_animList.Count - 1 ; animBaseIndex >= 0 ; animBaseIndex--)
		{
			for(int animIndex = m_animList[animBaseIndex].m_anims.Count - 1  ; animIndex >= 0 ; animIndex--)
			{
				if(m_animList[animBaseIndex].m_anims[animIndex].targetObj == targetObj)
				{
					m_animList[animBaseIndex].m_anims.RemoveAt(animIndex);
					if(m_animList[animBaseIndex].GetType() == typeof(Sequence))
					{
						Sequence sequence = (Sequence)m_animList[animBaseIndex];
						if(sequence.m_currentAnimIndex >= animIndex)
						{
							sequence.m_currentAnimIndex--;
							if(sequence.m_currentAnimIndex < 0)
							{
								if(sequence.m_animDelegate != null)
									sequence.m_animDelegate.AnimCallback(sequence.m_id, AnimState.Interrupted);
								m_animList.RemoveAt(animBaseIndex);
								continue;
							}
						}
					}
					
					// Remove sequence/spawn from the animList if count becomes zero
					if(m_animList[animBaseIndex].m_anims.Count == 0)
					{
						if(m_animList[animBaseIndex].m_animDelegate != null)
							m_animList[animBaseIndex].m_animDelegate.AnimCallback(m_animList[animBaseIndex].m_id, AnimState.Interrupted);
						m_animList.RemoveAt(animBaseIndex);
					}
				}
			}
		}
	}
	
	// Copies all anims from targetObj to newObj
	public static void CopyAllAnims(GameObject targetObj, GameObject newObj)
	{	
		// Do nothing if either object is null
		if(targetObj == null || newObj == null)
			return;
		
		// Find the object in the current anims list and, if found, copy anims
		for(int animBaseIndex = m_animList.Count - 1 ; animBaseIndex >= 0 ; animBaseIndex--)
		{
			for(int animIndex = m_animList[animBaseIndex].m_anims.Count - 1 ; animIndex >= 0 ; animIndex--)
			{
				if(m_animList[animBaseIndex].m_anims[animIndex].targetObj == targetObj)
					m_animList[animBaseIndex].m_anims[animIndex].targetObj = newObj;
			}
		}
	}
	
	public static void Update()
	{		
		// Update anims on the active list
		for(int animBaseIndex = m_animList.Count - 1 ; animBaseIndex >= 0 ; animBaseIndex--)
		{			
			AnimBase animBase = m_animList[animBaseIndex];			
			if(animBase.GetElapsedTime() < animBase.m_animStartDelay)
				continue;
			
			if(animBase.GetType() == typeof(Spawn))
			{
				Spawn spawn = (Spawn)animBase;
				bool bSpawnAnimsComplete = true;
				for(int animIndex = spawn.m_anims.Count - 1 ; animIndex >= 0 ; animIndex--)
				{
					Anim anim = spawn.m_anims[animIndex];
					
					if(anim.targetObj == null)
					{
						Debug.Log("Warning! Animation target has become null! Removing associated anim.");
						spawn.m_anims.RemoveAt(animIndex);
						continue;
					}
					
					// Set anim start time
					if(anim.initTime < 0)
						anim.initTime = Time.time;
					
					// Check for anim start delay
					if(anim.GetElapsedTime() < anim.startDelay)
					{
						bSpawnAnimsComplete = false;
						continue;
					}
					
					if(spawn.m_animDelegate != null)
					{
						spawn.m_animDelegate.AnimCallback(spawn.m_id, AnimState.Update, spawn.GetElapsedTime());
					}
					
					// Call update method for the appropriate anim type
					// All anims will be updated simultaneously in Spawn anim type
					if(anim.GetType() == typeof(MoveTo))
						bSpawnAnimsComplete &= UpdateMoveAnim((MoveTo)anim);
					if(anim.GetType() == typeof(RotateTo))
						bSpawnAnimsComplete &= UpdateRotateAnim((RotateTo)anim);
					if(anim.GetType() == typeof(UnityAnim))
						bSpawnAnimsComplete &= UpdateUnityAnim((UnityAnim)anim);
//					if(anim.GetType() == typeof(PrefabAnim))
//						bSpawnAnimsComplete &= UpdatePrefabAnim((PrefabAnim)anim);
				}
				
				// Check if all anims in the list are complete
				if(bSpawnAnimsComplete)
				{
					// Send callback if specified
					if(spawn.m_animDelegate != null)
						spawn.m_animDelegate.AnimCallback(spawn.m_id, AnimState.Complete);
					
					// Remove from anim list
					m_animList.RemoveAt(animBaseIndex);
				}
			}
			else if(animBase.GetType() == typeof(Sequence))
			{				
				Sequence sequence = (Sequence)animBase;
				// Get current anim in the sequence
				Anim anim = sequence.GetCurrentAnim();
					
				if(anim.targetObj == null)
				{
					Debug.Log("Warning! Animation target has become null! Removing associated anim.");
					sequence.m_anims.Remove(anim);
					if(sequence.m_currentAnimIndex >= sequence.m_anims.Count)
					{
						if(sequence.m_animDelegate != null)
							sequence.m_animDelegate.AnimCallback(sequence.m_id, AnimState.Interrupted);
						m_animList.RemoveAt(animBaseIndex);			
					}
					continue;
				}
					
				// Set anim start time
				if(anim.initTime < 0)
					anim.initTime = Time.time;
				
				// Check for anim start delay
				if(anim.GetElapsedTime() < anim.startDelay)
					continue;
				
				if(sequence.m_animDelegate != null)
				{
					sequence.m_animDelegate.AnimCallback(sequence.m_id, AnimState.Update, sequence.GetElapsedTime());
				}
				
				// Call update method for the current anim in the sequence
				if(anim.GetType() == typeof(MoveTo))
					if(UpdateMoveAnim((MoveTo)anim))
						sequence.m_currentAnimIndex++;
				
				if(anim.GetType() == typeof(RotateTo))
					if(UpdateRotateAnim((RotateTo)anim))
						sequence.m_currentAnimIndex++;
				
				if(anim.GetType() == typeof(UnityAnim))
					if(UpdateUnityAnim((UnityAnim)anim))
						sequence.m_currentAnimIndex++;
				
				if(anim.GetType() == typeof(PrefabAnim))
					if(UpdatePrefabAnim((PrefabAnim)anim))
						sequence.m_currentAnimIndex++;
				
				// Check for end of sequence
				if(sequence.m_currentAnimIndex >= sequence.m_anims.Count)
				{
					// Send callback if specified
					if(sequence.m_animDelegate != null)
						sequence.m_animDelegate.AnimCallback(sequence.m_id, AnimState.Complete);
					
					//Debug.Log("crashed = " + m_animList.Count);
					// Remove from anim list
					if(m_animList!=null && m_animList.Count>0)
					m_animList.RemoveAt(animBaseIndex);
				}
			}
		}
	}
		
	// Updates MoveTo anim type
	// Returns true if anim is over
	private static bool UpdateMoveAnim(MoveTo moveAnim)
	{
		bool bAnimComplete = false;
		
		Vector3 targetPoint = moveAnim.path[moveAnim.curTargetIndex];
		if(moveAnim.animMode == AnimationMode.Linear)
		{
			moveAnim.targetObj.transform.position = Vector3.MoveTowards(moveAnim.targetObj.transform.position,  moveAnim.path[moveAnim.curTargetIndex], moveAnim.animSpeed * Time.deltaTime);
		}
		else
		{
			targetPoint = moveAnim.bezier.endPoint;
			moveAnim.targetObj.transform.position = moveAnim.bezier.GetPointAtTime(moveAnim.animSpeed * Time.deltaTime);
		}
		if(targetPoint == moveAnim.targetObj.transform.position)
		{
			if(moveAnim.curTargetIndex < moveAnim.path.Length - 1)
			{
				// Step to the next animation key point
				moveAnim.curTargetIndex++;
			}
			else
			{
				if(moveAnim.curLoopIndex == moveAnim.loopCount)
				{
					bAnimComplete = true;
				}
				else
				{
					moveAnim.curLoopIndex++;
					moveAnim.curTargetIndex = 0;
				}
			}
		}
		
		return bAnimComplete;
	}
		
	// Updates RotateTo anim type
	// Returns true if anim is over
	private static bool UpdateRotateAnim(RotateTo rotateAnim)
	{
		bool bAnimComplete = false;		
		
		Quaternion targetRotation = Quaternion.Euler(rotateAnim.path[rotateAnim.curTargetIndex]);
		rotateAnim.targetObj.transform.rotation = Quaternion.RotateTowards(rotateAnim.targetObj.transform.rotation, targetRotation, rotateAnim.animSpeed * Time.deltaTime);
		//if(anim.path[anim.curTargetIndex] == anim.targetObj.transform.eulerAngles)
//		Debug.Log("targetRotation.eulerAngles  "+targetRotation.eulerAngles.x + "y "+targetRotation.eulerAngles.y +"z "+targetRotation.eulerAngles.z);
//		Debug.Log("rotateAnim.targetObj.transform.eulerAngles  "+rotateAnim.targetObj.transform.eulerAngles.x + "y "+rotateAnim.targetObj.transform.eulerAngles.y +"z "+rotateAnim.targetObj.transform.eulerAngles.z);

		if(targetRotation.eulerAngles.normalized == rotateAnim.targetObj.transform.eulerAngles.normalized)
		{
			if(rotateAnim.curTargetIndex < rotateAnim.path.Length - 1)
			{
				// Step to the next animation key point
				rotateAnim.curTargetIndex++;
			}
			else
			{
				if(rotateAnim.curLoopIndex == rotateAnim.loopCount)
				{
					bAnimComplete = true;
				}
				else
				{
					rotateAnim.curLoopIndex++;
					rotateAnim.curTargetIndex = 1;
				}
			}
		}
		
		return bAnimComplete;
	}
		
	// Updates UnityAnim anim type
	// Returns true if anim is over
	private static bool UpdateUnityAnim(UnityAnim unityAnim)
	{
		bool bAnimComplete = false;		
		
		if(!unityAnim.targetObj.GetComponent<Animation>().IsPlaying(unityAnim.animName))
		{
			// Infinite loop
			if(unityAnim.loopCount == 0)					
				unityAnim.targetObj.GetComponent<Animation>().Play(unityAnim.animName);
			// Finite loop count
			else
			{
				if(unityAnim.curLoopIndex == unityAnim.loopCount)
				{
					bAnimComplete = true;
				}
				else
				{
					unityAnim.curLoopIndex++;
					unityAnim.targetObj.GetComponent<Animation>().Play(unityAnim.animName);
				}
			}
		}
		
		return bAnimComplete;
	}
		
	// Updates PrefabAnim anim type
	// Returns true if anim is over
	private static bool UpdatePrefabAnim(PrefabAnim prefabAnim)
	{
		bool bAnimComplete = false;
		if(!prefabAnim.targetObj.activeSelf)
		{
			prefabAnim.targetObj.SetActive(true);
//			if(prefabAnim.animDuration > 0)
//				GameObject.Destroy(prefabAnim.targetObj, prefabAnim.animDuration);
		}
		else
		{
			bAnimComplete = (Time.time - prefabAnim.initTime) > prefabAnim.animDuration;
			if(bAnimComplete)
				GameObject.Destroy(prefabAnim.targetObj);
		}
		
		return bAnimComplete;
	}
}

