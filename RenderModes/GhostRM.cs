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
		
		ghostMesh = new Mesh("213", maxParticles, PrimitiveType.Points);
		lineMesh = new Mesh("2", 2 * maxParticles, PrimitiveType.Lines);
		
		priorityRequester = false;
	}
	
	override protected void doUpdateView(Matrix4 m){
		ghostShader.setMatrix4("view", m);
		lineShader.setMatrix4("view", m);
		ghostShader.setFloat("zoom", ren.cam.zoom);
	}
	
	override protected void doUpdateProj(){
		ghostShader.setMatrix4("projection", ren.projection);
		lineShader.setMatrix4("projection", ren.projection);
	}
	
	override protected void doDraw(){
		if(ren.ghost == null || ren.ghost.Count == 0){
			return;
		}
		
		if(ren.ghostLocked){
			ghostShader.use();
			
			List<float> f = new List<float>();
			
			foreach(Particle p in ren.ghost){
				f.AddRange(p.drawData());
			}
			
			ghostMesh.addDynamicData(f.ToArray());
			ghostMesh.draw();
			
			lineShader.use();
			lineShader.setVector3("color", orange);
			
			f.Clear();
			foreach(Particle p in ren.ghost){
				f.AddRange(new float[]{(float) p.position.X, (float) p.position.Y, (float) (p.position.X + (ren.ghostCenter.X - ren.cam.mouseWorldPos.X)), (float) (p.position.Y + (ren.ghostCenter.Y - ren.cam.mouseWorldPos.Y))});
			}
			
			lineMesh.addDynamicData(f.ToArray());
			lineMesh.draw();
		}else{
			ghostShader.use();
			
			ren.ghostCenter = ren.cam.mouseWorldPos;
			
			List<float> f = new List<float>();
			
			foreach(Particle p in ren.ghost){
				f.AddRange(new float[]{(float) (ren.ghostCenter.X + p.position.X), (float) (ren.ghostCenter.Y + p.position.Y), (float) p.radius, p.color.R/255f, p.color.G/255f, p.color.B/255f});
			}
			
			ghostMesh.addDynamicData(f.ToArray());
			
			ghostMesh.draw();
		}
		
	}
}