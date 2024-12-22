using System;
using System.Text;
using System.Diagnostics;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

static class Examples{
	public static readonly Particle[] solarSystem = new Particle[]{
		// Sun
		new Particle(new Vector2(0f, 0f), 1090f, new Color3("FFDD00")),
		// Mercury
		new Particle(new Vector2(5790f, 0f), new Vector2(0f, -13.74f), 3.8f, new Color3("BBBBBB")),
		// Venus
		new Particle(new Vector2(10820f, 0f), new Vector2(0f, -8.50f), 9.5f, new Color3("EEC1A5")),
		// Earth
		new Particle(new Vector2(14960f, 0f), new Vector2(0f, -7.98f), 10f, new Color3("0044FF")),
		// Moon
		new Particle(new Vector2(15040f, 0f), new Vector2(0f, -6.98f), 3.2f, new Color3("777777")),
		// Mars
		new Particle(new Vector2(22790f, 0f), new Vector2(0f, -6.41f), 5.3f, new Color3("FF5533")),
		// Phobos
		new Particle(new Vector2(22825f, 0f), new Vector2(0f, -7.28f), 1.2f, new Color3("775555")),
		// Deimos
		new Particle(new Vector2(22839f, 0f), new Vector2(0f, -6.87f), 0.8f, new Color3("775555")),
		// Jupiter
		new Particle(new Vector2(77830f, 0f), new Vector2(0f, -3.31f), 112f, new Color3("CC9933")),
		// Io
		new Particle(new Vector2(78900f, 0f), new Vector2(0f, -6.41f), 6.2f, new Color3("EEFF11")),
		// Europa
		new Particle(new Vector2(79200f, 0f), new Vector2(0f, -5.9f), 5.4f, new Color3("CCDD99")),
		// Ganymedes
		new Particle(new Vector2(80030f, 0f), new Vector2(0f, -4.86f), 8.2f, new Color3("845D3C")),
		// Callisto
		new Particle(new Vector2(80900f, 0f), new Vector2(0f, -5.31f), 5.9f, new Color3("CE5D3C")),
		// Saturn
		new Particle(new Vector2(142940f, 0f), new Vector2(0f, -2.22f), 94f, new Color3("F8C34A")),
		// Titan
		new Particle(new Vector2(144040f, 0f), new Vector2(0f, -5.71f), 5.1f, new Color3("C36B3D")),
		// Uranus
		new Particle(new Vector2(287099f, 0f), new Vector2(0f, -1.62f), 40f, new Color3("55BBFF")),
		// Neptune
		new Particle(new Vector2(450430f, 0f), new Vector2(0f, -1.32f), 38f, new Color3("3333FF"))
	};
	
	public static readonly PlanetSystem solntse = new PlanetSystem(1090f, "FFDD00").setLastName("Sun")
							.addPlanet(3.8f, 5790f, "BBBBBB").setLastName("Mercury") // Mercury
							.addPlanet(9.5f, 10820f, "EEC1A5").setLastName("Venus") // Venus
							.addPlanet(10f, 14960f, "0044FF").setLastName("Earth") // Earth
								.addMoon(3.2f, 100f, "777777").setLastName("Moon") // Moon
							.addPlanet(5.8f, 22790f, "FF5533").setLastName("Mars") // Mars
								.addMoon(0.8f, 20f, "775555").setLastName("Phobos") // Phobos
								.addMoon(0.6f, 54f, "775555").setLastName("Deimos") // Deimos
							.addPlanet(112f, 77830f, "CC9933").setLastName("Jupiter") // Jupiter
								.addMoon(6.2f, 421f, "EEFF11").setLastName("Io") // Io
								.addMoon(5.4f, 670f, "CCDD99").setLastName("Europa") // Europa
								.addMoon(8.2f, 1070f, "845D3C").setLastName("Ganymedes") // Ganymedes
								.addMoon(5.9f, 1880f, "CE5D3C").setLastName("Callisto") // Callisto
							.addPlanet(94f, 142940f, "F8C34A").setLastName("Saturn") // Saturn
								.addMoon(1.5f, 300f, "D4D4D4").setLastName("Rhea") // Rhea
								.addMoon(5.1f, 1200f, "C36B3D").setLastName("Titan") // Titan
							.addPlanet(40f, 287099f, "55BBFF").setLastName("Uranus") // Uranus
								.addMoon(2f, 200f, "9F9F9F").setLastName("Miranda") // Miranda
								.addMoon(4.1f, 800f, "B6D0E8").setLastName("Titania") // Titania
							.addPlanet(38f, 450430f, "3333FF").setLastName("Neptune") // Neptune
								.addMoon(2.3f, 300f, "6D9A96").setLastName("Triton") // Triton
								.addMoon(0.7f, 1670f, "D0E4F3").setLastName("Nereid") // Nereid
							.addPlanet(1.2f, 590600f, "C0A0A0").setLastName("Pluto") // Pluto
								.addMoon(0.5f, 200f, "F090A0").setLastName("Charon"); // Charon
							
	public static readonly PlanetSystem kyra = new PlanetSystem(1200f, "FF4400").setLastName("Kyra")
							.addPlanet(8f, 11090f, "FF6B21").setLastName("Erphest")
							.addPlanet(3.6f, 18100f, "999999").setLastName("Serta")
							.addPlanet(5f, 29500f, "993049").setLastName("Blitza")
							.addPlanet(11.2f, 32300f, "973899").setLastName("Ustria")
								.addMoon(2.3f, 400f, "61993B").setLastName("Palnia")
							.addPlanet(9.2f, 36110, "4023EE").setLastName("Neptia")
								.addMoon(2.1f, 310f, "493A33").setLastName("Posdta")
							.addPlanet(4.6f, 45000f, "A05B3B").setLastName("Dulma")
							.addPlanet(96f, 72000f, "69BF74").setLastName("Septia")
								.addMoon(7f, 980f, "DD3030").setLastName("Bilna")
								.addMoon(5.2f, 2100f, "E2E2E2").setLastName("Ypra")
								.addMoon(3.8f, 2900f, "775555").setLastName("Mirnia")
							.addPlanet(62f, 109000f, "BC9C8B").setLastName("Octia")
								.addMoon(3.1f, 800f, "B00030").setLastName("Kraza")
								.addMoon(5.2f, 2000f, "9E58B7").setLastName("Crysta")
							.addPlanet(112f, 184000f, "AF85B5").setLastName("Enia")
								.addMoon(4.1f, 1300f, "C6C627").setLastName("Sulfa")
								.addMoon(2.7f, 2000f, "47C178").setLastName("Xita")
								.addMoon(1f, 2400f, "4D44BF").setLastName("Finia")
								.addMoon(8.3f, 3600f, "4994BC").setLastName("Saphia")
							.addPlanet(35f, 256000f, "2FEFDF").setLastName("Dezia")
								.addMoon(3.6f, 700f, "777777").setLastName("Urtna")
							.addPlanet(4.8f, 310000f, "CCCCCC").setLastName("Pitia")
							.addPlanet(5.6f, 340000f, "FFCCCC").setLastName("Enea");
							
	public static readonly PlanetSystem solara = new PlanetSystem(1500f, "FFCC00").setLastName("Solara")
							.addPlanet(4.5f, 4000f, "FF4500").setLastName("Inferno")
							.addPlanet(9.8f, 12000f, "FFD700").setLastName("Veona")
							.addPlanet(15f, 21000f, "8A2BE2").setLastName("Xaros")
							.addPlanet(11f, 35000f, "228B22").setLastName("Eryndor")
							.addPlanet(7.4f, 42000f, "A9A9A9").setLastName("Aether")
							.addPlanet(10f, 55000f, "DC143C").setLastName("Ares")
							.addPlanet(30.2f, 69000f, "00BFFF").setLastName("Phantoma")
							.addPlanet(120f, 90000f, "DAA520").setLastName("Jaemus")
								.addMoon(6.5f, 1000f, "F0F8FF").setLastName("Kaela")
							.addPlanet(13f, 130000f, "FF6347").setLastName("Zorath")
							.addPlanet(5.6f, 160000f, "A52A2A").setLastName("Zythre")
							.addPlanet(20f, 210000f, "B0C4DE").setLastName("Ceres");
	
	public static readonly Particle[] planet = new Particle[]{
		new Particle(new Vector2(0f, 0f), new Vector2(0f, 0f), 100f, new Color3("FFDD00")),
		new Particle(new Vector2(250f, 0f), new Vector2(0f, -8f), 10f, new Color3("00FFFF"))
	};
	
	public static readonly Particle[] test = new Particle[]{
		Particle.Tark.translate(1, -32),
		Particle.Mark.translate(0, -23),
		Particle.Tark.translate(0, 23),
		Particle.Mark.translate(2, 32)
	};
	
	public static readonly Particle[] earlySolarSystem = new Particle[]{
		new Particle(new Vector2(0f, 0f), new Vector2(0f, 0f), 1000f, new Color3("FFDD00")),
		new Particle(new Vector2(10000f, 0f), new Vector2(0f, -10f), 10f, new Color3("BBBBBB")),
		new Particle(new Vector2(16000f, 0f), new Vector2(0f, -8f), 30f, new Color3("DDBB00")),
		new Particle(new Vector2(30000f, 0f), new Vector2(0f, -6f), 32f, new Color3("0044FF")),
		new Particle(new Vector2(37000f, 0f), new Vector2(0f, -5f), 15f, new Color3("CC1100")),
		new Particle(new Vector2(50000f, 0f), new Vector2(0f, -5.5f), 130f, new Color3("DD2255")),
		new Particle(new Vector2(70000f, 0f), new Vector2(0f, -4f), 100f, new Color3("FFB6AF"))
	};
	
	public static readonly Particle[] collision = new Particle[]{
		new Particle(new Vector2(-30f, -30f), new Vector2(1f, 1f), 10f, new Color3("FFDD00")),
		new Particle(new Vector2(30f, -30f), new Vector2(-1f, 1f), 8f, new Color3("00FFFF")),
		new Particle(new Vector2(0f, -60f), new Vector2(0f, 2f), 4f, new Color3("00FF00")),
		new Particle(new Vector2(20f, 80f), new Vector2(-0.1f, -2.2f), 4f, new Color3("FF3000")),
		new Particle(new Vector2(200f, 0f), new Vector2(-6f, 0.5f), 6f, new Color3("FFFFFF"))
	};
	
	public static Particle[] RPF{
		get{			
			return RPFparams(100, 500, 100f, 0.5f, 30, 30, 30, 5, 5, true);
		}
	}
	
	public static Particle[] RPFparams(int mnum, int xnum, float size, float vel, int wb, int wm, int wt, int wc, int wp, bool ring){
		List<Particle> particles = new List<Particle>();
		Random r = new Random();
	
		int n = r.Next(mnum, xnum);
		
		int prob = wb + wm + wt + wc + wp;
	
		for (int i = 0; i < n; i++){
			Particle newParticle;
			bool isValid;
			
			int nn = r.Next(prob);
			if(nn < wc){
				newParticle = Particle.Chark;
			}else if(nn < wc + wp){
				newParticle = Particle.Phark;
			}else if(nn < wc + wp + wm){
				newParticle = Particle.Mark;
			}else if(nn < wc + wp + wm + wt){
				newParticle = Particle.Tark;
			}else{
				newParticle = Particle.Basitron;
			}
	
			do{
				isValid = true;
				float x = random(r, -size, size);
				float y = random(r, -size, size);
				
				newParticle.translate(x, y);
	
				// Check for overlap with existing particles
				foreach (var particle in particles){
					double dx = newParticle.position.X - particle.position.X;
					double dy = newParticle.position.Y - particle.position.Y;
					double distance = dx * dx + dy * dy;
					
					double radSum = newParticle.radius + particle.radius;
					
					if (distance < radSum * radSum){
						isValid = false;
						break;
					}
				}
			}while(!isValid);
			
			Vector2d v;
			if(ring){
				v = vel * Vector2d.Normalize(newParticle.position);
			}else{
				float vx = random(r, -vel, vel);
				float vy = random(r, -vel, vel);
				v = new Vector2d(vx, vy);
			}
			
			newParticle.addVelocity(v);
	
			particles.Add(newParticle);
		}
		
		return particles.ToArray();
	}

	
	static float random(Random r, float min, float max){
		return (float) r.NextDouble() * (max - min) + min;
	}
}