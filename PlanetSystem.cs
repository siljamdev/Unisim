using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class PlanetSystem{
	List<Particle> bodies;
	
	double starMass;
	
	int lastPlanet;
	
	public PlanetSystem(float starRadius, string starColor){
		bodies = new List<Particle>();
		bodies.Add(new Particle(new Vector2(0f, 0f), starRadius, new Color3(starColor)));
		starMass = bodies[0].mass;
	}
	
	public PlanetSystem(float starRadius, float sm, string starColor){
		bodies = new List<Particle>();
		bodies.Add(new Particle(new Vector2(0f, 0f), new Vector2(0f, 0f), starRadius, sm, new Color3(starColor)));
		starMass = bodies[0].mass;
	}
	
	public static PlanetSystem Random{get{
		Random r = new Random();
		float s = random(r, 800f, 2000f);
		PlanetSystem p = new PlanetSystem(s, new Color3((byte) (150 + r.Next(106)), (byte) r.Next(256), 0).ToString());
		
		p.setLastName(generateStarName(r));
		
		int n = 1 + r.Next(12);
		float d = s * 4f;
		
		for(int i = 0; i < n; i++){
			float c = random(r, 1200f + 500f * i, 1150f * i * i + 2000f);
			d += c;
			float a = random(r, -0.24f * i * i + 2.9f * i + 3f, -2.7f * i * i + 35.7f * i + 4f);
			p.addPlanet(a, d, new Color3((byte) (20 + r.Next(235)), (byte) (r.Next(210)), (byte) (r.Next(200))).ToString());
			
			string pname = generatePlanetName(r);
			p.setLastName(pname);
			
			if(i > 3 && r.Next(n - i) == 0){
				int m = 1 + r.Next(3);
				float e = 300f;
				float g = (float) p.bodies[i + 1].radius;
				for(int j = 0; j < m; j++){
					float f = random(r, 20f * i + 100f, 100f * i + 1000f);
					e += f;
					float b = random(r, g/20f, g/4f);
					p.addMoon(b, e, new Color3((byte) (101 + r.Next(155)), (byte) (101 + r.Next(155)), (byte) (101 + r.Next(155))).ToString());
					
					p.setLastName(pname + " " + toRomanNumeral(j + 1));
				}
				d += 180f * a;
			}
		}
		
		return p;
	}}
	
	static float random(Random r, float min, float max){
		return (float) r.NextDouble() * (max - min) + min;
	}
	
	public PlanetSystem addPlanet(float radius, float distance, string color){
		float v = (float) (Math.Sqrt(starMass/distance));
		bodies.Add(new Particle(new Vector2(distance, 0f), new Vector2(0f, -v), radius, new Color3(color)));
		
		lastPlanet = bodies.Count - 2;
		
		return this;
	}
	
	public PlanetSystem addPlanet(float radius, float mass, float distance, string color){
		float v = (float) (Math.Sqrt(starMass/distance));
		bodies.Add(new Particle(new Vector2(distance, 0f), new Vector2(0f, -v), radius, mass, new Color3(color)));
		
		lastPlanet = bodies.Count - 2;
		
		return this;
	}
	
	public PlanetSystem addMoon(int body, float radius, float distance, string color){
		if(body < -1 || bodies.Count <= body + 1){
			return this;
		}
		float v = (float) Math.Sqrt(bodies[body + 1].mass/distance);
		bodies.Add(new Particle(bodies[body + 1].position + new Vector2(distance, 0f), bodies[body + 1].velocity + new Vector2(0f, -v), radius, new Color3(color)));
		
		return this;
	}
	
	public PlanetSystem addMoon(float radius, float distance, string color){
		return addMoon(lastPlanet, radius, distance, color);
	}
	
	public PlanetSystem addMoon(int body, float radius, float mass, float distance, string color){
		if(body < -1 || bodies.Count <= body + 1){
			return this;
		}
		float v = (float) Math.Sqrt(bodies[body + 1].mass/distance);
		bodies.Add(new Particle(bodies[body + 1].position + new Vector2(distance, 0f), bodies[body + 1].velocity + new Vector2(0f, -v), radius, mass, new Color3(color)));
		
		return this;
	}
	
	public PlanetSystem addMoon(float radius, float mass, float distance, string color){
		return addMoon(lastPlanet, radius, mass, distance, color);
	}
	
	private static readonly string[] prefixes = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa", "Omega", "Septa", "Hikta" };
    private static readonly string[] suffixes = { "Majoris", "Minoris", "Centauri", "Eridani", "Cygni", "Draconis", "Andromedae", "Aquarii", "Pegasi", "Serpentis", "Nexis", "Tulkis" };

    // Syllables to create original names
    private static readonly string[] syllables = { "ael", "ion", "ara", "nus", "zor", "ith", "mir", "phe", "rax", "tor", "xan", "cel", "ven", "tis", "ryn", "ora", "ban", "nam", "iet", "onis" };

    private static string generateStarName(Random r){
        string name;

        // Randomly decide whether to use a traditional prefix or go fully original
        bool useTraditional = r.Next(0, 2) == 0;

        if (useTraditional)
        {
            string prefix = prefixes[r.Next(prefixes.Length)];
            string suffix = suffixes[r.Next(suffixes.Length)];
            name = $"{prefix} {suffix}";
        }
        else
        {
            // Generate a random original name from syllables
            int syllableCount = r.Next(2, 4); // Between 2 and 3 syllables
            name = string.Empty;

            for (int i = 0; i < syllableCount; i++)
            {
                name += syllables[r.Next(syllables.Length)];
            }

            // Capitalize the first letter
            name = char.ToUpper(name[0]) + name.Substring(1);
        }

        return name;
    }
	
	private static readonly string[] descriptors = { "Prime", "Nova", "Minor", "Major", "Alpha", "Beta", "Gamma", "Zeta", "First", "Far", "Delta" };

    // Syllables for creating unique planet names
    private static readonly string[] psyllables = { "vul", "tora", "mel", "dra", "zan", "nor", "phy", "sel", "kur", "lyra", "os", "ina", "quar", "ven", "tari", "lum", "kep", "nim", "chus", "pol", "sec", "lum", "cus", "kom", "gol", "zont", "lim" };

    private static string generatePlanetName(Random r)
    {
        string name;

        // Randomly decide whether to use a descriptor or go fully original
        bool useDescriptor = r.Next(0, 2) == 0;

        if (useDescriptor)
        {
            string descriptor = descriptors[r.Next(descriptors.Length)];
            name = $"{generateBaseName(r)} {descriptor}";
        }
        else
        {
            name = generateBaseName(r);
        }

        return name;
    }
	
	private static string toRomanNumeral(int number)
	{	
		string[] romanNumerals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
		return romanNumerals[number - 1];
	}

    private static string generateBaseName(Random r){
        // Generate a random original name from syllables
        int syllableCount = r.Next(2, 4); // Between 2 and 3 syllables
        string baseName = string.Empty;

        for (int i = 0; i < syllableCount; i++)
        {
            baseName += psyllables[r.Next(psyllables.Length)];
        }

        // Capitalize the first letter
        return char.ToUpper(baseName[0]) + baseName.Substring(1);
    }
	
	public PlanetSystem setLastName(string n){
		bodies[bodies.Count - 1].setName(n);
		
		return this;
	}
	
	public Particle[] getParticles(){
		return bodies.ToArray();
	}
	
	public static implicit operator Particle[](PlanetSystem a){
        return a.getParticles();
    }
}