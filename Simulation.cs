using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Simulation{
	List<Particle> particles;
	List<Particle> particlesToAdd;
	
	public bool isRunning{get; private set;}
	bool stopFlag;
	
	public DeltaHelper th{get; private set;}
	
	public float targetTPS = 100f;
	public bool runAtMax;
	
	public int tickCounter{get; private set;}
	
	const double elementalRepulsionContant = 2.34d;
	const double electricalConstant = 8.6d;
	const double weakConstant = 0.02d;
	
	public bool changeForceMode;
	
	DrawBufferController dbc;
	
	public TimeTool tt;
	
	public Simulation(Particle[] par, DrawBufferController d){
		dbc = d;
		particles = new List<Particle>();
		particles.AddRange(par);
		
		particlesToAdd = new List<Particle>();
		
		tt = new TimeTool(1000, "Add New Particles", "Reset Particles", "Calculate Gravity", "Calculate Charges 1", "Calculate Charges 2", "Calculate Weak", "Calculate Repulsion", "Update Velocity", "Find Collisions", "Resolve Collisions", "Check errors in Collisions", "Generate DrawBuffers");
		
		//tick();
	}
	
	public void reset(Particle[] par){
		particles.Clear();
		particles.AddRange(par);
		
		tickCounter = 0;
		tt = new TimeTool(1000, "Add New Particles", "Reset Particles", "Calculate Gravity", "Calculate Charges 1", "Calculate Charges 2", "Calculate Weak", "Calculate Repulsion", "Update Velocity", "Find Collisions", "Resolve Collisions", "Check errors in Collisions", "Generate DrawBuffers");
		
		tick();
	}
	
	public void addParticle(Particle p){
		if(!isRunning){
			particles.Add(p);
		}else{
			particlesToAdd.Add(p);
		}
	}
	
	public async Task run(){
		try{
			if(isRunning){
				return;
			}
			isRunning = true;
			stopFlag = false;
			
			th = new DeltaHelper();
			th.Start();
			
			while(!stopFlag){
				tick();
				if(!runAtMax){
					th.Target(targetTPS);
				}
				th.Frame();
			}
		}catch(Exception e){
			Console.WriteLine(e);
		}finally{
			isRunning = false;
		}
	}
	
	public void stop(){
		stopFlag = true;
	}
	
	public void tick(){
		tt.tickStart();
		
		tickCounter++;
		
		if(changeForceMode){
			Particle.singleForce = !Particle.singleForce;
			changeForceMode = false;
		}
		
		if(particlesToAdd.Count > 0){
			particles.AddRange(particlesToAdd.ToArray());
			particlesToAdd.Clear();
		}
		
		tt.catEnd();
		
		for(int i = 0; i < particles.Count; i++){
			particles[i].reset();
		}
		
		tt.catEnd();
		
		for(int i = 0; i < particles.Count; i++){
			for(int j = i + 1; j < particles.Count; j++){
				computeForces(particles[i], particles[j]);
			}
		}
		
		for(int i = 0; i < particles.Count; i++){
			particles[i].updateVelocity();
		}
		
		tt.catEnd(7);
		
		doCollisions();
		
		tt.catEnd();
		
		generate();
		
		tt.catEnd();
		
		tt.tickEnd();
	}
	
	void computeForces(Particle p1, Particle p2){
		double dist = Vector2d.Distance((p1.position), (p2.position));
		double radSum = p1.radius + p2.radius;
		
		if(dist >= radSum){
			//Gravity
			double d = dist * dist;
			double mod = (p1.mass * p2.mass)/d;
			
			Vector2d dir = Vector2d.Normalize(p2.position - p1.position);
			
			p1.addForce(dir, mod);
			p2.addForce(-dir, mod);
			
			tt.catEnd(2);
			
			if(p1.charge != 0d && p2.charge != 0d){
				//Electrical force
				
				double charge = p1.charge * p2.charge;
				
				mod = electricalConstant * charge / d;
				
				tt.catEnd(3);
				
				mod += (8d * Math.Abs(charge) * radSum * radSum * radSum) / (radSum * d * dist);
				
				p1.addForce(-dir, mod);
				p2.addForce(dir, mod);
				
				tt.catEnd(4);
			}
			
			double w = p1.weak + p2.weak;
			
			if(p1.weak > 0d && p2.weak > 0d && dist > (33d * w)){
				//Weak worce
				double q = dist - 16d * w;
				q = q * q;
				mod = (-q / (dist - 32d * w)) + (64d + weakConstant) * w;
				
				if(mod > 0d){
					p1.addForce(dir, mod);
					p2.addForce(-dir, mod);
				}				
				
				tt.catEnd(5);
			}
		}else{
			//Elemental repulsion
			
			double mod = elementalRepulsionContant * (radSum - dist) * (radSum - dist);
			double totalMass = p1.mass + p2.mass;
			
			Vector2d dir = Vector2d.Normalize(p1.position - p2.position);
			
			p1.addForce(dir, mod * (p2.mass / totalMass));
			p2.addForce(-dir, mod * (p1.mass / totalMass));
			
			tt.catEnd(6);
		}
	}
	
	void doCollisions(){
		
		List<(Collision, int, int)> collisionsFound = new List<(Collision, int, int)>();
		int? pendingA = null;
		int? pendingB = null;
		
		for(int i = 0; i < particles.Count - 1; i++){
			for(int j = i + 1; j < particles.Count; j++){
				if(Particle.checkCollision(particles[i], particles[j], out Collision t)){
					collisionsFound.Add((t, i, j));
				}
			}
		}
		
		tt.catEnd(8);
		
		if(collisionsFound.Count > 0){
			collisionsFound.Sort((a, b) => a.Item1.t.CompareTo(b.Item1.t));
			
			Particle.resolveCollision(particles[collisionsFound[0].Item2], particles[collisionsFound[0].Item3], collisionsFound[0].Item1);
			pendingA = collisionsFound[0].Item2;
			pendingB = collisionsFound[0].Item3;
			collisionsFound.RemoveAt(0);
			
			collisionsFound = collisionsFound
			.Where(c => c.Item2 != pendingA && c.Item2 != pendingB && c.Item3 != pendingA && c.Item3 != pendingB)
			.ToList();
		}
		
		tt.catEnd(9);
		
		while((pendingA != null && pendingB != null) || collisionsFound.Count > 1){
			
			for(int j = 0; j < particles.Count; j++){
				if(j == pendingA){
					continue;
				}
				if(Particle.checkCollision(particles[(int) pendingA], particles[j], out Collision t)){
					collisionsFound.Add((t, (int) pendingA, j));
				}
			}
			
			for(int j = 0; j < particles.Count; j++){
				if(j == pendingB || j == pendingA){
					continue;
				}
				if(Particle.checkCollision(particles[(int) pendingB], particles[j], out Collision t)){
					collisionsFound.Add((t, (int) pendingB, j));
				}
			}
			
			tt.catEnd(8);
			
			pendingA = null;
			pendingB = null;
			
			if(collisionsFound.Count > 0){
				collisionsFound.Sort((a, b) => a.Item1.t.CompareTo(b.Item1.t));
				
				Particle.resolveCollision(particles[collisionsFound[0].Item2], particles[collisionsFound[0].Item3], collisionsFound[0].Item1);
				pendingA = collisionsFound[0].Item2;
				pendingB = collisionsFound[0].Item3;
				collisionsFound.RemoveAt(0);
				
				collisionsFound = collisionsFound
				.Where(c => c.Item2 != pendingA && c.Item2 != pendingB && c.Item3 != pendingA && c.Item3 != pendingB)
				.ToList();
			}
			
			tt.catEnd(9);			
		}
		
		//Uncomment this for checking for errors and stopping the simultion
		/* List<Vector2d> predictedPositions = new List<Vector2d>(particles.Count);
		for(int i = 0; i < particles.Count; i++){
			predictedPositions.Add(particles[i].position + particles[i].velocity);
		}
		
		for(int i = 0; i < particles.Count - 1; i++){
			for(int j = i + 1; j < particles.Count; j++){
				double dx = predictedPositions[i].X - predictedPositions[j].X;
				double dy = predictedPositions[i].Y - predictedPositions[j].Y;
				double distanceSquared = dx * dx + dy * dy;
				double radiusSum = particles[i].radius + particles[j].radius;
				double radiusSumSquared = radiusSum * radiusSum;
		
				if(distanceSquared < radiusSumSquared){
					Console.WriteLine(i + " & " + j);
					stopFlag = true;
				}
			}
		} */
		
		//tt.catEnd();
	}
	
	public Particle getNextParticle(Particle p){
		int u = particles.IndexOf(p);
		if(u != -1 && u >= particles.Count - 1){
			return null;
		}else if(u != -1){
			return particles[u+1];
		}
		return particles[0];
	}
	
	public Particle getParticleAtCursor(Vector2d cursor){
		for(int i = 0; i < particles.Count; i++){
			if(Vector2d.Distance(particles[i].position, cursor) < particles[i].radius){
				return particles[i];
			}
		}
		return null;
	}
	
	public void tryGenerate(){
		if(isRunning){
			return;
		}
		generate();
	}
	
	void generate(){
		float[] p = null;
		float[] v = null;
		float[] f = null;
		float[] b = null;
		float[] c = null;
		
		if(dbc.particles.required){
			List<float> pl = new List<float>(particles.Count * Particle.drawDataSize);
			
			for(int i = 0; i < particles.Count; i++){
				pl.AddRange(particles[i].drawData());
			}
			
			p = pl.ToArray();
		}
		
		if(dbc.velocities.required){
			List<float> vl = new List<float>(particles.Count);
			
			for(int i = 0; i < particles.Count; i++){
				vl.AddRange(particles[i].velocityDrawData());
			}
			
			v = vl.ToArray();
		}
		
		if(dbc.forces.required){
			List<float> fl = new List<float>(particles.Count);
			
			for(int i = 0; i < particles.Count; i++){
				fl.AddRange(particles[i].forcesDrawData());
			}
			
			f = fl.ToArray();
		}
		
		if(dbc.boxes.required){
			List<float> bl = new List<float>(particles.Count * AABB.drawDataSize);
			
			for(int i = 0; i < particles.Count; i++){
				bl.AddRange(particles[i].boxDrawData());
			}
			
			b = bl.ToArray();
		}
		
		if(dbc.collisions.required){
			List<float> cl = new List<float>(particles.Count);
			
			for(int i = 0; i < particles.Count; i++){
				cl.AddRange(particles[i].collisionsDrawData());
			}
			
			c = cl.ToArray();
		}
		
		
		dbc.setBuffers(p, v, f, b, c);
	}
}