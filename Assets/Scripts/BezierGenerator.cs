using UnityEngine;
using System.Collections;

public class BezierGenerator
{ 
    public Vector3 startPoint;
    public Vector3 controlPoint1;
    public Vector3 controlPoint2;
    public Vector3 endPoint;
 
    public float time = 0f; 

    private Vector3 b0 = Vector3.zero;
    private Vector3 b1 = Vector3.zero;
    private Vector3 b2 = Vector3.zero;
    private Vector3 b3 = Vector3.zero; 

    private float Ax;
    private float Ay;
    private float Az; 

    private float Bx;
    private float By;
    private float Bz; 

    private float Cx;
    private float Cy;
    private float Cz;
	
	const float LENGTH_OFFSET_RATIO = 0.35f;
 
    // Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
    // handle1 = v0 + v1
    // handle2 = v3 + v2
    public BezierGenerator( Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3 )
    {
        this.startPoint = v0;
        this.controlPoint1 = v1;
        this.controlPoint2 = v2;
        this.endPoint = v3;
    } 
	
	public BezierGenerator(Vector3 startPoint, Vector3 endPoint)
	{
		// Temp logic to generate a random control point given start point and end point
		// ToDo - find a better algorithm to generate control points
		
		this.startPoint = startPoint;
        this.endPoint = endPoint;
		
		float offset = (endPoint - startPoint).magnitude * LENGTH_OFFSET_RATIO;
		
		Vector3 interim = (endPoint + startPoint) / 2;
		if(interim.x == endPoint.x && interim.x == startPoint.x)
		{
			int random = Mathf.RoundToInt(UnityEngine.Random.value * 2);
			interim.x += (random % 2 == 0) ? offset : -offset;
		}
		if(interim.y == startPoint.y && interim.y == endPoint.y)
		{
			int random = Mathf.RoundToInt(UnityEngine.Random.value * 2);
			interim.y += (random % 2 == 0) ? offset : -offset;
		}
		
		this.controlPoint1 = interim;
		this.controlPoint2 = interim;
	}

    // 0.0 >= t <= 1.0
    public Vector3 GetPointAtTime( float deltatime )
    {
		time += deltatime;
		time = Mathf.Min(time, 1.0f);
		
        this.CheckConstant();
        float t2 = time * time;
        float t3 = time * time * time;
        float x = this.Ax * t3 + this.Bx * t2 + this.Cx * time + startPoint.x;
        float y = this.Ay * t3 + this.By * t2 + this.Cy * time + startPoint.y;
        float z = this.Az * t3 + this.Bz * t2 + this.Cz * time + startPoint.z;
		// Don't modify values if start point, end point & control points lie on the same plane
		if(this.startPoint.x == this.endPoint.x && this.startPoint.x == this.controlPoint1.x && this.startPoint.x == this.controlPoint2.x)
			x = this.startPoint.x;
		if(this.startPoint.y == this.endPoint.y && this.startPoint.y == this.controlPoint1.y && this.startPoint.y == this.controlPoint2.y)
			y = this.startPoint.y;
		if(this.startPoint.z == this.endPoint.z && this.startPoint.z == this.controlPoint1.z && this.startPoint.z == this.controlPoint2.z)
			z = this.startPoint.z;
        return new Vector3( x, y, z );
    } 

    private void SetConstant()
    {
        this.Cx = 3f * ( ( this.startPoint.x + this.controlPoint1.x ) - this.startPoint.x );
        this.Bx = 3f * ( ( this.endPoint.x + this.controlPoint2.x ) - ( this.startPoint.x + this.controlPoint1.x ) ) - this.Cx;
        this.Ax = this.endPoint.x - this.startPoint.x - this.Cx - this.Bx; 

        this.Cy = 3f * ( ( this.startPoint.y + this.controlPoint1.y ) - this.startPoint.y );
        this.By = 3f * ( ( this.endPoint.y + this.controlPoint2.y ) - ( this.startPoint.y + this.controlPoint1.y ) ) - this.Cy;
        this.Ay = this.endPoint.y - this.startPoint.y - this.Cy - this.By; 

        this.Cz = 3f * ( ( this.startPoint.z + this.controlPoint1.z ) - this.startPoint.z );
        this.Bz = 3f * ( ( this.endPoint.z + this.controlPoint2.z ) - ( this.startPoint.z + this.controlPoint1.z ) ) - this.Cz;
        this.Az = this.endPoint.z - this.startPoint.z - this.Cz - this.Bz;
    } 

    // Check if p0, p1, p2 or p3 have changed
    private void CheckConstant()
    {
        if( this.startPoint != this.b0 || this.controlPoint1 != this.b1 || this.controlPoint2 != this.b2 || this.endPoint != this.b3 )
        {
            this.SetConstant();
            this.b0 = this.startPoint;
            this.b1 = this.controlPoint1;
            this.b2 = this.controlPoint2;
            this.b3 = this.endPoint;
        }
    }
}
