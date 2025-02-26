using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class WorldBorder{
	public Vector2d size{get; private set;}
	public AABB box{get; private set;}
	
	public WorldBorder(Vector2d s){
		size = s;
		box = new AABB(new Vector2d(-size.X, -size.Y), new Vector2d(size.X, size.Y));
	}
}