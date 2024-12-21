using System;
using OpenTK;
using OpenTK.Mathematics;

abstract class Button{
	public AABB box{get; protected set;}
	
	public Action? action;
	
	protected Renderer ren;
	
	public bool active;
	
	public Button(Renderer r){
		ren = r;
		active = true;
	}
	
	public Button setAction(Action? a){
		action = a;
		return this;
	}
	
	abstract public void draw(Vector2d m);
	
	abstract public void updateBox();
}