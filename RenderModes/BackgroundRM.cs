using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class BackgroundRenderMode : RenderMode{
	
	Shader shader;
	Mesh mesh;
	Texture2D perlin;
	
	public BackgroundRenderMode(Renderer r, Simulation s) : base(r, s){
		shader = Shader.generateFromAssembly("clouds");
		
		float[] vertices = { //Just the full screen will be
			-1f, -1f,
			-1f, 1f,
			1f, -1f,
			1f,  1f,
			1f, -1f,
			-1f, 1f
		};
		
		mesh = new Mesh("2", vertices, PrimitiveType.Triangles);
		
		perlin = Texture2D.generateFromAssembly("noise.png", TextureParams.Noise);
		
		priorityRequester = false;
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setFloat("zoom", ren.cam.zoom);
		shader.setVector2("pos", (Vector2) ren.cam.position);
	}
	
	override protected void doUpdateProj(){
		shader.setVector2("resolution", new Vector2(ren.width, ren.height));
		shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){		
		shader.use();
		shader.setFloat("iTime", (float) Simulator.dh.GetTime());
		
		perlin.bind();
		
		mesh.draw();
	}
}