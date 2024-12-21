using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using AshLib;

class GhostRenderMode : RenderMode{
	
	Shader ghostShader;
	Shader lineShader;
	public Mesh ghostMesh;
	public Mesh lineMesh;
	
	static readonly Color3 orange = new Color3(230, 120, 0);
	
	public GhostRenderMode(Renderer r, Simulation s, Shader h) : base(r, s){
		lineShader = h;
		ghostShader = Shader.generateFromAssembly("ghost");
		
		float[] vertices = { //Just the full screen will be
			-1f, -1f,
			-1f, 1f,
			1f, -1f,
			1f,  1f,
			1f, -1f,
			-1f, 1f
		};
		
		ghostMesh = new Mesh("2", vertices, PrimitiveType.Triangles);
		lineMesh = new Mesh("2", 2, PrimitiveType.Lines);
		
		priorityRequester = false;
	}
	
	override protected void doUpdateView(Matrix4 m){
		ghostShader.setMatrix4("view", m);
		lineShader.setMatrix4("view", m);
	}
	
	override protected void doUpdateProj(){
		ghostShader.setMatrix4("projection", ren.projection);
		lineShader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		if(ren.ghost == null){
			return;
		}
		
		if(ren.ghostLocked){
			ghostShader.use();
			
			ghostShader.setVector2("pos", (Vector2) ren.ghost.position);
			ghostShader.setFloat("rad", (float) ren.ghost.radius);
			ghostShader.setVector3("col", ren.ghost.color);
			ghostMesh.draw();
			
			lineShader.use();
			lineShader.setVector3("color", orange);
			
			lineMesh.addDynamicData(new float[]{(float) ren.ghost.position.X, (float) ren.ghost.position.Y, (float) (2.0 * ren.ghost.position.X - ren.cam.mouseWorldPos.X), (float) (2.0 * ren.ghost.position.Y - ren.cam.mouseWorldPos.Y)});
			lineMesh.draw();
		}else{
			ghostShader.use();
			
			ghostShader.setVector2("pos", (Vector2) ren.cam.mouseWorldPos);
			ghostShader.setFloat("rad", (float) ren.ghost.radius);
			ghostShader.setVector3("col", ren.ghost.color);
			ghostMesh.draw();
		}
		
	}
}