using System;

struct Scene{
	public List<Particle> particles{get; private set;}
	
	public WorldBorder wb{get; private set;}
	
	public string name{get; private set;}
	
	public Scene(List<Particle> p, WorldBorder w, string n){
		particles = p;
		wb = w;
		name = n;
	}
}