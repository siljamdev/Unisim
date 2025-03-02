using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common.Input;
using StbImageSharp;
using AshLib;
using AshLib.Time;
using AshLib.Folders;
using AshLib.AshFiles;

#if WINDOWS
	using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
#endif

partial class Simulator : GameWindow{
	
	KeyBind escape = new KeyBind(Keys.Escape, Keys.LeftShift, false);
	
	KeyBind fullscreen = new KeyBind(Keys.F11, false);
	KeyBind screenshot = new KeyBind(Keys.F2, false);
	
	KeyBind advancedMode = new KeyBind(Keys.LeftAlt, false);
	
	KeyBind showForces = new KeyBind(Keys.F3, false);
	KeyBind showPoints = new KeyBind(Keys.F5, false);
	KeyBind showBoxes = new KeyBind(Keys.F4, false);
	
	KeyBind tickForward = new KeyBind(Keys.F, false);
	KeyBind pause = new KeyBind(Keys.Space, false);
	
	KeyBind tickRateUpKey = new KeyBind(Keys.KeyPadAdd, false);
	KeyBind tickRateDownKey = new KeyBind(Keys.KeyPadSubtract, false);
	KeyBind runAtMax = new KeyBind(Keys.M, false);
	
	KeyBind nextParticle = new KeyBind(Keys.Tab, Keys.LeftShift, false);
	
	KeyBind quickAdd = new KeyBind(Keys.E, false);
	KeyBind remove = new KeyBind(Keys.R, false);
	KeyBind duplicate = new KeyBind(Keys.V, false);
	
	KeyBind startSelection = new KeyBind(Keys.L, false);
	
	KeyBind moveUp = new KeyBind(Keys.W, true);
	KeyBind moveDown = new KeyBind(Keys.S, true);
	KeyBind moveLeft = new KeyBind(Keys.A, true);
	KeyBind moveRight = new KeyBind(Keys.D, true);
	
	KeyBind del = new KeyBind(Keys.Backspace, false);
	
	KeyBind logUp = new KeyBind(Keys.Up, true);
	KeyBind logDown = new KeyBind(Keys.Down, true);
	
	public Dependencies dep;
	
	bool waitingForSelection;
	bool selectionLocked;
	public bool selectionDisplaying;
	
	bool setSelectionNextTick;
	
	bool takeScreenshotNextTick;
	
	Renderer ren;
	Simulation sim;
	
	List<Particle> ghost;
	
	Scene? sceneToLoad;
	
	public static DeltaHelper dh;
	
	bool isFullscreened;
	
	float maxFps = 144f;
	
	static void Main(string[] args){
		if(args.Length > 0){
			using(Simulator sim = new Simulator(removeQuotesSingle(args[0]))){
				sim.Run();
			}
		}else{
			using(Simulator sim = new Simulator()){
				sim.Run();
			}
		}
	}
	
	static string removeQuotesSingle(string p){
		p = p.Trim();
		
		if(p.Length < 1){
			return p;
		}
		char[] c = p.ToCharArray();
		if(c[0] == '\"' && c[c.Length - 1] == '\"'){
			if(c.Length < 2){
				return "";
			}
			return p.Substring(1, p.Length - 2);
		}
		return p;
	}
	
	Simulator(string p) : base(GameWindowSettings.Default, NativeWindowSettings.Default){
		CenterWindow(new Vector2i(640, 480));
		Title = "Unisim";
		
		FileDrop += onFileDropped;
		
		VSync = VSyncMode.On;
		
		if(File.Exists(p)){
			AshFile af = new AshFile(p);
			sceneToLoad = FileConverter.getScene(af);
		}
	}
	
	Simulator() : base(GameWindowSettings.Default, NativeWindowSettings.Default){
		CenterWindow(new Vector2i(640, 480));
		Title = "Unisim";
		
		FileDrop += onFileDropped;
		
		VSync = VSyncMode.On;
	}
	
	void initialize(){
		dh = new DeltaHelper();
		dh.Start();
		
		string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		dep = new Dependencies(appDataPath + "/ashproject/unisim", true, new string[]{"screenshots", "saves"}, null);
		
		setIcon();
		
		initializeConfigPart1();
		
		DrawBufferController dbc = new DrawBufferController();
		
		if(sceneToLoad != null){
			sim = new Simulation((Scene) sceneToLoad, dbc);
		}else{
			sim = new Simulation(Examples.RPF, dbc);
		}		
		
		ren = new Renderer(this, sim, dbc);
		
		initializeScreens();
		
		initializeConfigPart2();
		updateSceneConfigScreen();
		
		sim.tick();
	}
	
	void onResize(int x, int y){
		GL.Viewport(0, 0, x, y);
		if(this.ren != null){
			ren.updateSize(x, y);
		}
	}
	
	void initializeConfigPart1(){
		List<Keys> k = new List<Keys>{
			fullscreen.key,
			screenshot.key,
			advancedMode.key,
			showForces.key,
			showPoints.key,
			showBoxes.key,
			tickForward.key,
			pause.key,
			tickRateUpKey.key,
			tickRateDownKey.key,
			runAtMax.key,
			nextParticle.key,
			quickAdd.key,
			remove.key,
			duplicate.key,
			startSelection.key,
			moveUp.key,
			moveDown.key,
			moveLeft.key,
			moveRight.key
		};
		
		int[] ka = new int[k.Count];
		for(int i = 0; i < k.Count; i++){
			ka[i] = (int) k[i];
		}
		
		AshFileModel afm = new AshFileModel(new ModelInstance(ModelInstanceOperation.Type, "clouds", true),
		new ModelInstance(ModelInstanceOperation.Type, "dotSize", 3f),
		new ModelInstance(ModelInstanceOperation.Type, "bgColor", new Color3(8, 8, 26)),
		new ModelInstance(ModelInstanceOperation.Type, "maxParticles", 1000),
		new ModelInstance(ModelInstanceOperation.Type, "vsync", true),
		new ModelInstance(ModelInstanceOperation.Type, "maxFps", 144),
		new ModelInstance(ModelInstanceOperation.Type, "wbColor", new Color3(117, 215, 255)),
		new ModelInstance(ModelInstanceOperation.Type, "cloudsColor", new Color3(204, 204, 204)),
		new ModelInstance(ModelInstanceOperation.Type, "savePath", ""),
		new ModelInstance(ModelInstanceOperation.Type, "multithread", true),
		new ModelInstance(ModelInstanceOperation.Type, "colMultithread", false),
		new ModelInstance(ModelInstanceOperation.Type, "controls", ka));
		
		afm.deleteNotMentioned = true;
		
		dep.config *= afm;		
		dep.config.Save();
		
		Simulation.setMaxParticles(dep.config.GetCamp<int>("maxParticles"));
		Simulation.multiThreading = dep.config.GetCamp<bool>("multithread");
		Simulation.collisionsMultiThreading = dep.config.GetCamp<bool>("colMultithread");
	}
	
	void initializeConfigPart2(){		
		ren.setBgColor(dep.config.GetCamp<Color3>("bgColor"));
		if(dep.config.GetCamp<bool>("clouds") != ren.modes[0].active){
			ren.modes[0].toggleActivation();
		}
		((PointRenderMode) ren.modes[3]).setPointSize(dep.config.GetCamp<float>("dotSize"));
		setVsync(dep.config.GetCamp<bool>("vsync"));
		maxFps = dep.config.GetCamp<int>("maxFps");
		((BorderRenderMode) ren.modes[1]).setColor(dep.config.GetCamp<Color3>("wbColor"));
		((BackgroundRenderMode) ren.modes[0]).setColor(dep.config.GetCamp<Color3>("cloudsColor"));
		
		int[] ka = dep.config.GetCamp<int[]>("controls");
		if(ka.Length > 19){
			fullscreen.update((Keys)ka[0]);
			screenshot.update((Keys)ka[1]);
			advancedMode.update((Keys)ka[2]);
			showForces.update((Keys)ka[3]);
			showPoints.update((Keys)ka[4]);
			showBoxes.update((Keys)ka[5]);
			tickForward.update((Keys)ka[6]);
			pause.update((Keys)ka[7]);
			tickRateUpKey.update((Keys)ka[8]);
			tickRateDownKey.update((Keys)ka[9]);
			runAtMax.update((Keys)ka[10]);
			nextParticle.update((Keys)ka[11]);
			quickAdd.update((Keys)ka[12]);
			remove.update((Keys)ka[13]);
			duplicate.update((Keys)ka[14]);
			startSelection.update((Keys)ka[15]);
			moveUp.update((Keys)ka[16]);
			moveDown.update((Keys)ka[17]);
			moveLeft.update((Keys)ka[18]);
			moveRight.update((Keys)ka[19]);
		}
		
		((CheckButton) optionsScreen1.buttons[2]).on = dep.config.GetCamp<bool>("clouds");
		((Field) optionsScreen1.buttons[3]).text = dep.config.GetCamp<Color3>("cloudsColor").ToString().Substring(1);
		((Field) optionsScreen1.buttons[4]).text = dep.config.GetCamp<float>("dotSize").ToString();
		((Field) optionsScreen1.buttons[5]).text = dep.config.GetCamp<Color3>("bgColor").ToString().Substring(1);
		((CheckButton) optionsScreen1.buttons[8]).on = dep.config.GetCamp<bool>("vsync");
		((Field) optionsScreen1.buttons[9]).text = dep.config.GetCamp<int>("maxFps").ToString();
		((Field) optionsScreen1.buttons[6]).text = dep.config.GetCamp<Color3>("wbColor").ToString().Substring(1);
		
		((Field) optionsScreen2.buttons[1]).text = dep.config.GetCamp<int>("maxParticles").ToString();
		((Field) optionsScreen2.buttons[2]).text = dep.config.GetCamp<string>("savePath");
		((CheckButton) optionsScreen2.buttons[4]).on = dep.config.GetCamp<bool>("multithread");
		((CheckButton) optionsScreen2.buttons[6]).on = dep.config.GetCamp<bool>("colMultithread");
	}
	
	void updateSceneConfigScreen(){
		if(sim.sceneName != null){
			((Field) sceneConfigScreen.buttons[1]).text = sim.sceneName;
		}
		
		if(sim.wb != null){
			((CheckButton) sceneConfigScreen.buttons[3]).on = true;
			((Field) sceneConfigScreen.buttons[4]).text = sim.wb.size.X.ToString();
			((Field) sceneConfigScreen.buttons[5]).text = sim.wb.size.Y.ToString();
		}else{
			((CheckButton) sceneConfigScreen.buttons[3]).on = false;
			((Field) sceneConfigScreen.buttons[4]).text = "1000";
			((Field) sceneConfigScreen.buttons[5]).text = "1000";
		}
	}
	
	public void setVsync(bool b){
		if(b){
			VSync = VSyncMode.On;
		}else{
			VSync = VSyncMode.Off;
		}
	}
	
	public void setGhost(Particle p){
		ghost = new List<Particle>();
		ghost.Add(p);
		ren.ghostCenter = p.position;
		ren.ghost = ghost;
	}
	
	public void setGhost(List<Particle> p){
		ghost = p;
		ren.ghost = ghost;
	}
	
	public static Vector2d findMiddlePoint(List<Vector2d> points){
		if(points == null || points.Count == 0){
			return Vector2.Zero;
		}
		
		Vector2d sum = Vector2d.Zero;
		foreach(var point in points){
			sum += point;
		}
		
		return sum / points.Count;
	}
	
	void handleKeyboardInput(){
		// check to see if the window is focused
		if(!IsFocused){
			return;
		}
		
		switch(escape.isActiveMod(KeyboardState)){
			case 1:
			if(ren.currentScreen != null){
				ren.setScreen(null);
			}else{
				if(sim.isRunning){
					togglePause();
				}
				
				stopDisplayingSquare();
				ren.setScreen(pauseMenuScreen);
			}
			break;
			
			case 2:
			Close();
			break;
		}
		
		if(screenshot.isActive(KeyboardState)){
			captureScreenshot();
			ren.setCornerInfo("Saved screenshot");
		}
		
		if(fullscreen.isActive(KeyboardState)){
			toggleFullscreen();
		}
		
		if(ren.currentScreen != null){
			if(ren.currentScreen.writing && del.isActive(KeyboardState)){
				ren.currentScreen.tryDelChar();
				return;
			}
			
			if(ren.currentScreen.doScroll){
				if(logUp.isActive(KeyboardState)){
					ren.currentScreen.scroll(ren, 40f * (float) dh.deltaTime);
				}else if(logDown.isActive(KeyboardState)){
					ren.currentScreen.scroll(ren, -40f * (float) dh.deltaTime);
				}
			}
			return;
		}
		
		if(tickRateUpKey.isActive(KeyboardState)){
			tickRateUp();
		}
		
		if(tickRateDownKey.isActive(KeyboardState)){
			tickRateDown();
		}
		
		if(runAtMax.isActive(KeyboardState)){
			toggleMaxTicks();
		}
		
		if(tickForward.isActive(KeyboardState)){
			if(!sim.isRunning){
				Task.Run(() => {sim.doOneTick(); ren.setCornerInfo("Tick advanced");});
			}
		}
		
		if(pause.isActive(KeyboardState)){
			togglePause();
		}
		
		if(quickAdd.isActive(KeyboardState)){
			stopDisplayingSquare();
			addParticle();
		}
		
		if(remove.isActive(KeyboardState)){
			removeSelection();
			stopDisplayingSquare();
		}
		
		if(duplicate.isActive(KeyboardState)){
			duplicateSelection();
			stopDisplayingSquare();
		}
		
		if(startSelection.isActive(KeyboardState)){
			startSquareSelection();
		}
		
		if(advancedMode.isActive(KeyboardState)){
			ren.toggleAdvancedMode();
		}
		
		if(showForces.isActive(KeyboardState)){
			ren.modes[4].toggleActivation();
			ren.modes[5].toggleActivation();
			sim.tryGenerate();
			
			if(ren.modes[4].active){
				sim.changeForceMode = false;
				ren.setCornerInfo("Forces enabled");
			}else{
				sim.changeForceMode = false;
				ren.setCornerInfo("Forces disabled");
			}
		}
		
		if(showPoints.isActive(KeyboardState)){
			ren.modes[3].toggleActivation();
			
			if(ren.modes[3].active){
				ren.setCornerInfo("Points enabled");
			}else{
				ren.setCornerInfo("Points disabled");
			}
		}
		
		if(showBoxes.isActive(KeyboardState)){
			ren.modes[6].toggleActivation();
			ren.modes[7].toggleActivation();
			sim.tryGenerate();
			
			if(ren.modes[6].active){
				ren.setCornerInfo("Bounding boxes enabled");
			}else{
				ren.setCornerInfo("Bounding boxes disabled");
			}
		}
		
		switch(nextParticle.isActiveMod(KeyboardState)){
			case 1:
				followNext();
				break;
			case 2:
				followNone();
				break;
		}
		
		if(moveUp.isActive(KeyboardState)){
			ren.cam.moveUp((float) dh.deltaTime);
		}
		
		if(moveDown.isActive(KeyboardState)){
			ren.cam.moveDown((float) dh.deltaTime);
		}
		
		if(moveLeft.isActive(KeyboardState)){
			ren.cam.moveLeft((float) dh.deltaTime);
		}
		
		if(moveRight.isActive(KeyboardState)){
			ren.cam.moveRight((float) dh.deltaTime);
		}
		
		ren.cam.endFrame();
	}
	
	public void togglePause(){
		if(sim.isRunning){
			sim.stop();
			ren.setCornerInfo("Simulation paused");
			((ImageButton)ren.mainScreen.buttons[1]).textureName = "stop";
			((ImageButton)ren.mainScreen.buttons[1]).color = new Color3("FFBBBB");
		}else{
			Task.Run(() => sim.run());
			ren.setCornerInfo("Simulation running");
			((ImageButton)ren.mainScreen.buttons[1]).textureName = "play";
			((ImageButton)ren.mainScreen.buttons[1]).color = new Color3("BBBBFF");
		}
	}
	
	public void toggleMaxTicks(){
		sim.runAtMax = !sim.runAtMax;
		if(sim.runAtMax){
			ren.setCornerInfo("Running at max speed");
			((ImageButton)ren.mainScreen.buttons[5]).textureName = "max";
		}else{
			ren.setCornerInfo("Disabled max speed");
			((ImageButton)ren.mainScreen.buttons[5]).textureName = "nomax";
		}
	}
	
	public void removeSelection(){
		if(ren.cam.follow == null){
			//Console.WriteLine("B");
			if(selectionDisplaying){
				AABB sel = ((SquareRenderMode) ren.modes[9]).getSelection();
				List<Particle> par = sim.getParticlesInSelection(sel);
				sim.removeParticle(par);
				ren.setCornerInfo("Removed " + par.Count + " particles");
			}else if(ghost != null){
				setGhost((List<Particle>) null);
				ren.setCornerInfo("Removed ghost");
			}
		}else{
			Particle p = ren.cam.follow;
			sim.removeParticle(p);
			ren.cam.setFollow(null);
			ren.setCornerInfo("Removed " + (p.name == null ? "particle" : p.name));
		}
	}
	
	public void duplicateSelection(){	
		if(ren.cam.follow == null){
			if(selectionDisplaying){
				AABB sel = ((SquareRenderMode) ren.modes[9]).getSelection();
				List<Particle> par = sim.getParticlesInSelection(sel);
				
				List<Vector2d> c = new List<Vector2d>();
				foreach(Particle p in par){
					c.Add(p.position);
				}
				
				ren.ghostCenter = findMiddlePoint(c);
				
				List<Particle> p2 = new List<Particle>();
				foreach(Particle p in par){
					p2.Add(p.Clone().translate(p.position - ren.ghostCenter));
				}
				setGhost(p2);
				ren.setCornerInfo("Duplicated " + p2.Count + " particles");
			}
		}else{
			setGhost(ren.cam.follow.Clone());
			ren.setCornerInfo("Duplicated " + (ren.cam.follow.name == null ? "particle" : ren.cam.follow.name));
			ren.cam.setFollow(null);
		}
	}
	
	void stopDisplayingSquare(){
		if(selectionDisplaying){
			selectionDisplaying = false;
			ren.modes[9].toggleActivation();
			ren.mainScreen.buttons[6].active = false;
			ren.mainScreen.buttons[7].active = false;
		}
	}
	
	public void startSquareSelection(){
		setGhost((List<Particle>) null);
		stopDisplayingSquare();
		waitingForSelection = true;
		((ImageButton) ren.mainScreen.buttons[8]).color = Renderer.selectedTextColor;
	}
	
	public void squareNextTick(){
		setSelectionNextTick = true;
	}
	
	public void followNext(){
		Particle p = sim.getNextParticle(ren.cam.follow);
		ren.cam.setFollow(p);
	}
	
	public void followPrevious(){
		Particle p = sim.getPreviousParticle(ren.cam.follow);
		ren.cam.setFollow(p);
	}
	
	public void followNone(){
		ren.cam.setFollow(null);
	}
	
	public void tickRateUp(){
		sim.targetTPS *= 1.3f;
		ren.setCornerInfo(sim.targetTPS.ToString("F0"));
	}
	
	public void tickRateDown(){
		sim.targetTPS /= 1.3f;
		ren.setCornerInfo(sim.targetTPS.ToString("F0"));
	}
	
	void toggleFullscreen(){
		if(!isFullscreened){
			MonitorInfo mi = Monitors.GetMonitorFromWindow(this);
			WindowState = WindowState.Fullscreen;
			this.CurrentMonitor = mi;
			isFullscreened = true;
			VSync = VSyncMode.On;
		} else {
			WindowState = WindowState.Normal;
			isFullscreened = false;
			VSync = VSyncMode.On;
		}
	}
	
	public void checkErrors(){
		OpenTK.Graphics.OpenGL.ErrorCode errorCode = GL.GetError();
        while(errorCode != OpenTK.Graphics.OpenGL.ErrorCode.NoError){
            Console.WriteLine("OpenGL Error: " + errorCode);
			if(ren != null){
				ren.setCornerInfo("OpenGL Error: " + errorCode, Renderer.redTextColor);
			}
			
            errorCode = GL.GetError();
        }
	}
	
	void captureScreenshot(){
		int width = ren.width;
		int height = ren.height;
		
		// Create a byte array to hold the pixel data
		byte[] pixels = new byte[width * height * 3]; // RGBA (4 bytes per pixel)
		
		// Read pixels from OpenGL frame buffer
		GL.ReadBuffer(ReadBufferMode.Front);
		GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, pixels);
		
		// Create a new byte array to hold the RGB data (ignore alpha channel)
		byte[] rgbPixels = new byte[width * height * 3]; // RGB (3 bytes per pixel)
		
		// Copy only RGB values
		for(int i = 0; i < height; i++){
			for(int j = 0; j < width; j++){
				rgbPixels[((height - i - 1) * width + j) * 3] = pixels[(i * width + j) *3];      // Red
				rgbPixels[((height - i - 1) * width + j) * 3 + 1] = pixels[(i * width + j) * 3 + 1];  // Green
				rgbPixels[((height - i - 1) * width + j) * 3 + 2] = pixels[(i * width + j) * 3 + 2];  // Blue
			}
		}
		
		// Create a Bitmap and write the RGB pixel data into it
		using(Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb)){
			BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			System.Runtime.InteropServices.Marshal.Copy(rgbPixels, 0, data.Scan0, rgbPixels.Length);
			bitmap.UnlockBits(data);
	
			// Save the image (or copy to clipboard if needed)
			bitmap.Save(dep.path + "/screenshots/" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".png", ImageFormat.Png);
		}
	}
	
	void setIcon(){
		byte[] imageBytes = AssemblyFiles.get("res.icon.png");
		
		//Generate the image and put it as icon
		ImageResult image = ImageResult.FromMemory(imageBytes, ColorComponents.RedGreenBlueAlpha);
		if (image == null || image.Data == null){
			return;
		}
		
		OpenTK.Windowing.Common.Input.Image i = new OpenTK.Windowing.Common.Input.Image(image.Width, image.Height, image.Data);
		WindowIcon w = new WindowIcon(i);
		
		this.Icon = w;
	}
	
	void onFileDropped(FileDropEventArgs f){
		if(ren.currentScreen == dropScreen){
			if(f.FileNames.Length > 0){
				if(File.Exists(f.FileNames[0])){
					Task.Run(() => loadScene(f.FileNames[0]));
				}else{
					ren.setCornerInfo("The file was not found");
				}
			}
			closeCurrent();
		}
	}
	
	async Task loadScene(string p){
		ren.setCornerInfo("Loading scene...");
		AshFile af = new AshFile(p);
		setSimulation(FileConverter.getScene(af));
		ren.setCornerInfo("Scene loaded", Renderer.selectedTextColor);
	}
	
	protected override void OnKeyDown(KeyboardKeyEventArgs e){
		if(ren.currentScreen != null && ren.currentScreen.keySelecting && !e.IsRepeat && e.Key != Keys.Escape){
			ren.currentScreen.trySetKeybind(e.Key);
		}
	}
	
	protected override void OnTextInput(TextInputEventArgs e){
		if(ren.currentScreen != null && ren.currentScreen.writing){
			string s = e.AsString;
			switch(ren.currentScreen.tryGetWritingMode()){
				case WritingType.Hex:
				if(KeyBind.getHexTyping(s)){
					ren.currentScreen.tryAddStr(s);
				}
				break;
				
				case WritingType.Int:
				if(KeyBind.getIntTyping(s)){
					ren.currentScreen.tryAddStr(s);
				}
				break;
				
				case WritingType.Float:
				if(KeyBind.getFloatTyping(s)){
					ren.currentScreen.tryAddStr(s);
				}
				break;
				
				case WritingType.FloatPositive:
				if(KeyBind.getFloatPositiveTyping(s)){
					ren.currentScreen.tryAddStr(s);
				}
				break;
				
				case WritingType.String:
				ren.currentScreen.tryAddStr(s);
				break;
			}
		}
		
		base.OnTextInput(e);
	}
	
	protected override void OnLoad(){		
		initialize();
		base.OnLoad();
	}
	
	protected override void OnResize(ResizeEventArgs args){
		onResize(args.Width, args.Height);
		base.OnResize(args);
	}
	
	protected override void OnUpdateFrame(FrameEventArgs args){
		handleKeyboardInput();
		base.OnUpdateFrame(args);
	}
	
	protected override void OnRenderFrame(FrameEventArgs args){
		ren.draw();
		Context.SwapBuffers();
		checkErrors();
		if(takeScreenshotNextTick){
			captureScreenshot();
			ren.setCornerInfo("Saved screenshot");
			takeScreenshotNextTick = false;
		}
		if(setSelectionNextTick){
			setSelectionNextTick = false;
			startSquareSelection();
		}
		base.OnRenderFrame(args);
		dh.Frame();
		if(VSync != VSyncMode.On){
			dh.Target(maxFps);
		}
	}
	
	protected override void OnMouseWheel(MouseWheelEventArgs args){
		if(ren.currentScreen != null){
			if(ren.currentScreen.doScroll){
				ren.currentScreen.scroll(ren, args.OffsetY);
			}
			base.OnMouseWheel(args);
			return;
		}
		
		ren.cam.scroll(args.OffsetY);
        
		base.OnMouseWheel(args);
    }
	
	protected override void OnMouseMove(MouseMoveEventArgs e){
        ren.cam.mouse(e.X, e.Y);
		base.OnMouseMove(e);
    }
	
	protected override void OnMouseDown(MouseButtonEventArgs e){
        if(e.Button == MouseButton.Left){
			Screen s;
			if(ren.currentScreen == null){
				if(!ren.mainScreen.click(ren, ren.cam.mouseLastPos, KeyboardState.IsKeyDown(Keys.LeftShift))){
					Particle p = sim.getParticleAtCursor(ren.cam.mouseWorldPos);
					if(p != null){
						ren.cam.setFollow(p);
						waitingForSelection = false;
						((ImageButton) ren.mainScreen.buttons[8]).color = Renderer.textColor;
						stopDisplayingSquare();
					}else{
						if(waitingForSelection){
							((SquareRenderMode) ren.modes[9]).lockSelection(ren.cam.mouseWorldPos);
							ren.modes[9].toggleActivation();
							ren.cam.setFollow(null);
							waitingForSelection = false;
							((ImageButton) ren.mainScreen.buttons[8]).color = Renderer.textColor;
							selectionLocked = true;
						}
					}
				}else{
					waitingForSelection = false;
					((ImageButton) ren.mainScreen.buttons[8]).color = Renderer.textColor;
					stopDisplayingSquare();
				}
			}else{
				ren.currentScreen.click(ren, ren.cam.mouseLastPos, KeyboardState.IsKeyDown(Keys.LeftShift));
			}
        }else if(e.Button == MouseButton.Right){
			if(ghost != null && !ren.ghostLocked){
				foreach(Particle p in ghost){
					p.translate(ren.cam.mouseWorldPos);
				}
				
				ren.ghostLocked = true;
			}
		}
		
		base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e){
        if(e.Button == MouseButton.Right){
			if(ghost != null && ren.ghostLocked){
				foreach(Particle p in ghost){
					p.addVelocity(ren.ghostCenter - ren.cam.mouseWorldPos);
				}
				
				sim.addParticle(ghost);
				ghost = null;
				ren.ghost = null;
				ren.ghostLocked = false;
				sim.tryGenerate();
			}
        }else if(e.Button == MouseButton.Left){
			if(ren.currentScreen == null && selectionLocked){
				((SquareRenderMode) ren.modes[9]).lockEnd(ren.cam.mouseWorldPos);
				selectionLocked = false;
				selectionDisplaying = true;
				ren.mainScreen.buttons[6].active = true;
				ren.mainScreen.buttons[7].active = true;
			}
		}
		
		base.OnMouseUp(e);
    }
}