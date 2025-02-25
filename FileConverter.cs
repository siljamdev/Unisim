using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;
using AshLib.AshFiles;

static class FileConverter{
	public static AshFile getFile(List<Particle> par){
		AshFile af = new AshFile();
		
		af.SetCamp("numOfParticles", par.Count);
		
		for(int i = 0; i < par.Count; i++){
			Particle p = par[i];
			
			af.SetCamp(i + ".p", convert(p.position));
			af.SetCamp(i + ".v", convert(p.velocity));
			af.SetCamp(i + ".c", p.color);
			af.SetCamp(i + ".r", p.radius);
			af.SetCamp(i + ".m", p.mass);
			af.SetCamp(i + ".e", p.charge);
			af.SetCamp(i + ".w", p.weak);
			
			if(p.name != null){
				af.SetCamp(i + ".n", p.name);
			}
		}
		
		//Console.WriteLine(af.VisualizeAsTree());
		
		return af;
	}
	
	public static List<Particle> getParticles(AshFile af){
		List<Particle> par = new List<Particle>();
		
		if(!af.CanGetCamp("numOfParticles", out int num)){
			return par;
		}
		
		for(int i = 0; i < num; i++){
			if(!af.CanGetCamp(i + ".p", out Vec2 pos)){
				continue;
			}
			if(!af.CanGetCamp(i + ".v", out Vec2 vel)){
				continue;
			}
			if(!af.CanGetCamp(i + ".c", out Color3 col)){
				continue;
			}
			if(!af.CanGetCamp(i + ".r", out double rad)){
				continue;
			}
			if(!af.CanGetCamp(i + ".m", out double mass)){
				continue;
			}
			if(!af.CanGetCamp(i + ".e", out double charge)){
				continue;
			}
			if(!af.CanGetCamp(i + ".w", out double weak)){
				continue;
			}
			if(af.CanGetCamp(i + ".n", out string nam)){
				par.Add(new Particle(convert(pos), convert(vel), rad, mass, charge, weak, col).setName(nam));
			}else{
				par.Add(new Particle(convert(pos), convert(vel), rad, mass, charge, weak, col));
			}
		}
		
		return par;
	}
	
	static Vec2 convert(Vector2d v){
		return new Vec2((float) v.X, (float) v.Y);
	}
	
	static Vector2d convert(Vec2 v){
		return new Vector2d((double) v.X, (double) v.Y);
	}
}