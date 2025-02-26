using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using AshLib;

class BorderRenderMode : RenderMode{
	
	Shader shader;
	Mesh mesh;
	Texture2D perlin;
	
	Vector2d size;
	
	static readonly Color3 defColor = new Color3(117, 215, 255);
	
	public BorderRenderMode(Renderer r, Simulation s) : base(r, s){
		shader = Shader.generateFromAssembly("border");
		shader.setVector3("color", defColor);
		
		float[] vertices = { //Just the full screen will be
			-1f, -1f,
			-1f, 1f,
			1f, -1f,
			1f,  1f,
			1f, -1f,
			-1f, 1f
		};
		
		mesh = new Mesh("2", vertices, PrimitiveType.Triangles);
		
		priorityRequester = false;
	}
	
	override protected void doUpdateView(Matrix4 m){
		shader.setFloat("zoom", ren.cam.zoom);
		shader.setVector2("pos", (Vector2) ren.cam.position);
	}
	
	override protected void doUpdateProj(){
		shader.setVector2("resolution", new Vector2(ren.width, ren.height));
		//shader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		if(sim.wb == null){
			return;
		}
		
		shader.use();
		
		if(sim.wb.size != size){
			size = sim.wb.size;
			shader.setVector2("size", (Vector2) size);
		}
		
		mesh.draw();
	}
	
	public void setColor(Color3 c){
		shader.setVector3("color", c);
	}
}