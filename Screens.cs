using System;
using System.Diagnostics;
using System.IO;
using OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using AshLib;
using AshLib.AshFiles;

partial class Simulator : GameWindow{
	Screen customRPFScreen;
	Screen addParticleScreen;
	Screen newSimulationScreen;
	Screen infoScreen;
	Screen helpScreen;
	Screen pauseMenuScreen;
	Screen optionsScreen;
	Screen dropScreen;
	
	void initializeScreens(){
		customRPFScreen = new Screen(new Text("Custom RPF", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Field("Min number of particles:", "100", 0, 0, 180f, 6f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Field("Max number of particles:", "500", 0, 0, 180f, 5f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Field("Size:", "100", 0, 0, 180f, 4f * Renderer.fieldSeparation, 100f, 8, WritingType.Float).setDescription("Size of the square"),
										new Field("Velocity:", "0.5", 0, 0, 180f, 3f * Renderer.fieldSeparation, 100f, 8, WritingType.Float),
										new Field("Weight of basitrons:", "30", 0, 0, 180f, 1f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Field("Weight of marks:", "30", 0, 0, 180f, 0f, 100f, 5, WritingType.Int),
										new Field("Weight of tarks:", "30", 0, 0, 180f, -1f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Field("Weight of charks:", "5", 0, 0, 180f, -2f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Field("Weight of pharks:", "5", 0, 0, 180f, -3f * Renderer.fieldSeparation, 100f, 5, WritingType.Int),
										new Text("Velocity type:", 0, 0, 0f, 2f * Renderer.fieldSeparation),
										new CheckButton(true, 0, 0, 150f, 2f * Renderer.fieldSeparation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor).setDescription("ON: ring OFF: random"),
										new TextButton("Reset", 0, -1, 0f, 3.5f * Renderer.separation, 300f, Renderer.buttonColor).setAction(resetCustomRPF),
										new TextButton("Done", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(setCustomRPF),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent)).setWriting()
										.setErrorText(new Text("", 0, 1, 0f, 60f, Renderer.redTextColor));
										
										
		addParticleScreen = new Screen(new Text("Add particle", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Field("Name:", "Particle", 0, 0, 80f, 3f * Renderer.fieldSeparation, 180f, 16, WritingType.String),
										new Field("Color:", "AAAAFF", 0, 0, 80f, 2f * Renderer.fieldSeparation, 140f, 6, WritingType.Hex),
										new Field("Mass:", "1", 0, 0, 80f, Renderer.fieldSeparation, 140f, 8, WritingType.Float),
										new Field("Radius:", "1", 0, 0, 80f, 0f, 140f, 8, WritingType.Float),
										new Field("Charge:", "0", 0, 0, 80f, -1f * Renderer.fieldSeparation, 140f, 5, WritingType.Float).setDescription("Electrical charge"),
										new Field("Weak charge:", "0", 0, 0, 80f, -2f * Renderer.fieldSeparation, 140f, 5, WritingType.Float).setDescription("Weak, long range weak force"),
										new TextButton("Basitron", 0, -1, -310f, 3f * Renderer.separation, 150f, washDown(Particle.Basitron.color), Renderer.black, new Color3("333333")).setAction(setBasitronScreen),
										new TextButton("Mark", 0, -1, -155f, 3f * Renderer.separation, 150f, washDown(Particle.Mark.color)).setAction(setMarkScreen),
										new TextButton("Tark", 0, -1, 0f, 3f * Renderer.separation, 150f, washDown(Particle.Tark.color)).setAction(setTarkScreen),
										new TextButton("Chark", 0, -1, 155f, 3f * Renderer.separation, 150f, washDown(Particle.Chark.color)).setAction(setCharkScreen),
										new TextButton("Phark", 0, -1, 310f, 3f * Renderer.separation, 150f, washDown(Particle.Phark.color)).setAction(setPharkScreen),
										new TextButton("Done", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(addParticle),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent)).setWriting()
										.setErrorText(new Text("", 0, 1, 0f, 60f, Renderer.redTextColor));
										
		newSimulationScreen = new Screen(new Text("New Simulation", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new TextButton("Solar System", 0, 0, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setSolarSystem),
										new TextButton("Solara System", 0, 0, 0f, 2f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Fictional Star System").setAction(setSolara),
										new TextButton("Kyra System", 0, 0, 0f, 1f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Fictional Star System").setAction(setKyra),
										new TextButton("Random star system", 0, 0, 0f, 0f, 300f, Renderer.buttonColor).setDescription("Could be unstable!").setAction(setRanSS),
										new TextButton("RPF", 0, 0, 0f, -1f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Random elemental particles").setAction(setRPF),
										new ImageBackButton("pencil", 0, 0, 175f, -1f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.textColor, Renderer.buttonColor).setDescription("Edit").setAction(setCustomRPFScreen),
										new TextButton("Empty", 0, 0, 0f, -2f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setEmpty),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent));
										
		infoScreen = new Screen(new Text("Info", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Text("Unisim, created by Siljam", 0, 1, 0f, 3f * Renderer.textSize.Y, Renderer.selectedTextColor),
										new Text("Version 1.2.0", 0, 1, 0f, 4f * Renderer.textSize.Y, Renderer.selectedTextColor),
										new TextButton("GitHub", 0, -1, -200f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setAction(github),
										new TextButton("Desmos", 0, -1, 0f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setDescription("Graph showing forces between two particles").setAction(desmos),
										new TextButton("Instagram", 0, -1, 200f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setDescription("Follow me on ig!").setAction(instagram),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setScrollingLog(new Log(20f, 20f, 6f * Renderer.textSize.Y, "Particle simulator, aiming to simulate both planet systems and elemental particles",
														"Follow me on instagram for regular updates on my projects!",
														"",
														"### v1.2.0 Changelog - February 2025 ###",
														"+ Added square selections",
														"+ Added options",
														"+ Added many buttons and gui improvements",
														"+ Updated AshLib to latest version",
														"+ Added the ability to remove and duplicate particles",
														"- Deleted the clouds keybind",
														"",
														"### v1.1.1b Changelog - January 2025 ###",
														"+ Changed the username and links (name change from Dumblefo to Siljam)",
														"",
														"### v1.1.1 Changelog - December 2024 ###",
														"+ Position snaps backs to 0,0 when creating a new scene",
														"",
														"### v1.1.0 Changelog - December 2024 ###",
														"+ Changed background color",
														"+ Improved UI",
														"+ Added more functionality to the add button",
														"+ Improved performance",
														"",
														"### v1.0.1 Changelog - December 2024 ###",
														"+ Fixed the executable",
														"",
														"### v1.0.0 Changelog - December 2024 ###",
														"Initial release!"));
										
		helpScreen = new Screen(new Text("Help", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setScrollingLog(new Log(20f, 20f, 4f * Renderer.textSize.Y, "You can pause the simulation with the space bar or by clicking the pause button.",
														"You can change simulation speed with numpad + and - or clicking the arrow buttons.",
														"You can toggle max speed with M or by clicking the button.",
														"You can advance 1 tick forward with F.",
														"You can move the camera with WASD.",
														"You can follow a particle by clicking on it or use Tab to cycle, and you can click the camera button or press Shift+Tab to stop following. You can also click the tab button to go to next or Shift+click it to go to previous.",
														"You can toggle advanced mode with Alt.",
														"You can toggle fullscreen with F11.",
														"You can take a screenshot with F2 or with the button in the pause menu.",
														"You can see velocites(yellow) and forces(green) with F3.",
														"You can see bounding boxes(red) and collision points(orange) with F4.",
														"You can toggle particle points with F5.",
														"You can add particles with the button, and then choose position by right clicking and velocity(orange) by dragging it. If you press Shift+click the button or press E the last added particle will be added again.",
														"You can make a square selection with L or clicking the button.",
														"You can delete a square selection or following particle with R or clicking the button.",
														"You can duplicate a square selection or following particle with V or clicking the button.",
														"You can save and load scenes with the buttons in the pause menu."));
		
		optionsScreen = new Screen(new Text("Options", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Text("Clouds:", 0, 0, 0f, 3f * Renderer.separation),
										new CheckButton(true, 0, 0, 80f, 3f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor),
										new Field("Dot size:", "3", 0, 0, 100f, 2f * Renderer.separation, 90f, 5, WritingType.FloatPositive),
										new Field("Background color:", "08081A", 0, 0, 150f, 1f * Renderer.separation, 120f, 6, WritingType.Hex),
										new Field("Max particles:", "1000", 0, 0, 140f, 0f, 100f, 5, WritingType.Int).setDescription("Might reduce performance"),
										new Text("Vsync:", 0, 0, 0f, -1f * Renderer.separation),
										new CheckButton(true, 0, 0, 80f, -1f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor),
										new Field("Targer framerate:", "144", 0, 0, 150f, -2f * Renderer.separation, 100f, 4, WritingType.Int).setDescription("Vsync overrites it"),
										new TextButton("Reset", 0, -1, 0f, 2.5f * Renderer.separation, 300f, Renderer.buttonColor).setAction(resetConfig),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setWriting().setCloseAction(saveConfig).setErrorText(new Text("", 0, 1, 0f, 60f, Renderer.redTextColor));
										
		dropScreen = new Screen(new Text("Drop the file into the window", 0, 0, 0f, 0f, Renderer.selectedTextColor));
										
		pauseMenuScreen = new Screen(new Text("Pause Menu", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent),
										new TextButton("Options", 0, 0, 0f, 1f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setOptionsScreen),
										new TextButton("New Simulation", 0, 0, 0f, 0f, 300f, Renderer.buttonColor).setAction(setNewSimulationScreen),
										new TextButton("Help", 0, 0, 0f, -1f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setHelpScreen),
										new TextButton("Info", 0, 0, 0f, -2f * Renderer.separation, 300f, Renderer.buttonColor).setAction(setInfoScreen),
										new ImageButton("screenshot", 1, 0, 10f, 0f, 35f, 35f, Renderer.textColor).setDescription("Take screenshot").setAction(takeScreenshotButton),
										new ImageButton("load", -1, 0, 10f, 15f, 30f, 30f, Renderer.textColor).setDescription("Load scene").setAction(loadScene),
										new ImageButton("save", -1, 0, 10f, -15f, 30f, 30f, Renderer.textColor).setDescription("Save scene").setAction(saveScene),
										new TextButton("Quit", 0, 0, 0f, 2.5f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(Close));
	}
	
	Color3 washDown(Color3 c){
		return new Color3((byte) (c.R * 0.6f), (byte) (c.G * 0.6f), (byte) (c.B * 0.6f));
	}
	
	void resetConfig(){
		dep.config.SetCamp("clouds", true);
		dep.config.SetCamp("dotSize", 3f);
		dep.config.SetCamp("bgColor", new Color3(8, 8, 26));
		dep.config.SetCamp("maxParticles", 1000);
		dep.config.SetCamp("maxFps", 144);
		
		ren.setBgColor(dep.config.GetCamp<Color3>("bgColor"));
		if(dep.config.GetCamp<bool>("clouds") != ren.modes[0].active){
			ren.modes[0].toggleActivation();
		}
		((PointRenderMode) ren.modes[2]).setPointSize(dep.config.GetCamp<float>("dotSize"));
		setVsync(dep.config.GetCamp<bool>("vsync"));
		maxFps = dep.config.GetCamp<int>("maxFps");
		
		((CheckButton) optionsScreen.buttons[2]).on = dep.config.GetCamp<bool>("clouds");
		((Field) optionsScreen.buttons[3]).text = dep.config.GetCamp<float>("dotSize").ToString();
		((Field) optionsScreen.buttons[4]).text = dep.config.GetCamp<Color3>("bgColor").ToString().Substring(1);
		((Field) optionsScreen.buttons[5]).text = dep.config.GetCamp<int>("maxParticles").ToString();
		((CheckButton) optionsScreen.buttons[7]).on = dep.config.GetCamp<bool>("vsync");
		((Field) optionsScreen.buttons[8]).text = dep.config.GetCamp<int>("maxFps").ToString();
		
		dep.config.Save();
	}
	
	void saveConfig(){		
		float d;
		if(!float.TryParse(((Field) optionsScreen.buttons[3]).text, out d)){
			optionsScreen.showError(ren, "Couldnt parse dot size");
			ren.setCornerInfo("Couldnt parse dot size");
			return;
		}
		
		Color3 b;
		if(!Color3.TryParse(((Field) optionsScreen.buttons[4]).text, out b)){
			optionsScreen.showError(ren, "Couldnt parse background color");
			ren.setCornerInfo("Couldnt parse background color");
			return;
		}
		
		int m;
		if(!int.TryParse(((Field) optionsScreen.buttons[5]).text, out m)){
			optionsScreen.showError(ren, "Couldnt parse max particles");
			ren.setCornerInfo("Couldnt parse max particles");
			return;
		}
		
		int f;
		if(!int.TryParse(((Field) optionsScreen.buttons[8]).text, out f)){
			optionsScreen.showError(ren, "Couldnt parse target framerate");
			ren.setCornerInfo("Couldnt parse target framerate");
			return;
		}
		
		dep.config.SetCamp("clouds", ((CheckButton) optionsScreen.buttons[2]).on);
		dep.config.SetCamp("dotSize", d);
		dep.config.SetCamp("bgColor", b);
		dep.config.SetCamp("maxParticles", m);
		dep.config.SetCamp("vsync", ((CheckButton) optionsScreen.buttons[7]).on);
		dep.config.SetCamp("maxFps", f);
		
		ren.setBgColor(b);
		if(dep.config.GetCamp<bool>("clouds") != ren.modes[0].active){
			ren.modes[0].toggleActivation();
		}
		((PointRenderMode) ren.modes[2]).setPointSize(d);
		setVsync(dep.config.GetCamp<bool>("vsync"));
		maxFps = f;
		
		if(Simulation.maxParticles != m){
			optionsScreen.showError(ren, "Some changes will apply when unisim restarts");
		}else{
			optionsScreen.showError(ren, "");
		}
		
		dep.config.Save();
	}
	
	void saveScene(){
		AshFile af = FileConverter.getFile(sim.getParticlesForSaving());
		dep.SaveAshFile("saves/scene_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".unisim", af);
		ren.setCornerInfo("Scene saved");
		closeCurrent();
	}
	
	void loadScene(){
		ren.setScreen(dropScreen);
	}
	
	void closeCurrent(){
		ren.setScreen(null);
	}
	
	void takeScreenshotButton(){
		closeCurrent();
		takeScreenshotNextTick = true;
	}
	
	void setSimulation(Particle[] p){
		if(sim.isRunning){
			return;
		}
		
		ren.cam.setFollow(null);
		ren.cam.position = new Vector2d(0d, 0d);
		ren.cam.resetZoom();
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
		uint m;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[1]).text, out m)){
			setSimulation(Examples.RPF);
			return;
		}
		
		uint x;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[2]).text, out x)){
			setSimulation(Examples.RPF);
			return;
		}
		
		if(m > x){
			setSimulation(Examples.RPF);
			return;
		}
		
		if(x > Simulation.maxParticles){
			setSimulation(Examples.RPF);
			return;
		}
		
		float s;
		if(!float.TryParse(((Field)customRPFScreen.buttons[3]).text, out s)){
			setSimulation(Examples.RPF);
			return;
		}
		
		float v;
		if(!float.TryParse(((Field)customRPFScreen.buttons[4]).text, out v)){
			setSimulation(Examples.RPF);
			return;
		}
		
		uint b;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[5]).text, out b)){
			setSimulation(Examples.RPF);
			return;
		}
		
		uint mw;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[6]).text, out mw)){
			setSimulation(Examples.RPF);
			return;
		}
		
		uint t;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[7]).text, out t)){
			setSimulation(Examples.RPF);
			return;
		}
		
		uint c;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[8]).text, out c)){
			setSimulation(Examples.RPF);
			return;
		}
		
		uint p;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[9]).text, out p)){
			setSimulation(Examples.RPF);
			return;
		}
		
		bool r = ((CheckButton)customRPFScreen.buttons[11]).on;
		
		setSimulation(Examples.RPFparams((int) m, (int) x, s, v, (int) b, (int) mw, (int) t, (int) c, (int) p, r));
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
	
	void resetCustomRPF(){
		((Field)customRPFScreen.buttons[1]).text = "100";
		((Field)customRPFScreen.buttons[2]).text = "500";
		((Field)customRPFScreen.buttons[3]).text = "100";
		((Field)customRPFScreen.buttons[4]).text = "0.5";
		((Field)customRPFScreen.buttons[5]).text = "30";
		((Field)customRPFScreen.buttons[6]).text = "30";
		((Field)customRPFScreen.buttons[7]).text = "30";
		((Field)customRPFScreen.buttons[8]).text = "5";
		((Field)customRPFScreen.buttons[9]).text = "5";
		((CheckButton)customRPFScreen.buttons[11]).on = true;
	}
	
	void setCustomRPF(){
		uint m;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[1]).text, out m)){
			((Text)customRPFScreen.buttons[12]).setText(ren, "Couldnt parse min num");
			return;
		}
		
		uint x;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[2]).text, out x)){
			customRPFScreen.showError(ren, "Couldnt parse max num");
			return;
		}
		
		if(m > x){
			customRPFScreen.showError(ren, "Min cant be bigger than max");
			return;
		}
		
		if(x > Simulation.maxParticles){
			customRPFScreen.showError(ren, "There cannot be more than " + Simulation.maxParticles + " particles");
			return;
		}
		
		float s;
		if(!float.TryParse(((Field)customRPFScreen.buttons[3]).text, out s)){
			customRPFScreen.showError(ren, "Couldnt parse size");
			return;
		}
		
		float v;
		if(!float.TryParse(((Field)customRPFScreen.buttons[4]).text, out v)){
			customRPFScreen.showError(ren, "Couldnt parse velocity");
			return;
		}
		
		uint b;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[5]).text, out b)){
			customRPFScreen.showError(ren, "Couldnt parse basitrons");
			return;
		}
		
		uint mw;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[6]).text, out mw)){
			customRPFScreen.showError(ren, "Couldnt parse marks");
			return;
		}
		
		uint t;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[7]).text, out t)){
			customRPFScreen.showError(ren, "Couldnt parse tarks");
			return;
		}
		
		uint c;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[8]).text, out c)){
			customRPFScreen.showError(ren, "Couldnt parse charks");
			return;
		}
		
		uint p;
		if(!uint.TryParse(((Field)customRPFScreen.buttons[9]).text, out p)){
			customRPFScreen.showError(ren, "Couldnt parse pharks");
			return;
		}
		
		bool r = ((CheckButton)customRPFScreen.buttons[11]).on;
		
		customRPFScreen.showError(ren, "");
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
			addParticleScreen.showError(ren, "Couldnt parse color");
			ren.setCornerInfo("Couldnt parse color");
			return;
		}
		
		float mass;
		if(!float.TryParse(((Field)addParticleScreen.buttons[3]).text, out mass)){
			addParticleScreen.showError(ren, "Couldnt parse mass");
			ren.setCornerInfo("Couldnt parse mass");
			return;
		}
		
		if(mass <= 0f){
			addParticleScreen.showError(ren, "Mass must be positive");
			ren.setCornerInfo("Mass must be positive");
			return;
		}
		
		float radius;
		if(!float.TryParse(((Field)addParticleScreen.buttons[4]).text, out radius)){
			addParticleScreen.showError(ren, "Couldnt parse radius");
			ren.setCornerInfo("Couldnt parse radius");
			return;
		}
		
		if(radius <= 0f){
			addParticleScreen.showError(ren, "Radius must be positive");
			ren.setCornerInfo("Radius must be positive");
			return;
		}
		
		float charge;
		if(!float.TryParse(((Field)addParticleScreen.buttons[5]).text, out charge)){
			addParticleScreen.showError(ren, "Couldnt parse charge");
			ren.setCornerInfo("Couldnt parse charge");
			return;
		}
		
		float weak;
		if(!float.TryParse(((Field)addParticleScreen.buttons[6]).text, out weak)){
			addParticleScreen.showError(ren, "Couldnt parse weak");
			ren.setCornerInfo("Couldnt parse weak");
			return;
		}
		
		addParticleScreen.showError(ren, "");
		setGhost(new Particle(radius, mass, charge, weak, c).setName(name));
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
	
	void instagram(){
		Process.Start(new ProcessStartInfo("https://www.instagram.com/siljamdev/"){UseShellExecute = true});
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
	
	void setOptionsScreen(){
		ren.setScreen(optionsScreen);
	}
}