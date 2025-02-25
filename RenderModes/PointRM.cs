using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class PointRenderMode : RenderMode{
	
	Shader shader;
	Mesh mesh;
	
	public PointRenderMode(Renderer r, Simulation s, Mesh m) : base(r, s){
		shader = Shader.generateFromAssembly("point");
		shader.setFloat("pointSize", 3.0f);
		
		mesh = m;
		
		db = dbc.particles;
		priorityRequester = false;
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setMatrix4("view", m);
	}
	
	override protected void doUpdateProj(){
		shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		shader.use();
		
		mesh.draw();
	}
	
	public void setPointSize(float f){
		shader.setFloat("pointSize", f);
	}
}