using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class BoxesRenderMode : RenderMode{
	
	Shader shader;
	Mesh mesh;
	
	static readonly Vector3 boxRed = new Vector3(230f/255f, 0f, 0f);
	
	public BoxesRenderMode(Renderer r, Simulation s) : base(r, s){
		shader = Shader.generateFromAssembly("AABB");
		shader.setVector3("color", boxRed);
		
		mesh = new Mesh("22", maxParticles, PrimitiveType.Points);
		
		db = dbc.boxes;
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setMatrix4("view", m);
		shader.setFloat("zoom", ren.cam.zoom);
	}
	
	override protected void doUpdateProj(){
		shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		float[] f = dbc.boxes.getData();
		
		if(f == null){
			return;
		}
		
		shader.use();
		
		
		mesh.addDynamicData(f);
		mesh.draw();
	}
}