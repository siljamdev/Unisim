using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class Camera{
	
	Renderer ren;
	
	const float cameraSpeed = 200f;
	const float smoothZoomTime = 0.2f;
	
	public Vector2d position;
	public Vector2d mouseLastPos{get; private set;}
	
	Vector2d movement;
	
	public Vector2d mouseWorldPos{get{
		Vector2d m = mouseLastPos;
		m.X -= ren.width/2f;
		
		m.Y -= ren.height/2f;
		m.Y = -m.Y;
		
		m /= zoom;
		m -= position;
		return m;
	}}
	
	public Particle follow{get; private set;}
	
	public float zoom{get; private set;}
	
	public int zoomFactor{get; private set;}
	
	public Matrix4 view{get; private set;}
	
	Stopwatch sw;
	float startZoom;
	float targetZoom;
	
	public Camera(Renderer r){
		ren = r;
		position = new Vector2d(0.0d, 0.0d);
		targetZoom = 1.0f;
		zoom = 1.0f;
		updateMatrix();
		notifyRenderer();
	}
	
	void updateMatrix(){
		if(follow != null){
			position = -follow.position;
		}
		view = Matrix4.CreateTranslation(new Vector3((float) position.X, (float) position.Y, 0.0f)) * Matrix4.CreateScale(zoom);
		
		notifyRenderer();
	}
	
	void notifyRenderer(){
		ren.updateView(view);
	}
	
	public void setFollow(Particle p){
		if(follow == null && p == null){
			return;
		}
		
		follow = p;
		
		if(follow == null){
			ren.setCornerInfo("Camera free");
			ren.mainScreen.buttons[0].active = false;
		}else{
			if(follow.name != null){
				ren.setCornerInfo("Camera following " + follow.name);
				((ImageButton)ren.mainScreen.buttons[0]).setDescription("Following " + follow.name);
			}else{
				ren.setCornerInfo("Camera following particle");
				((ImageButton)ren.mainScreen.buttons[0]).setDescription("Following particle");
			}
			ren.mainScreen.buttons[0].active = true;
		}
	}
	
	public void startFrame(){
		if(sw != null){
			float elapsedSeconds = (float) sw.Elapsed.TotalSeconds;
			if(elapsedSeconds > smoothZoomTime){
				sw = null;
				zoom = targetZoom;
				updateMatrix();
			}else{
				zoom = startZoom + (elapsedSeconds / smoothZoomTime) * (targetZoom - startZoom);
				updateMatrix();
			}
		}
		
		if(follow != null){
			updateMatrix();
		}
	}
	
	public void moveUp(float q){
		movement.Y -= cameraSpeed * q / zoom;
	}
	
	public void moveDown(float q){
		movement.Y += cameraSpeed * q / zoom;
	}
	
	public void moveLeft(float q){
		movement.X += cameraSpeed * q / zoom;
	}
	
	public void moveRight(float q){
		movement.X -= cameraSpeed * q / zoom;
	}
	
	public void endFrame(){
		if(movement != Vector2d.Zero){
			position += movement;
			movement *= 0.8d;
			
			if(movement.Length < 0.001d){
				movement = Vector2d.Zero;
			}
			
			updateMatrix();
			notifyRenderer();
		}
	}
	
	public void scroll(float q){
		if(q < 0){
			zoomFactor--;
			
			startZoom = zoom;
			targetZoom /= 1.2f;
		}else{
			zoomFactor++;
			
			startZoom = zoom;
			targetZoom *= 1.2f;
		}
		sw = new Stopwatch();
		sw.Start();
	}
	
	public void mouse(float x, float y) {
		mouseLastPos = new Vector2d(x, y);
	}
}