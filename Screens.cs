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

#if WINDOWS
	using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
#endif

partial class Simulator : GameWindow{
	Screen customRPFScreen;
	Screen addParticleScreen;
	Screen newSimulationScreen;
	Screen infoScreen;
	Screen helpScreen;
	Screen pauseMenuScreen;
	Screen optionsScreen1;
	Screen optionsScreen2;
	Screen dropScreen;
	Screen sceneConfigScreen;
	Screen controlScreen1;
	Screen controlScreen2;
	
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
										new TextButton("Reset", 0, -1, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(resetCustomRPF),
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
										new TextButton("Solar System", 0, 0, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(() => setSimulation(Examples.solntse)),
										new TextButton("Solara System", 0, 0, 0f, 2f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Fictional Star System").setAction(() => setSimulation(Examples.solara)),
										new TextButton("Kyra System", 0, 0, 0f, 1f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Fictional Star System").setAction(() => setSimulation(Examples.kyra)),
										new TextButton("Random star system", 0, 0, 0f, 0f, 300f, Renderer.buttonColor).setDescription("Could be unstable!").setAction(() => setSimulation(PlanetSystem.Random)),
										new TextButton("RPF", 0, 0, 0f, -1f * Renderer.separation, 300f, Renderer.buttonColor).setDescription("Random elemental particles").setAction(setRPF),
										new ImageBackButton("pencil", 0, 0, 175f, -1f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.textColor, Renderer.buttonColor).setDescription("Edit").setAction(() => ren.setScreen(customRPFScreen)),
										new TextButton("Empty", 0, 0, 0f, -2f * Renderer.separation, 300f, Renderer.buttonColor).setAction(() => setSimulation(new Scene(new List<Particle>(), null, generateRandomName()))),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent));
										
		infoScreen = new Screen(new Text("Info", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Text("Unisim, created by Siljam", 0, 1, 0f, 3f * Renderer.textSize.Y, Renderer.selectedTextColor),
										new Text("Version 1.2.0", 0, 1, 0f, 4f * Renderer.textSize.Y, Renderer.selectedTextColor),
										new TextButton("GitHub", 0, -1, -200f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setAction(() => Process.Start(new ProcessStartInfo("https://github.com/siljamdev/Unisim"){UseShellExecute = true})),
										new TextButton("Desmos", 0, -1, 0f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setDescription("Graph showing forces between two particles").setAction(() => Process.Start(new ProcessStartInfo("https://www.desmos.com/calculator/8bq31utqb4"){UseShellExecute = true})),
										new TextButton("Instagram", 0, -1, 200f, 3f * Renderer.separation, 190f, Renderer.buttonColor).setDescription("Follow me on ig!").setAction(() => Process.Start(new ProcessStartInfo("https://www.instagram.com/siljamdev/"){UseShellExecute = true})),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setScrollingLog(new Log(20f, 20f, 6f * Renderer.textSize.Y, "Particle simulator, aiming to simulate both planet systems and elemental particles",
														"Follow me on instagram for regular updates on my projects!",
														"",
														"### v1.2.0 Changelog - March 2025 ###",
														"+ Added square selections",
														"+ Added the world border",
														"+ Added options",
														"+ Added many buttons and gui improvements (A LOT!)",
														"+ Updated AshLib and OpenTK to latest version",
														"+ Added the ability to remove and duplicate particles",
														"+ Added controls",
														"+ Added multithreading to a lot of places, increasing performance (toggleable)",
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
														"You can set the world border and other scene configurations with the icon in the menu.",
														"You can save and load scenes with the buttons in the pause menu."));
														
		Text optionsErrorText = new Text("", 0, 1, 0f, 60f, Renderer.redTextColor);
		
		optionsScreen1 = new Screen(new Text("Options", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Text("Clouds:", 0, 0, 20f, 4f * Renderer.separation),
										new CheckButton(true, 0, 0, 110f, 4f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor),
										new Field("Cloud color:", "CCCCCC", 0, 0, 140f, 3f * Renderer.separation, 120f, 6, WritingType.Hex),
										new Field("Dot size:", "3", 0, 0, 140f, 2f * Renderer.separation, 120f, 5, WritingType.FloatPositive),
										new Field("Background color:", "08081A", 0, 0, 140f, 1f * Renderer.separation, 120f, 6, WritingType.Hex),
										new Field("World border color:", "75D7FF", 0, 0, 140f, 0f, 120f, 6, WritingType.Hex),
										new Text("Vsync:", 0, 0, 20f, -1f * Renderer.separation),
										new CheckButton(true, 0, 0, 110f, -1f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor),
										new Field("Targer framerate:", "144", 0, 0, 140f, -2f * Renderer.separation, 120f, 4, WritingType.Int).setDescription("Vsync overrites it"),
										new ImageBackButton("next", 1, 0, 10f, 0f, 45f, 45f, Renderer.textColor, Renderer.buttonColor).setAction(() => ren.setScreen(optionsScreen2)),
										new TextButton("Reset", 0, -1, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(resetConfig),
										new TextButton("Save", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(saveConfig),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setWriting().setErrorText(optionsErrorText);
										
		optionsScreen2 = new Screen(new Text("Options", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Field("Max particles:", "1000", 0, 0, 140f, 3f * Renderer.separation, 120f, 5, WritingType.Int).setDescription("Might reduce performance"),
										new Field("Save path:", "", 0, 0, 40f, 2f * Renderer.separation, 300f, 35, WritingType.String).setDescription("Leave empty for default"),
										new Text("Multithreading:", 0, 0, 30f, 1f * Renderer.separation),
										new CheckButton(true, 0, 0, 170f, 1f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor).setDescription("May be undeterministic"),
										new Text("Collision multithreading:", 0, 0, -40f, 0f),
										new CheckButton(false, 0, 0, 170f, 0f, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor).setDescription("Could be unstable"),
										new TextButton("Controls", 0, 0, 0f, -1f * Renderer.separation, 300f, Renderer.buttonColor).setAction(() => ren.setScreen(controlScreen1)),
										new ImageBackButton("previous", -1, 0, 10f, 0f, 45f, 45f, Renderer.textColor, Renderer.buttonColor).setAction(() => ren.setScreen(optionsScreen1)),
										new TextButton("Reset", 0, -1, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(resetConfig),
										new TextButton("Save", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(saveConfig),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setWriting().setErrorText(optionsErrorText);
										
		sceneConfigScreen = new Screen(new Text("Scene Config", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new Field("Scene name:", generateRandomName(), 0, 0, 120f, 2f * Renderer.separation, 200f, 20, WritingType.String),
										new Text("World border:", 0, 0, 0f, 1f * Renderer.separation),
										new CheckButton(false, 0, 0, 120f, 1f * Renderer.separation, Renderer.textSize.Y + 10f, Renderer.textSize.Y + 10f, Renderer.buttonColor),
										new Field("Border X size:", "1000", 0, 0, 150f, 0f, 135f, 8, WritingType.FloatPositive),
										new Field("Border Y size:", "1000", 0, 0, 150f, -1f * Renderer.separation, 135f, 8, WritingType.FloatPositive),
										new TextButton("Reset", 0, -1, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(resetSceneConfig),
										new TextButton("Save", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(updateSceneConfig),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setWriting().setErrorText(new Text("", 0, 1, 0f, 60f, Renderer.redTextColor));
										
		dropScreen = new Screen(new Text("Drop the file into the window", 0, 0, 0f, 0f, Renderer.selectedTextColor)
										#if WINDOWS
											,new ImageBackButton("file", 0, 0, 0f, -60f, 45f, 45f, Renderer.textColor, new Color3("CCB64B")).setDescription("Choose file").setAction(openFileDialog)
										#endif
										);
		
		Text controlErrorText = new Text("", 0, 1, 0f, 60f, Renderer.redTextColor);
		
		controlScreen1 = new Screen(new Text("Controls", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new KeyField("Fullscreen:", fullscreen, 0, 0, -70f, 3f * Renderer.separation),
										new KeyField("Screenshot:", screenshot, 0, 0, -70f, 2f * Renderer.separation),
										new KeyField("Advanced mode:", advancedMode, 0, 0, -70f, 1f * Renderer.separation),
										new KeyField("Show forces:", showForces, 0, 0, -70f, 0f),
										new KeyField("Show points:", showPoints, 0, 0, -70f, -1f * Renderer.separation),
										new KeyField("Show boxes:", showBoxes, 0, 0, -70f, -2f * Renderer.separation),
										new KeyField("Move up:", moveUp, 0, 0, 220, 2f * Renderer.separation),
										new KeyField("Move left:", moveLeft, 0, 0, 220, 1f * Renderer.separation),
										new KeyField("Move right:", moveRight, 0, 0, 220, 0f),
										new KeyField("Move down:", moveDown, 0, 0, 220, -1f * Renderer.separation),
										new ImageBackButton("next", 1, 0, 10f, 0f, 45f, 45f, Renderer.textColor, Renderer.buttonColor).setAction(() => ren.setScreen(controlScreen2)),
										new TextButton("Reset", 0, -1, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(resetControls),
										new TextButton("Done", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(doCheckControls),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setKeySelecting().setErrorText(controlErrorText).setCloseAction(saveControls);
										
		controlScreen2 = new Screen(new Text("Controls", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new KeyField("Tick:", tickForward, 0, 0, -70f, 2f * Renderer.separation),
										new KeyField("Pause:", pause, 0, 0, -70f, 1f * Renderer.separation),
										new KeyField("TPS up:", tickRateUpKey, 0, 0, -70f, 0f),
										new KeyField("TPS down:", tickRateDownKey, 0, 0, -70f, -1f * Renderer.separation),
										new KeyField("Max TPS:", runAtMax, 0, 0, -70f, -2f * Renderer.separation),
										new KeyField("Start selection:", startSelection, 0, 0, 220, 2f * Renderer.separation),
										new KeyField("Quick add:", quickAdd, 0, 0, 220, 1f * Renderer.separation),
										new KeyField("Remove:", remove, 0, 0, 220, 0f),
										new KeyField("Duplicate:", duplicate, 0, 0, 220, -1f * Renderer.separation),
										new KeyField("Follow next:", nextParticle, 0, 0, 220, -2f * Renderer.separation),
										new ImageBackButton("previous", -1, 0, 10f, 0f, 45f, 45f, Renderer.textColor, Renderer.buttonColor).setAction(() => ren.setScreen(controlScreen1)),
										new TextButton("Reset", 0, -1, 0f, 3f * Renderer.separation, 300f, Renderer.buttonColor).setAction(resetControls),
										new TextButton("Done", 0, -1, 0f, 2f * Renderer.separation, 300f, Renderer.greenButtonColor).setAction(doCheckControls),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent))
										.setKeySelecting().setErrorText(controlErrorText).setCloseAction(saveControls);
										
		pauseMenuScreen = new Screen(new Text("Pause Menu", 0, 1, 0f, 20f, Renderer.titleTextColor),
										new TextButton("Close", 0, -1, 0f, 1f * Renderer.separation, 300f, Renderer.redButtonColor).setAction(closeCurrent),
										new TextButton("Options", 0, 0, 0f, 1f * Renderer.separation, 300f, Renderer.buttonColor).setAction(() => ren.setScreen(optionsScreen1)),
										new TextButton("New Simulation", 0, 0, 0f, 0f, 300f, Renderer.buttonColor).setAction(() => ren.setScreen(newSimulationScreen)),
										new TextButton("Help", 0, 0, 0f, -1f * Renderer.separation, 300f, Renderer.buttonColor).setAction(() => ren.setScreen(helpScreen)),
										new TextButton("Info", 0, 0, 0f, -2f * Renderer.separation, 300f, Renderer.buttonColor).setAction(() => ren.setScreen(infoScreen)),
										new ImageButton("screenshot", 1, 0, 10f, 0f, 35f, 35f, Renderer.textColor).setDescription("Take screenshot").setAction(takeScreenshotButton),
										new ImageButton("load", -1, 0, 10f, 15f, 30f, 30f, Renderer.textColor).setDescription("Load scene").setAction(() => ren.setScreen(dropScreen)),
										new ImageButton("save", -1, 0, 10f, -15f, 30f, 30f, Renderer.textColor).setDescription("Save scene").setAction(saveScene),
										new ImageButton("config", -1, 0, 10f, -90f, 30f, 30f, Renderer.textColor).setDescription("Scene config").setAction(() => ren.setScreen(sceneConfigScreen)),
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
		dep.config.SetCamp("vsync", true);
		dep.config.SetCamp("maxFps", 144);
		dep.config.SetCamp("wbColor", new Color3(117, 215, 255));
		dep.config.SetCamp("cloudsColor", new Color3(204, 204, 204));
		dep.config.SetCamp("savePath", "");
		dep.config.SetCamp("multithread", true);
		dep.config.SetCamp("colMultithread", false);
		
		ren.setBgColor(dep.config.GetCamp<Color3>("bgColor"));
		if(dep.config.GetCamp<bool>("clouds") != ren.modes[0].active){
			ren.modes[0].toggleActivation();
		}
		((PointRenderMode) ren.modes[3]).setPointSize(dep.config.GetCamp<float>("dotSize"));
		setVsync(dep.config.GetCamp<bool>("vsync"));
		maxFps = dep.config.GetCamp<int>("maxFps");
		((BorderRenderMode) ren.modes[1]).setColor(dep.config.GetCamp<Color3>("wbColor"));
		((BackgroundRenderMode) ren.modes[0]).setColor(dep.config.GetCamp<Color3>("cloudsColor"));
		
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
		
		//dep.config.Save();
	}
	
	void saveConfig(){		
		float d;
		if(!float.TryParse(((Field) optionsScreen1.buttons[4]).text, out d)){
			optionsScreen1.showError(ren, "Couldnt parse dot size");
			return;
		}
		
		Color3 b;
		if(!Color3.TryParse(((Field) optionsScreen1.buttons[5]).text, out b)){
			optionsScreen1.showError(ren, "Couldnt parse background color");
			return;
		}
		
		int f;
		if(!int.TryParse(((Field) optionsScreen1.buttons[9]).text, out f)){
			optionsScreen1.showError(ren, "Couldnt parse target framerate");
			return;
		}
		
		Color3 w;
		if(!Color3.TryParse(((Field) optionsScreen1.buttons[6]).text, out w)){
			optionsScreen1.showError(ren, "Couldnt parse border color");
			return;
		}
		
		Color3 c;
		if(!Color3.TryParse(((Field) optionsScreen1.buttons[3]).text, out c)){
			optionsScreen1.showError(ren, "Couldnt parse cloud color");
			return;
		}
		
		int m;
		if(!int.TryParse(((Field) optionsScreen2.buttons[1]).text, out m)){
			optionsScreen2.showError(ren, "Couldnt parse max particles");
			return;
		}
		
		dep.config.SetCamp("clouds", ((CheckButton) optionsScreen1.buttons[2]).on);
		dep.config.SetCamp("dotSize", d);
		dep.config.SetCamp("bgColor", b);
		dep.config.SetCamp("vsync", ((CheckButton) optionsScreen1.buttons[8]).on);
		dep.config.SetCamp("maxFps", f);
		dep.config.SetCamp("wbColor", w);
		dep.config.SetCamp("bgColor", b);
		dep.config.SetCamp("cloudsColor", c);
		
		dep.config.SetCamp("savePath", ((Field) optionsScreen2.buttons[2]).text);
		dep.config.SetCamp("maxParticles", m);
		dep.config.SetCamp("multithread", ((CheckButton) optionsScreen2.buttons[4]).on);
		dep.config.SetCamp("colMultithread", ((CheckButton) optionsScreen2.buttons[6]).on);
		
		ren.setBgColor(b);
		if(dep.config.GetCamp<bool>("clouds") != ren.modes[0].active){
			ren.modes[0].toggleActivation();
		}
		((PointRenderMode) ren.modes[3]).setPointSize(d);
		setVsync(dep.config.GetCamp<bool>("vsync"));
		maxFps = f;
		((BorderRenderMode) ren.modes[1]).setColor(w);
		((BackgroundRenderMode) ren.modes[0]).setColor(c);
		
		if(Simulation.maxParticles != m){
			optionsScreen1.showError(ren, "Some changes will apply when unisim restarts");
			ren.setCornerInfo("Config saved", Renderer.selectedTextColor);
		}else{
			optionsScreen1.showError(ren, "");
			ren.setCornerInfo("Config saved", Renderer.selectedTextColor);
			closeCurrent();
		}
		
		Simulation.multiThreading = dep.config.GetCamp<bool>("multithread");
		Simulation.collisionsMultiThreading = dep.config.GetCamp<bool>("colMultithread");
		
		dep.config.Save();
	}
	
	void resetSceneConfig(){
		((CheckButton) sceneConfigScreen.buttons[3]).on = false;
		((Field) sceneConfigScreen.buttons[4]).text = "1000";
		((Field) sceneConfigScreen.buttons[5]).text = "1000";
		//sim.setWorldBorder(null);
	}
	
	void updateSceneConfig(){
		sim.sceneName = ((Field) sceneConfigScreen.buttons[1]).text;
		
		if(!((CheckButton) sceneConfigScreen.buttons[3]).on){
			sim.setWorldBorder(null);
			ren.setCornerInfo("Scene updated", Renderer.selectedTextColor);
			closeCurrent();
			return;
		}
		
		float x;
		if(!float.TryParse(((Field) sceneConfigScreen.buttons[4]).text, out x)){
			sceneConfigScreen.showError(ren, "Couldnt parse x size");
			return;
		}
		
		float y;
		if(!float.TryParse(((Field) sceneConfigScreen.buttons[5]).text, out y)){
			sceneConfigScreen.showError(ren, "Couldnt parse y size");
			return;
		}
		
		sim.setWorldBorder(new WorldBorder(x, y));
		
		ren.setCornerInfo("Scene updated", Renderer.selectedTextColor);
		closeCurrent();
	}
	
	#if WINDOWS
		void openFileDialog(){
			Thread thread = new Thread(() => {
			using(OpenFileDialog openFileDialog = new OpenFileDialog()){
				openFileDialog.Title = "Select a file";
				openFileDialog.Filter = "All Files|*.*";
				
				if(openFileDialog.ShowDialog() == DialogResult.OK){
					if(File.Exists(openFileDialog.FileName)){
						Task.Run(() => loadScene(openFileDialog.FileName));
					}else{
						ren.setCornerInfo("The file was not found");
					}
					closeCurrent();
				}
			}});
			
			thread.SetApartmentState(ApartmentState.STA); // Required for OpenFileDialog
			thread.Start();
			thread.Join(); // Wait for the dialog to close before continuing
		}
	#endif
	
	void saveScene(){
		Task.Run(() => saveSceneTask(sim.getSceneForSaving()));
	}
	
	async Task saveSceneTask(Scene sce){
		ren.setCornerInfo("Saving scene...");
		AshFile af = FileConverter.getFile(sce);
		string attemptedPath = dep.config.GetCamp<string>("savePath");
		if(attemptedPath != "" && Directory.Exists(attemptedPath)){
			string n = attemptedPath + "/" + af.GetCampOrDefault("name", generateRandomName()) + ".unisim";
			af.Save(n);
			#if WINDOWS
				Process.Start("explorer.exe", "/select,\"" + System.IO.Path.GetFullPath(n) + "\"");
			#endif
		}else{
			string n = "saves/" + af.GetCampOrDefault("name", generateRandomName()) + ".unisim";
			dep.SaveAshFile(n, af);
			#if WINDOWS
				Process.Start("explorer.exe", "/select,\"" + System.IO.Path.GetFullPath(dep.path + "/" + n) + "\"");
			#endif
		}
		
		ren.setCornerInfo("Scene saved", Renderer.selectedTextColor);
		closeCurrent();
	}
	
	void resetControls(){
		fullscreen.update(Keys.F11);
		screenshot.update(Keys.F2);
		
		advancedMode.update(Keys.LeftAlt);
		
		showForces.update(Keys.F3);
		showPoints.update(Keys.F5);
		showBoxes.update(Keys.F4);
		
		tickForward.update(Keys.F);
		pause.update(Keys.Space);
		
		tickRateUpKey.update(Keys.KeyPadAdd);
		tickRateDownKey.update(Keys.KeyPadSubtract);
		runAtMax.update(Keys.M);
		
		nextParticle.update(Keys.Tab);
		
		quickAdd.update(Keys.E);
		remove.update(Keys.R);
		duplicate.update(Keys.V);
		
		startSelection.update(Keys.L);
		
		moveUp.update(Keys.W);
		moveDown.update(Keys.S);
		moveLeft.update(Keys.A);
		moveRight.update(Keys.D);
	}
	
	void doCheckControls(){
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
		
		HashSet<Keys> seen = new HashSet<Keys>();
		
		foreach(Keys key in k){
			if(!seen.Add(key)){
				controlScreen1.showError(ren, "Conflict found");
				return;
			}
		}
		
		controlScreen1.showError(ren, "");
		
		doSaveControls();
		closeCurrent();
	}
	
	bool checkControls(){
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
		
		HashSet<Keys> seen = new HashSet<Keys>();
		
		foreach(Keys key in k){
			if(!seen.Add(key)){
				controlScreen1.showError(ren, "Conflict found");
				ren.setCornerInfo("Conflict found in controls", Renderer.redTextColor);
				return false;
			}
		}
		
		controlScreen1.showError(ren, "");
		
		return true;
	}
	
	void doSaveControls(){
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
		
		dep.config.SetCamp("controls", ka);
		dep.config.Save();
		
		ren.setCornerInfo("Controls saved", Renderer.selectedTextColor);
	}
	
	void saveControls(){
		if(!checkControls()){
			return;
		}
		
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
		
		dep.config.SetCamp("controls", ka);
		dep.config.Save();
		
		ren.setCornerInfo("Controls saved", Renderer.selectedTextColor);
	}
	
	void closeCurrent(){
		ren.setScreen(null);
	}
	
	void takeScreenshotButton(){
		closeCurrent();
		takeScreenshotNextTick = true;
	}
	
	static string generateRandomName(int length = 16){
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		Random random = new Random();
		return new string(Enumerable.Range(0, length)
			.Select(_ => chars[random.Next(chars.Length)]).ToArray());
	}
	
	void setSimulation(Scene sce){
		if(sim.isRunning){
			return;
		}
		
		ren.cam.setFollow(null);
		ren.cam.position = new Vector2d(0d, 0d);
		ren.cam.resetZoom();
		ren.cam.updateForce();
		
		sim.reset(sce);
		updateSceneConfigScreen();
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
			ren.setCornerInfo("Couldnt parse color", Renderer.redTextColor);
			return;
		}
		
		float mass;
		if(!float.TryParse(((Field)addParticleScreen.buttons[3]).text, out mass)){
			addParticleScreen.showError(ren, "Couldnt parse mass");
			ren.setCornerInfo("Couldnt parse mass", Renderer.redTextColor);
			return;
		}
		
		if(mass <= 0f){
			addParticleScreen.showError(ren, "Mass must be positive");
			ren.setCornerInfo("Mass must be positive", Renderer.redTextColor);
			return;
		}
		
		float radius;
		if(!float.TryParse(((Field)addParticleScreen.buttons[4]).text, out radius)){
			addParticleScreen.showError(ren, "Couldnt parse radius");
			ren.setCornerInfo("Couldnt parse radius", Renderer.redTextColor);
			return;
		}
		
		if(radius <= 0f){
			addParticleScreen.showError(ren, "Radius must be positive");
			ren.setCornerInfo("Radius must be positive", Renderer.redTextColor);
			return;
		}
		
		float charge;
		if(!float.TryParse(((Field)addParticleScreen.buttons[5]).text, out charge)){
			addParticleScreen.showError(ren, "Couldnt parse charge");
			ren.setCornerInfo("Couldnt parse charge", Renderer.redTextColor);
			return;
		}
		
		float weak;
		if(!float.TryParse(((Field)addParticleScreen.buttons[6]).text, out weak)){
			addParticleScreen.showError(ren, "Couldnt parse weak");
			ren.setCornerInfo("Couldnt parse weak", Renderer.redTextColor);
			return;
		}
		
		addParticleScreen.showError(ren, "");
		setGhost(new Particle(radius, mass, charge, weak, c).setName(name));
		ren.currentScreen = null;
	}
	
	public void setAddParticleScreen(){
		ren.setScreen(addParticleScreen);
	}
}