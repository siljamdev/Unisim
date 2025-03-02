using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class WorldBorder{
	public Vector2d size{get; private set;}
	public AABB box{get; private set;}
	
	public WorldBorder(Vector2d s){
		size = s;
		box = new AABB(-s, s);
	}
	
	public WorldBorder(double x, double y){
		size = new Vector2d(x, y);
		box = new AABB(-size, size);
	}
	
	public bool contains(AABB a){
		return AABB.contained(a, box);
	}
	
	public bool contains(Vector2d a){
		return AABB.contained(a, box);
	}
}