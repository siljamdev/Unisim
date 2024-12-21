using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using AshLib;

class CollisionRenderMode : RenderMode{
	
	Shader shader;
	Mesh mesh;
	
	static readonly Color3 orange = new Color3(230, 120, 0);
	
	public CollisionRenderMode(Renderer r, Simulation s) : base(r, s){
		shader = Shader.generateFromAssembly("collision");
		shader.setVector3("color", orange);
		
		mesh = new Mesh("2", 6 * maxParticles, PrimitiveType.Points);
		
		db = dbc.collisions;
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setMatrix4("view", m);
	}
	
	override protected void doUpdateProj(){
		shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		float[] f = dbc.collisions.getData();
		
		if(f == null){
			return;
		}
		
		shader.use();
		
		
		mesh.addDynamicData(f);
		mesh.draw();
	}
}