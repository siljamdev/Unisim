using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class ParticleRenderMode : RenderMode{
	
	Shader shader;
	public Mesh mesh{get; private set;}
	
	public ParticleRenderMode(Renderer r, Simulation s) : base(r, s){
		shader = Shader.generateFromAssembly("particle");
		
		mesh = new Mesh("213", maxParticles, PrimitiveType.Points);
		
		db = dbc.particles;
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setMatrix4("view", m);
		shader.setFloat("zoom", ren.cam.zoom);
	}
	
	override protected void doUpdateProj(){
		shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		float[] f = dbc.particles.getData();
		
		if(f == null){
			return;
		}
		
		shader.use();
		
		mesh.addDynamicData(f);
		mesh.draw();
	}
}