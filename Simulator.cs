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

class Simulator : GameWindow{
	
	KeyBind escape = new KeyBind(Keys.Escape, Keys.LeftShift, false);
	
	KeyBind fullscreen = new KeyBind(Keys.F11, false);
	KeyBind screnshot = new KeyBind(Keys.F2, false);
	
	KeyBind advancedMode = new KeyBind(Keys.LeftAlt, false);
	
	KeyBind showForces = new KeyBind(Keys.F3, false);
	KeyBind showPoints = new KeyBind(Keys.F6, false);
	KeyBind showBoxes = new KeyBind(Keys.F4, false);
	KeyBind showClouds = new KeyBind(Keys.F5, false);
	
	KeyBind tickForward = new KeyBind(Keys.F, false);
	KeyBind pause = new KeyBind(Keys.Space, false);
	
	KeyBind tickRateUpKey = new KeyBind(Keys.KeyPadAdd, false);
	KeyBind tickRateDownKey = new KeyBind(Keys.KeyPadSubtract, false);
	KeyBind runAtMax = new KeyBind(Keys.M, false);
	
	KeyBind nextParticle = new KeyBind(Keys.Tab, Keys.LeftShift, false);
	
	KeyBind moveUp = new KeyBind(Keys.W, true);
	KeyBind moveDown = new KeyBind(Keys.S, true);
	KeyBind moveLeft = new KeyBind(Keys.A, true);
	KeyBind moveRight = new KeyBind(Keys.D, true);
	
	KeyBind debugInfo = new KeyBind(Keys.I, false);
	
	KeyBind del = new KeyBind(Keys.Backspace, false);
	
	Screen customRPFScreen;
	Screen addParticleScreen;
	Screen newSimulationScreen;
	Screen infoScreen;
	Screen helpScreen;
	Screen pauseMenuScreen;
	
	bool fieldLastWrite;
	
	Renderer ren;
	Simulation sim;
	
	Particle ghost;
	
	public static DeltaHelper dh;
	
	bool isFullscreened;
	
	
	static void Main(string[] args){
		using(Simulator sim = new Simulator()){
			sim.Run();
		}
	}
	
	Simulator() : base(GameWindowSettings.Default, NativeWindowSettings.Default){
		CenterWindow(new Vector2i(640, 480));
		Title = "Unisim";
		
		VSync = VSyncMode.On;
	}
	
	void initialize(){
		dh = new DeltaHelper();
		dh.Start();
		
		setIcon();
		
		DrawBufferController dbc = new DrawBufferController();
		
		sim = new Simulation(Examples.RPF, dbc);
		
		ren = new Renderer(this, sim, dbc);
		
		initializeScreens();
		
		sim.tick();
	}
	
	void initializeScreens(){
		customRPFScreen = new Screen(new Text("Custom RPF", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Field("Min number of particles:", "100", 0, 0, 180f, 6f * Renderer.fieldSeparation, 100f, 4, WritingType.Int),
										new Field("Max number of particles:", "500", 0, 0, 180f, 5f * Renderer.fieldSeparation, 100f, 4, WritingType.Int),
										new Field("Size:", "100", 0, 0, 180f, 4f * Renderer.fieldSeparation, 100f, 8, WritingType.Float).setDescription("Size of the square"),
										new Field("Velocity:", "0.5", 0, 0, 180f, 3f * Renderer.fieldSeparation, 100f, 8, WritingType.Float),
										new Field("Weight of basitrons:", "30", 0, 0, 180f, 1f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Field("Weight of marks:", "30", 0, 0, 180f, 0f, 100f, 5, WritingType.Int),
										new Field("Weight of tarks:", "30", 0, 0, 180f, -1f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Field("Weight of charks:", "5", 0, 0, 180f, -2f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Field("Weight of pharks:", "5", 0, 0, 180f, -3f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Text("Velocity type:", 0, 0, 0f, 2f * Renderer.fieldSeparation),
										new CheckButton(true, 0, 0, 150f, 2f * Renderer.fieldSeparation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor).setDescription("ON: ring OFF: random"),
										new Text("", 0, 1, 0f, 60f, Renderer.redTextColor),
										new TextButton("Done", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(setCustomRPF),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent)).setWriting();
										
										
		addParticleScreen = new Screen(new Text("Add particle", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Field("Name:", "Particle", 0, 0, 80f, 3f * Renderer.fieldSeparation, 180f, 12, WritingType.String),
										new Field("Color:", "AAAAFF", 0, 0, 80f, 2f * Renderer.fieldSeparation, 140f, 6, WritingType.Hex),
										new Field("Mass:", "1", 0, 0, 80f, Renderer.fieldSeparation, 140f, 8, WritingType.Float),
										new Field("Radius:", "1", 0, 0, 80f, 0f, 140f, 8, WritingType.Float),
										new Field("Charge:", "0", 0, 0, 80f, -1f * Renderer.fieldSeparation, 140f, 5, WritingType.Float).setDescription("Electrical charge"),
										new Field("Weak charge:", "0", 0, 0, 80f, -2f * Renderer.fieldSeparation, 140f, 5, WritingType.Float).setDescription("Weak, long range weak force"),
										new Text("", 0, 1, 0f, 60f, Renderer.redTextColor),
										new TextButton("Basitron", 0, -1, -310f, 3f * Renderer.separation, 150f, washDown(Particle.Basitron.color), Renderer.black, new Color3("333333")).setAction(setBasitronScreen),
										new TextButton("Mark", 0, -1, -155f, 3f * Renderer.separation, 150f, washDown(Particle.Mark.color)).setAction(setMarkScreen),
										new TextButton("Tark", 0, -1, 0f, 3f * Renderer.separation, 150f, washDown(Particle.Tark.color)).setAction(setTarkScreen),
										new TextButton("Chark", 0, -1, 155f, 3f * Renderer.separation, 150f, washDown(Particle.Chark.color)).setAction(setCharkScreen),
										new TextButton("Phark", 0, -1, 310f, 3f * Renderer.separation, 150f, washDown(Particle.Phark.color)).setAction(setPharkScreen),
										new TextButton("Done", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(addParticle),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent)).setWriting();
										
		newSimulationScreen = new Screen(new Text("New Simulation", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new TextButton("Solar System", 0, 0, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setSolarSystem),
										new TextButton("Solara System", 0, 0, 0f, 2f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Fictional Solar System").setAction(setSolara),
										new TextButton("Kyra System", 0, 0, 0f, 1f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Fictional Solar System").setAction(setKyra),
										new TextButton("Random solar system", 0, 0, 0f, 0f, 300f, Renderer.buttonColor).setDescription("Could be unstable!").setAction(setRanSS),
										new TextButton("RPF", 0, 0, 0f, -1f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Random elemental particles").setAction(setRPF),
										new ImageBackButton("pencil", 0, 0, 175f, -1f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.textColor, Renderer.buttonColor).setDescription("Edit").setAction(setCustomRPFScreen),
										new TextButton("Empty", 0, 0, 0f, -2f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setEmpty),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent));
										
		infoScreen = new Screen(new Text("Info", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Text("Unisim, created by Dumbelfo", 0, 1, 0f, 3f * Renderer.textSize.Y),
										new Text("Version 1.1.1", 0, 1, 0f, 4f * Renderer.textSize.Y),
										new Log(20f, 20f, 6f * Renderer.textSize.Y, "Particle simulator, aiming to simulate both planet systems and elemental particles"),
										new TextButton("GitHub", 0, -1, -200f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setAction(github),
										new TextButton("Desmos", 0, -1, 0f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setDescription("Graph showing forces between two particles").setAction(desmos),
										new TextButton("Youtube", 0, -1, 200f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setAction(youtube),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent));
										
		helpScreen = new Screen(new Text("Help", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Log(20f, 20f, 4f * Renderer.textSize.Y, "You can pause the simulation with the space bar or by clicking the pause icon.",
																						"You can change simulation speed with numpad + and - or clicking the arrow icons.",
																						"You can toggle max speed with M.",
																						"You can advance 1 tick forward with F.",
																						"You can move the camera with WASD.",
																						"You can follow a particle by clicking on it or use Tab to cycle, and you can click the camera icon or press Shift+Tab to stop following.",
																						"You can toggle advanced mode with Alt.",
																						"You can toggle fullscreen with F11.",
																						"You can take a screenshot with F2.",
																						"You can see velocites(yellow) and forces(green) with F3.",
																						"You can see bounding boxes(red) and collision points(orange) with F4.",
																						"You can toggle background clouds with F5.",
																						"You can toggle particle points with F6.",
																						"You can add particles with the icon, and then choose position by right clicking and velocity(orange) by dragging it. If you press Shift while clicking the button the last added particle will be added again."),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent));
										
		pauseMenuScreen = new Screen(new Text("Pause Menu", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent),
										new TextButton("New Simulation", 0, 0, 0f, 0f, 300f, Renderer.buttonColor).setAction(setNewSimulationScreen),
										new TextButton("Help", 0, 0, 0f, -1f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setHelpScreen),
										new TextButton("Info", 0, 0, 0f, -2f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setInfoScreen),
										new TextButton("Quit", 0, 0, 0f, 1f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(Close));
	}
	
	Color3 washDown(Color3 c){
		return new Color3((byte) (c.R * 0.6f), (byte) (c.G * 0.6f), (byte) (c.B * 0.6f));
	}
	
	void onResize(int x, int y){
		GL.Viewport(0, 0, x, y);
		if(this.ren != null){
			ren.updateSize(x, y);
		}
	}
	
	void closeCurrent(){
		ren.setScreen(null);
	}
	
	void setSimulation(Particle[] p){
		if(sim.isRunning){
			return;
		}
		
		ren.cam.setFollow(null);
		ren.cam.position = new Vector2d(0d, 0d);
		ren.cam.updateForce();
		
		sim.reset(p);
	}
	
	void setEmpty(){
		setSimulation(new Particle[]{});
	}
	
	void setSolarSystem(){
		setSimulation(Examples.solntse);
	}
	
	void setRPF(){
		setSimulation(Examples.RPF);
	}
	
	void setRanSS(){
		setSimulation(PlanetSystem.Random);
	}
	
	void setSolara(){
		setSimulation(Examples.solara);
	}
	
	void setKyra(){
		setSimulation(Examples.kyra);
	}
	
	void setCustomRPF(){
		uint m;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[1]).text, out m)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse min num");
			return;
		}
		
		uint x;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[2]).text, out x)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse max num");
			return;
		}
		
		if(m > x){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Min cant be bigger than max");
			return;
		}
		
		float s;
		if(!float.TryParse(((Field)customRPFScreen.buttons[3]).text, out s)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse size");
			return;
		}
		
		float v;
		if(!float.TryParse(((Field)customRPFScreen.buttons[4]).text, out v)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse velocity");
			return;
		}
		
		uint b;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[5]).text, out b)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse basitrons");
			return;
		}
		
		uint mw;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[6]).text, out mw)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse marks");
			return;
		}
		
		uint t;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[7]).text, out t)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse tarks");
			return;
		}
		
		uint c;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[8]).text, out c)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse charks");
			return;
		}
		
		uint p;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[9]).text, out p)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse pharks");
			return;
		}
		
		bool r = ((CheckButton)customRPFScreen.buttons[11]).on;
		
		setSimulation(Examples.RPFparams((int) m, (int) x, s, v, (int) b, (int) mw, (int) t, (int) c, (int) p, r));
	}
	
	void setBasitronScreen(){
		((Field)addParticleScreen.buttons[1]).text = Particle.Basitron.name;
		((Field)addParticleScreen.buttons[2]).text = Particle.Basitron.color.ToString().Substring(1);
		((Field)addParticleScreen.buttons[3]).text = Particle.Basitron.mass.ToString();
		((Field)addParticleScreen.buttons[4]).text = Particle.Basitron.radius.ToString();
		((Field)addParticleScreen.buttons[5]).text = Particle.Basitron.charge.ToString();
		((Field)addParticleScreen.buttons[6]).text = Particle.Basitron.weak.ToString();
	}
	
	void setMarkScreen(){
		((Field)addParticleScreen.buttons[1]).text = Particle.Mark.name;
		((Field)addParticleScreen.buttons[2]).text = Particle.Mark.color.ToString().Substring(1);
		((Field)addParticleScreen.buttons[3]).text = Particle.Mark.mass.ToString();
		((Field)addParticleScreen.buttons[4]).text = Particle.Mark.radius.ToString();
		((Field)addParticleScreen.buttons[5]).text = Particle.Mark.charge.ToString();
		((Field)addParticleScreen.buttons[6]).text = Particle.Mark.weak.ToString();
	}
	
	void setTarkScreen(){
		((Field)addParticleScreen.buttons[1]).text = Particle.Tark.name;
		((Field)addParticleScreen.buttons[2]).text = Particle.Tark.color.ToString().Substring(1);
		((Field)addParticleScreen.buttons[3]).text = Particle.Tark.mass.ToString();
		((Field)addParticleScreen.buttons[4]).text = Particle.Tark.radius.ToString();
		((Field)addParticleScreen.buttons[5]).text = Particle.Tark.charge.ToString();
		((Field)addParticleScreen.buttons[6]).text = Particle.Tark.weak.ToString();
	}
	
	void setCharkScreen(){
		((Field)addParticleScreen.buttons[1]).text = Particle.Chark.name;
		((Field)addParticleScreen.buttons[2]).text = Particle.Chark.color.ToString().Substring(1);
		((Field)addParticleScreen.buttons[3]).text = Particle.Chark.mass.ToString();
		((Field)addParticleScreen.buttons[4]).text = Particle.Chark.radius.ToString();
		((Field)addParticleScreen.buttons[5]).text = Particle.Chark.charge.ToString();
		((Field)addParticleScreen.buttons[6]).text = Particle.Chark.weak.ToString();
	}
	
	void setPharkScreen(){
		((Field)addParticleScreen.buttons[1]).text = Particle.Phark.name;
		((Field)addParticleScreen.buttons[2]).text = Particle.Phark.color.ToString().Substring(1);
		((Field)addParticleScreen.buttons[3]).text = Particle.Phark.mass.ToString();
		((Field)addParticleScreen.buttons[4]).text = Particle.Phark.radius.ToString();
		((Field)addParticleScreen.buttons[5]).text = Particle.Phark.charge.ToString();
		((Field)addParticleScreen.buttons[6]).text = Particle.Phark.weak.ToString();
	}
	
	public void addParticle(){
		string name = null;
		if(((Field)addParticleScreen.buttons[1]).text.Length > 0){
			name = ((Field)addParticleScreen.buttons[1]).text;
			name = char.ToUpper(name[0]) + name.Substring(1);
		}
		
		Color3 c;
		try{
			c = new Color3(((Field)addParticleScreen.buttons[2]).text);
		}catch(Exception){
			((Text)addParticleScreen.buttons[7]).setText(ren, "Couldnt parse color");
			ren.setCornerInfo("Couldnt parse color");
			return;
		}
		
		float mass;
		if(!float.TryParse(((Field)addParticleScreen.buttons[3]).text, out mass)){
			((Text)addParticleScreen.buttons[7]).setText(ren, "Couldnt parse mass");
			ren.setCornerInfo("Couldnt parse mass");
			return;
		}
		
		if(mass <= 0f){
			((Text)addParticleScreen.buttons[7]).setText(ren, "Mass must be positive");
			ren.setCornerInfo("Mass must be positive");
			return;
		}
		
		float radius;
		if(!float.TryParse(((Field)addParticleScreen.buttons[4]).text, out radius)){
			((Text)addParticleScreen.buttons[7]).setText(ren, "Couldnt parse radius");
			ren.setCornerInfo("Couldnt parse radius");
			return;
		}
		
		if(radius <= 0f){
			((Text)addParticleScreen.buttons[7]).setText(ren, "Radius must be positive");
			ren.setCornerInfo("Radius must be positive");
			return;
		}
		
		float charge;
		if(!float.TryParse(((Field)addParticleScreen.buttons[5]).text, out charge)){
			((Text)addParticleScreen.buttons[7]).setText(ren, "Couldnt parse charge");
			ren.setCornerInfo("Couldnt parse charge");
			return;
		}
		
		float weak;
		if(!float.TryParse(((Field)addParticleScreen.buttons[6]).text, out weak)){
			((Text)addParticleScreen.buttons[7]).setText(ren, "Couldnt parse weak");
			ren.setCornerInfo("Couldnt parse weak");
			return;
		}
		
		ghost = new Particle(radius, mass, charge, weak, c).setName(name);
		ren.ghost = ghost;
		ren.currentScreen = null;
	}
	
	void setCustomRPFScreen(){
		ren.setScreen(customRPFScreen);
	}
	
	public void setAddParticleScreen(){
		ren.setScreen(addParticleScreen);
	}
	
	void github(){
		Process.Start(new ProcessStartInfo("https://github.com/siljamdev/Unisim"){UseShellExecute = true});
	}
	
	void desmos(){
		Process.Start(new ProcessStartInfo("https://www.desmos.com/calculator/8bq31utqb4"){UseShellExecute = true});
	}
	
	void youtube(){
		Process.Start(new ProcessStartInfo("https://www.youtube.com/watch?v=LXCAxlIyGBQ&t"){UseShellExecute = true});
	}
	
	void setNewSimulationScreen(){
		ren.setScreen(newSimulationScreen);
	}
	
	void setInfoScreen(){
		ren.setScreen(infoScreen);
	}
	
	void setHelpScreen(){
		ren.setScreen(helpScreen);
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
				
				ren.setScreen(pauseMenuScreen);
			}
			break;
			
			case 2:
			Close();
			break;
		}
		
		if(screnshot.isActive(KeyboardState)){
			captureScreenshot();
			ren.setCornerInfo("Saved screenshot");
		}
		
		if(fullscreen.isActive(KeyboardState)){
			toggleFullscreen();
		}
		
		if(ren.currentScreen != null){
			if(ren.currentScreen.writing){
				if(del.isActive(KeyboardState)){
					ren.currentScreen.tryDel();
				}else if(!fieldLastWrite){
					switch(ren.currentScreen.tryGet()){
						case WritingType.Hex:
						if(KeyBind.getHexTyping(KeyboardState, out char c)){
							ren.currentScreen.tryAdd(c);
							fieldLastWrite = true;
						}
						break;
						
						case WritingType.Int:
						if(KeyBind.getIntTyping(KeyboardState, out c)){
							ren.currentScreen.tryAdd(c);
							fieldLastWrite = true;
						}
						break;
						
						case WritingType.Float:
						if(KeyBind.getFloatTyping(KeyboardState, out c)){
							ren.currentScreen.tryAdd(c);
							fieldLastWrite = true;
						}
						break;
						
						case WritingType.String:
						if(KeyBind.getStringTyping(KeyboardState, out c)){
							ren.currentScreen.tryAdd(c);
							fieldLastWrite = true;
						}
						break;
					}
					
				}else{
					if(!KeyBind.getFullTyping(KeyboardState, out char c)){
						fieldLastWrite = false;
					}
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
			sim.runAtMax = !sim.runAtMax;
			
			if(sim.runAtMax){
				ren.setCornerInfo("Running at max speed");
			}else{
				ren.setCornerInfo("Disabled max speed");
			}
		}
		
		if(tickForward.isActive(KeyboardState)){
			if(!sim.isRunning){
				sim.tick();
				ren.setCornerInfo("Tick advanced");
			}
		}
		
		if(pause.isActive(KeyboardState)){
			togglePause();
		}
		
		if(advancedMode.isActive(KeyboardState)){
			ren.toggleAdvancedMode();
		}
		
		if(showForces.isActive(KeyboardState)){
			ren.modes[3].toggleActivation();
			ren.modes[4].toggleActivation();
			sim.changeForceMode = true;
			sim.tryGenerate();
			
			if(ren.modes[3].active){
				ren.setCornerInfo("Forces enabled");
			}else{
				ren.setCornerInfo("Forces disabled");
			}
		}
		
		if(showPoints.isActive(KeyboardState)){
			ren.modes[2].toggleActivation();
			
			if(ren.modes[2].active){
				ren.setCornerInfo("Points enabled");
			}else{
				ren.setCornerInfo("Points disabled");
			}
		}
		
		if(showBoxes.isActive(KeyboardState)){
			ren.modes[5].toggleActivation();
			ren.modes[6].toggleActivation();
			sim.tryGenerate();
			
			if(ren.modes[5].active){
				ren.setCornerInfo("Bounding boxes enabled");
			}else{
				ren.setCornerInfo("Bounding boxes disabled");
			}
		}
		
		if(showClouds.isActive(KeyboardState)){
			ren.modes[0].toggleActivation();
			sim.tryGenerate();
			
			if(ren.modes[0].active){
				ren.setCornerInfo("Clouds enabled");
			}else{
				ren.setCornerInfo("Clouds disabled");
			}
		}
		
		if(debugInfo.isActive(KeyboardState)){
			Console.WriteLine(sim.tt.meanInfo());
			/* File.WriteAllText("log.tlog", Simulation.tl.getLog());
			
			string notepadPlusPlusPath = @"C:\Program Files\Notepad++\notepad++.exe"; // Adjust this path if necessary
			
			// Use Process.Start to open the file with Notepad++
			Process.Start(new ProcessStartInfo
			{
				FileName = notepadPlusPlusPath,
				Arguments = "\"log.tlog\"", // Quote the file path to handle spaces
				UseShellExecute = false // Ensures the process uses the provided executable
			}); */
		}
		
		switch(nextParticle.isActiveMod(KeyboardState)){
			case 1:
				Particle p = sim.getNextParticle(ren.cam.follow);
				ren.cam.setFollow(p);
				break;
			case 2:
				ren.cam.setFollow(null);
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
			this.CurrentMonitor = mi.Handle;
			isFullscreened = true;
			VSync = VSyncMode.On;
		} else {
			WindowState = WindowState.Normal;
			isFullscreened = false;
			VSync = VSyncMode.On;
		}
	}
	
	public static void checkErrors(){
		OpenTK.Graphics.OpenGL.ErrorCode errorCode = GL.GetError();
        while(errorCode != OpenTK.Graphics.OpenGL.ErrorCode.NoError){
            Console.WriteLine("OpenGL Error: " + errorCode);
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
			bitmap.Save("screenshot" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".png", ImageFormat.Png);
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
		base.OnRenderFrame(args);
		dh.Frame();
		//dh.Target(144f);
	}
	
	protected override void OnMouseWheel(MouseWheelEventArgs args){
		if(ren.currentScreen != null){
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
					}
				}
			}else{
				ren.currentScreen.click(ren, ren.cam.mouseLastPos, KeyboardState.IsKeyDown(Keys.LeftShift));
			}
        }else if(e.Button == MouseButton.Right){
			if(ghost != null && !ren.ghostLocked){
				ghost.translate(ren.cam.mouseWorldPos);
				ren.ghostLocked = true;
			}
		}
		
		base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e){
        if(e.Button == MouseButton.Right){
			if(ghost != null && ren.ghostLocked){
				ghost.addVelocity(ghost.position - ren.cam.mouseWorldPos);
				sim.addParticle(ghost);
				ghost = null;
				ren.ghost = null;
				ren.ghostLocked = false;
				sim.tryGenerate();
			}
        }
		
		base.OnMouseUp(e);
    }
}