using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using AshLib;

class Renderer{
	
	public int width{get; private set;}
	public int height{get; private set;}
	
	//readonly Color4 backgroundColor = new Color4(0.1f, 0.1f, 0.3f, 1.0f); //og color
	static readonly Color4 backgroundColor = new Color4(0.03f, 0.03f, 0.1f, 1.0f);
	public static readonly Vector2 textSize = new Vector2(15f, 18f);
	
	public const float separation = 40f;
	public const float fieldSeparation = 30f;
	
	public static readonly Color3 textColor = new Color3("EFEFEF");
	public static readonly Color3 selectedTextColor = new Color3("FFFF8F");
	public static readonly Color3 titleTextColor = new Color3("DFDFFF");
	public static readonly Color3 fieldTextColor = new Color3("D6D6D6");
	public static readonly Color3 buttonColor = new Color3("555577");
	public static readonly Color3 greenButtonColor = new Color3("557755");
	public static readonly Color3 redButtonColor = new Color3("775555");
	public static readonly Color3 redTextColor = new Color3("FF8888");
	public static readonly Color3 black = new Color3("000000");
	public static readonly Color3 fieldColor = new Color3("222222");
	public static readonly Color3 fieldSelectedColor = new Color3("505050");
	
	public Camera cam{get; private set;}
	public UIRenderer ui{get; private set;}
	public FontRenderer fr{get; private set;}
	
	Simulation sim;
	Simulator sor;
	
	Stopwatch sw;
	string corner;
	
	public Matrix4 projection{get; private set;}
	
	public List<RenderMode> modes{get; private set;}
	
	public DrawBufferController dbc;
	
	bool advancedMode;
	
	public Particle ghost;
	public bool ghostLocked;
	
	public Screen mainScreen;
	public Screen currentScreen;
	
	public Renderer(Simulator st, Simulation s, DrawBufferController d){
		sim = s;
		sor = st;
		sw = new Stopwatch();
		dbc = d;
		
		width = 640;
		height = 480;
		
		//Modes
		
		modes = new List<RenderMode>();
		
		modes.Add(new BackgroundRenderMode(this, sim)); //0
		
		ParticleRenderMode p = new ParticleRenderMode(this, sim);
		modes.Add(p); //1
		modes.Add(new PointRenderMode(this, sim, p.mesh)); //2
		
		Shader lineShader = Shader.generateFromAssembly("line");
		modes.Add(new ForcesRenderMode(this, sim, lineShader)); //3
		modes.Add(new VelocitiesRenderMode(this, sim, lineShader)); //4
		modes.Add(new BoxesRenderMode(this, sim)); //5
		modes.Add(new CollisionRenderMode(this, sim)); //6
		modes.Add(new GhostRenderMode(this, sim, lineShader)); //7
		
		//other utilities
		
		cam = new Camera(this);
		
		float[] vertices = { //y is in -1 so starting pos of the text is in the left upper corner
			1f, -1f,
			1f, 0f,
			0f, -1f,
			1f, 0f,
			0f, 0f,
			0f, -1f,
		};
		
		Mesh uiMesh = new Mesh("2", vertices, PrimitiveType.Triangles);
		
		ui = new UIRenderer(uiMesh);
		fr = new FontRenderer(uiMesh, Texture2D.generateFromAssembly("font.png", TextureParams.Default), 16, 16);
		
		//load textures
		ui.addTexture("play", Texture2D.generateFromAssembly("playButton.png", TextureParams.Default));
		ui.addTexture("stop", Texture2D.generateFromAssembly("stopButton.png", TextureParams.Default));
		ui.addTexture("camera", Texture2D.generateFromAssembly("cameraButton.png", TextureParams.Default));
		ui.addTexture("pencil", Texture2D.generateFromAssembly("editButton.png", TextureParams.Default));
		ui.addTexture("add", Texture2D.generateFromAssembly("addButton.png", TextureParams.Default));
		ui.addTexture("up", Texture2D.generateFromAssembly("increaseButton.png", TextureParams.Default));
		ui.addTexture("down", Texture2D.generateFromAssembly("downButton.png", TextureParams.Default));
		ui.addTexture("tick", Texture2D.generateFromAssembly("tick.png", TextureParams.Default));
		
		//Activate modes
		modes[0].toggleActivation();
		modes[1].toggleActivation();
		modes[2].toggleActivation();
		modes[7].toggleActivation();
		
		//For points
		GL.Enable(EnableCap.ProgramPointSize);
		
		//Enable transparency (blending)
		GL.Enable(EnableCap.Blend);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		
		mainScreen = new Screen(new ImageButton("camera", -1, -1, 0f, 0f, 30f, 30f, textColor).setAction(camFree),
								new ImageButton("stop", 1, -1, 0f, 0f, 30f, 30f, new Color3("FFBBBB")).setAction(sor.togglePause),
								new ImageButton("add", -1, -1, 30f, 0f, 30f, 30f, textColor).setDescription("Add new particle").setAction(sor.setAddParticleScreen).setQuickAction(sor.addParticle),
								new ImageButton("up", 1, -1, 30f, 0f, 30f, 30f, textColor).setDescription("+").setAction(sor.tickRateUp),
								new ImageButton("down", 1, -1, 60f, 0f, 30f, 30f, textColor).setDescription("-").setAction(sor.tickRateDown));
		
		mainScreen.buttons[0].active = false;
		
		mainScreen.updateProj(this);
	}
	
	void camFree(){
		cam.setFollow(null);
	}
	
	public void setScreen(Screen s){
		currentScreen = s;
		if(currentScreen != null){
			currentScreen.updateProj(this);
		}
	}
	
	public void setCornerInfo(string s){
		corner = s;
		sw.Restart();
	}
	
	public void updateSize(int w, int h){
		width = w;
		height = h;
		projection = Matrix4.CreateOrthographic((float) width, (float) height, -1.0f, 1.0f);
		
		foreach(RenderMode rm in modes){
			rm.updateProj();
		}
		
		ui.setProjection(projection);
		fr.setProjection(projection);
		
		mainScreen.updateProj(this);
		
		if(currentScreen != null){
			currentScreen.updateProj(this);
		}
	}
	
	public void updateView(Matrix4 m){
		foreach(RenderMode rm in modes){
			rm.updateView(m);
		}
	}
	
	public void toggleAdvancedMode(){
		advancedMode = !advancedMode;
		
		if(advancedMode){
			setCornerInfo("Advanced mode enabled");
		}else{
			setCornerInfo("Advanced mode diabled");
		}
	}
	
	public void draw(){
		GL.ClearColor(backgroundColor);
		GL.Clear(ClearBufferMask.ColorBufferBit);
		
		cam.startFrame();
		
		foreach(RenderMode rm in modes){
			rm.draw();
		}
		
		if(advancedMode){
			string s = "FPS: " + Simulator.dh.stableFps.ToString("F0");
			fr.drawText(s, width/2f - s.Length * textSize.X, (height/2f), textSize, textColor);
			
			int decimals = cam.zoomFactor <= 0 ? 0 : Math.Clamp((int)(cam.zoomFactor / 10), 0, 3);
			string format = "F" + decimals;
			
			s = "(" + (-cam.position.X).ToString(format) + ", " + (-cam.position.Y).ToString(format) + "\\" + cam.zoomFactor.ToString() + ")";
			fr.drawText(s, width/2f - s.Length * textSize.X, (height/2f) - textSize.Y, textSize, textColor);
			
			s = "Cursor: " + cam.mouseWorldPos.ToString(format);
			fr.drawText(s, width/2f - s.Length * textSize.X, (height/2f) - 2f * textSize.Y, textSize, textColor);
			
			if(cam.follow != null){
				s = "Position: " + cam.follow.position.ToString(format);
				fr.drawText(s, -width/2f, -height/2f + 30f + textSize.Y, textSize, textColor);
				
				s = "Radius: " + cam.follow.radius.ToString("F1");
				fr.drawText(s, -width/2f, -height/2f + 30f + 2f * textSize.Y, textSize, textColor);
				
				s = "Mass: " + cam.follow.mass.ToString("F1");
				fr.drawText(s, -width/2f, -height/2f + 30f + 3f * textSize.Y, textSize, textColor);
			}
		}
		
		if(sim.isRunning){
			fr.drawText("TPS: " + sim.th.stableFps.ToString("F0"), -width/2f, (height/2f), textSize, textColor);
		}
		
		fr.drawText("Tick " + sim.tickCounter.ToString(), -width/2f, height/2f - textSize.Y, textSize, textColor);
		
		if(corner != null){
			if(sw.Elapsed.TotalSeconds < 2d){
				fr.drawText(corner, -width/2f, (height/2f) - 2f * textSize.Y, textSize, textColor);
			}else if(sw.Elapsed.TotalSeconds < 3d){
				fr.drawText(corner, -width/2f, (height/2f) - 2f * textSize.Y, textSize, textColor, (float) (3d - sw.Elapsed.TotalSeconds));
			}else{
				corner = null;
				sw.Stop();
			}
		}
		
		if(currentScreen != null){
			mainScreen.draw(this, false);
			
			ui.drawRect(-width/2f, height/2f, width, height, black, 0.6f);
			currentScreen.draw(this, true);
		}else{
			mainScreen.draw(this, true);
		}
	}
}