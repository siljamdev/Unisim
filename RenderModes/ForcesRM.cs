using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class ForcesRenderMode : RenderMode{
	
	Shader shader;
	Mesh mesh;
	
	static readonly Vector3 forceGreen = new Vector3(0f, 0.82f, 0.23f);
	
	public ForcesRenderMode(Renderer r, Simulation s, Shader h) : base(r, s){
		shader = h;
		
		mesh = new Mesh("2", 5 * (maxParticles - 1) * maxParticles, PrimitiveType.Lines);
		
		db = dbc.forces;
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setMatrix4("view", m);
	}
	
	override protected void doUpdateProj(){
		shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		float[] f = dbc.forces.getData();
		
		if(f == null){
			return;
		}
		
		shader.use();
		shader.setVector3("color", forceGreen);
		
		mesh.addDynamicData(f);
		mesh.draw();
	}
}