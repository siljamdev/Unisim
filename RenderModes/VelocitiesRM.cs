using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class VelocitiesRenderMode : RenderMode{
	
	Shader shader;
	Mesh mesh;
	
	static readonly Vector3 yellow = new Vector3(1f, 1f, 0f);
	
	public VelocitiesRenderMode(Renderer r, Simulation s, Shader h) : base(r, s){
		shader = h;
		
		mesh = new Mesh("2", maxParticles * maxParticles, PrimitiveType.Lines);
		
		db = dbc.velocities;
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setMatrix4("view", m);
	}
	
	override protected void doUpdateProj(){
		shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){		
		float[] f = dbc.velocities.getData();
		
		if(f == null){
			return;
		}
		
		shader.use();
		shader.setVector3("color", yellow);
		
		mesh.addDynamicData(f);
		mesh.draw();
	}
}