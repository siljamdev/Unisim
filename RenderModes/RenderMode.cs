using System;
using OpenTK;
using OpenTK.Mathematics;

abstract class RenderMode{
	
	protected Renderer ren;
	protected Simulation sim;
	
	protected DrawBufferController dbc;
	protected DrawBuffer db;
	protected bool priorityRequester;
	
	public bool active{get; private set;}
	
	protected const int maxParticles = 1000;
	
	public RenderMode(Renderer r, Simulation s){
		ren = r;
		sim = s;
		
		dbc = ren.dbc;
		
		priorityRequester = true;
	}
	
	public void toggleActivation(){
		active = !active;
		
		update();
	}
	
	public void draw(){
		if(active){
			doDraw();
		}
	}
	
	void update(){
		if(active){
			doUpdateView(ren.cam.view);
			doUpdateProj();
			
			if(priorityRequester){
				db.required = true;
			}
		}else if(priorityRequester){
			db.required = false;
		}
		
		onActivation();
	}
	
	public void updateView(Matrix4 m){
		if(active){
			doUpdateView(m);
		}
	}
	
	public void updateProj(){
		if(active){
			doUpdateProj();
		}
	}
	
	virtual protected void onActivation(){
		
	}
	
	abstract protected void doUpdateView(Matrix4 m);
	
	abstract protected void doUpdateProj();
	
	abstract protected void doDraw();
}