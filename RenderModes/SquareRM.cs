using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using AshLib;

class SquareRenderMode : RenderMode{
	
	Shader shader;
	Mesh mesh;
	
	Vector2d startPos;
	Vector2d endPos;
	
	bool lockedEnd;
	
	static readonly Color3 white = Renderer.textColor;
	
	public SquareRenderMode(Renderer r, Simulation s) : base(r, s){
		shader = Shader.generateFromAssembly("square");
		
		float[] vertices = {
			0f, 0f
		};
		
		mesh = new Mesh("2", vertices, PrimitiveType.Points);
		
		priorityRequester = false;
		
		shader.setVector2("startPos", new Vector2(0f, 0f));
		shader.setVector2("endPosU", new Vector2(10f, 10f));
		shader.setVector3("col", white);
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setMatrix4("view", m);
	}
	
	override protected void doUpdateProj(){
		shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		shader.use();
		
		if(!lockedEnd){
			shader.setVector2("endPosU", (Vector2) ren.cam.mouseWorldPos);
		}
		
		mesh.draw();
	}
	
	public void lockSelection(Vector2d s){
		startPos = s;
		shader.setVector2("startPos", (Vector2) s);
		lockedEnd = false;
	}
	
	public void lockEnd(Vector2d s){
		endPos = s;
		shader.setVector2("endPosU", (Vector2) s);
		lockedEnd = true;
	}
	
	public AABB getSelection(){
		return new AABB(startPos, endPos);
	}
}