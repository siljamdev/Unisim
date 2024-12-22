using System;
using OpenTK;
using OpenTK.Mathematics;

abstract class Button{
	public AABB box{get; protected set;}
	
	public Action? action;
	public Action? quickAction;
	
	public bool active;
	
	public bool hasHover;
	
	public Button(){
		active = true;
	}
	
	public Button setAction(Action? a){
		action = a;
		return this;
	}
	
	public Button setQuickAction(Action? a){
		quickAction = a;
		return this;
	}
	
	abstract public void draw(Renderer ren, Vector2d m);
	
	abstract public void drawHover(Renderer ren, Vector2d m);
	
	abstract public void updateBox(Renderer ren);
}