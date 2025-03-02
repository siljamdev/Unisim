using System;
using System.Collections.Concurrent;
using OpenTK;
using OpenTK.Mathematics;
using AshLib.Time;

class Simulation{
	List<Particle> particles;
	List<Particle> particlesToAdd;
	List<Particle> particlesToRemove;
	
	public WorldBorder wb{get; private set;}
	
	public bool isRunning{get; private set;}
	bool stopFlag;
	
	public DeltaHelper th{get; private set;}
	
	public float targetTPS = 100f;
	public bool runAtMax;
	
	public int tickCounter{get; private set;}
	
	const double elementalRepulsionContant = 2.34d;
	const double electricalConstant = 8.6d;
	const double weakConstant = 0.02d;
	
	public static int maxParticles = 1000;
	
	public bool changeForceMode = true;
	
	public static bool multiThreading = true;
	public static bool collisionsMultiThreading = false;
	
	DrawBufferController dbc;
	
	public string? sceneName;
	
	public Simulation(Scene sce, DrawBufferController d){
		dbc = d;
		particles = new List<Particle>(sce.particles);
		
		wb = sce.wb;
		sceneName = sce.name;
		
		particlesToAdd = new List<Particle>();
		particlesToRemove = new List<Particle>();
		
		//tt = new TimeTool(1000, "Add New Particles", "Reset Particles", "Calculate Gravity", "Calculate Charges 1", "Calculate Charges 2", "Calculate Weak", "Calculate Repulsion", "Update Velocity", "Find Collisions", "Resolve Collisions", "Check errors in Collisions", "Generate DrawBuffers");
		
		//tick();
	}
	
	public void reset(Scene sce){
		if(isRunning){
			stop();
		}
		particles.Clear();
		particles.AddRange(sce.particles);
		
		wb = sce.wb;
		sceneName = sce.name;
		
		particlesToAdd.Clear();
		particlesToRemove.Clear();
		
		tickCounter = 0;
		//tt = new TimeTool(1000, "Add New Particles", "Reset Particles", "Calculate Gravity", "Calculate Charges 1", "Calculate Charges 2", "Calculate Weak", "Calculate Repulsion", "Update Velocity", "Find Collisions", "Resolve Collisions", "Check errors in Collisions", "Generate DrawBuffers");
		
		tick();
	}
	
	public static void setMaxParticles(int m){
		maxParticles = m;
		RenderMode.maxParticles = m;
	}
	
	public void addParticle(Particle p){
		if(particles.Count >= maxParticles){
			return;
		}
		if(wb != null && !(wb.box % p.position)){
			return;
		}
		if(!isRunning){
			particles.Add(p);
			generate();
		}else{
			particlesToAdd.Add(p);
		}
	}
	
	public void addParticle(List<Particle> par){
		if(particles.Count + par.Count > maxParticles){
			return;
		}
		if(!isRunning){
			par.ForEach(p => {
				if(wb == null || (wb != null && wb.box % p.position)){
					particles.Add(p);
				}
			});
			generate();
		}else{
			par.ForEach(p => {
				if(wb == null || (wb != null && wb.box % p.position)){
					particlesToAdd.Add(p);
				}
			});
		}
	}
	
	public void removeParticle(Particle p){
		if(!isRunning){
			particles.Remove(p);
			generate();
		}else{
			particlesToRemove.Add(p);
		}
	}
	
	public void removeParticle(List<Particle> par){
		if(!isRunning){
			foreach(Particle p in par){
				particles.Remove(p);
			}
			generate();
		}else{
			particlesToRemove.AddRange(par);
		}
	}
	
	public void setWorldBorder(WorldBorder w){
		wb = w;
		if(wb != null){
			for(int i = 0; i < particles.Count; i++){
				if(!wb.contains(particles[i].position + particles[i].velocity)){
					particles.RemoveAt(i);
					i--;
				}
			}
			generate();
		}
	}
	
	public Scene getSceneForSaving(){
		return new Scene(getParticlesForSaving(), wb, sceneName);
	}
	
	List<Particle> getParticlesForSaving(){
		List<Particle> par = particles.ToList();
		par.AddRange(particlesToAdd);
		
		foreach(Particle p in particlesToRemove){
			par.Remove(p);
		}
		
		return par;
	}
	
	public int numberOfParticles(){
		return particles.Count;
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
	
	public void doOneTick(){
		isRunning = true;
		tick();
		isRunning = false;
	}
	
	public void tick(){		
		tickCounter++;
		
		if(changeForceMode != Particle.singleForce){
			Particle.singleForce = changeForceMode;
		}
		
		if(particlesToAdd.Count > 0){
			particles.AddRange(particlesToAdd.ToArray());
			particlesToAdd.Clear();
		}
		
		if(particlesToRemove.Count > 0){
			foreach(Particle p in particlesToRemove){
				particles.Remove(p);
			}
			particlesToRemove.Clear();
		}
		
		if(multiThreading){
			Parallel.For(0, particles.Count, i =>{
				particles[i].reset();
			});
		}else{
			for(int i = 0; i < particles.Count; i++){
				particles[i].reset();
			}	
		}
		
		if(wb != null){
			ensureBorderSafety();
		}
		
		if(multiThreading){
			Parallel.For(0, particles.Count - 1, i =>{
				for(int j = i + 1; j < particles.Count; j++){
					computeForces(particles[i], particles[j]);
				}
			});
		}else{
			for(int i = 0; i < particles.Count - 1; i++){
				for(int j = i + 1; j < particles.Count; j++){
					computeForces(particles[i], particles[j]);
				}
			}
		}
		
		if(multiThreading){
			Parallel.For(0, particles.Count, i =>{
				particles[i].updateVelocity();
			});
		}else{
			for(int i = 0; i < particles.Count; i++){
				particles[i].updateVelocity();
			}
		}
		
		doCollisions();
		
		generate();
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
			
			if(p1.charge != 0d && p2.charge != 0d){
				//Electrical force
				
				double charge = p1.charge * p2.charge;
				
				mod = electricalConstant * charge / d;
				
				mod += (8d * Math.Abs(charge) * radSum * radSum * radSum) / (radSum * d * dist);
				
				p1.addForce(-dir, mod);
				p2.addForce(dir, mod);
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
			}
		}else{
			//Elemental repulsion
			
			double mod = elementalRepulsionContant * (radSum - dist) * (radSum - dist);
			double totalMass = p1.mass + p2.mass;
			
			Vector2d dir = Vector2d.Normalize(p1.position - p2.position);
			
			p1.addForce(dir, mod * (p2.mass / totalMass));
			p2.addForce(-dir, mod * (p1.mass / totalMass));
		}
	}
	
	void doCollisions(){
		
		ConcurrentBag<(Collision, int, int)> collisionsFound = new ConcurrentBag<(Collision, int, int)>();
		int? pendingA = null;
		int? pendingB = null;
		
		if(collisionsMultiThreading){
			Parallel.For(0, particles.Count, i =>{
				for(int j = i + 1; j < particles.Count; j++){
					if(Particle.checkCollision(particles[i], particles[j], out Collision t)){
						collisionsFound.Add((t, i, j));
					}
				}
				
				if(wb != null && Particle.checkCollision(particles[i], wb, out Collision c)){
					collisionsFound.Add((c, i, -1));
				}
			});
		}else{
			for(int i = 0; i < particles.Count; i++){
				for(int j = i + 1; j < particles.Count; j++){
					if(Particle.checkCollision(particles[i], particles[j], out Collision t)){
						collisionsFound.Add((t, i, j));
					}
				}
				
				if(wb != null && Particle.checkCollision(particles[i], wb, out Collision c)){
					collisionsFound.Add((c, i, -1));
				}
			}
		}
		
		if(collisionsFound.Count > 0){			
			(Collision, int, int) minCollision = collisionsFound.MinBy(c => c.Item1.t);
			
			if(minCollision.Item3 == -1){
				Particle.resolveCollision(particles[minCollision.Item2], wb, minCollision.Item1);
				
				pendingA = minCollision.Item2;
				pendingB = -1;
				
				IEnumerable<(Collision, int, int)> filtered = collisionsFound
				.Where(c => c.Item2 != pendingA && c.Item3 != pendingA);
				
				collisionsFound = new ConcurrentBag<(Collision, int, int)>(filtered);
			}else{
				Particle.resolveCollision(particles[minCollision.Item2], particles[minCollision.Item3], minCollision.Item1);
				
				pendingA = minCollision.Item2;
				pendingB = minCollision.Item3;
				
				IEnumerable<(Collision, int, int)> filtered = collisionsFound
				.Where(c => c.Item2 != pendingA && c.Item2 != pendingB && c.Item3 != pendingA && c.Item3 != pendingB);
				
				collisionsFound = new ConcurrentBag<(Collision, int, int)>(filtered);
			}
		}
		
		while((pendingA != null && pendingB != null) || collisionsFound.Count > 1){
			
			if(collisionsMultiThreading){
				Parallel.For(0, particles.Count, j => {
					if(j == pendingA){
						return;
					}
					
					if(Particle.checkCollision(particles[(int) pendingA], particles[j], out Collision t)){
						collisionsFound.Add((t, (int) pendingA, j));
					}
				});
			}else{
				for(int j = 0; j < particles.Count; j++){
					if(j == pendingA){
						continue;
					}
					
					if(Particle.checkCollision(particles[(int) pendingA], particles[j], out Collision t)){
						collisionsFound.Add((t, (int) pendingA, j));
					}
				}
			}
			
			
			if(wb != null && Particle.checkCollision(particles[(int) pendingA], wb, out Collision c1)){
				collisionsFound.Add((c1, (int) pendingA, -1));
			}
			
			if(pendingB != -1){
				if(collisionsMultiThreading){
					Parallel.For(0, particles.Count, j => {
						if(j == pendingB || j == pendingA){
							return;
						}
						
						if(Particle.checkCollision(particles[(int) pendingB], particles[j], out Collision t)){
							collisionsFound.Add((t, (int) pendingB, j));
						}
					});
				}else{
					for(int j = 0; j < particles.Count; j++){
						if(j == pendingB || j == pendingA){
							continue;
						}
						
						if(Particle.checkCollision(particles[(int) pendingB], particles[j], out Collision t)){
							collisionsFound.Add((t, (int) pendingB, j));
						}
					}
				}
				
				
				if(wb != null && Particle.checkCollision(particles[(int) pendingB], wb, out Collision c2)){
					collisionsFound.Add((c2, (int) pendingB, -1));
				}
			}
			
			pendingA = null;
			pendingB = null;
			
			if(collisionsFound.Count > 0){
				(Collision, int, int) minCollision = collisionsFound.MinBy(c => c.Item1.t);
				
				if(minCollision.Item3 == -1){
					Particle.resolveCollision(particles[minCollision.Item2], wb, minCollision.Item1);
					
					pendingA = minCollision.Item2;
					pendingB = -1;
					
					IEnumerable<(Collision, int, int)> filtered = collisionsFound
					.Where(c => c.Item2 != pendingA && c.Item3 != pendingA);
					
					collisionsFound = new ConcurrentBag<(Collision, int, int)>(filtered);
				}else{
					Particle.resolveCollision(particles[minCollision.Item2], particles[minCollision.Item3], minCollision.Item1);
					
					pendingA = minCollision.Item2;
					pendingB = minCollision.Item3;
					
					IEnumerable<(Collision, int, int)> filtered = collisionsFound
					.Where(c => c.Item2 != pendingA && c.Item2 != pendingB && c.Item3 != pendingA && c.Item3 != pendingB);
					
					collisionsFound = new ConcurrentBag<(Collision, int, int)>(filtered);
				}
			}			
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
	}
	
	void ensureBorderSafety(){
		if(multiThreading){
			ConcurrentBag<int> rem = new ConcurrentBag<int>();
			Parallel.For(0, particles.Count, i => {
				if(particles[i].position.X > wb.size.X + particles[i].radius || particles[i].position.X < -wb.size.X - particles[i].radius || particles[i].position.Y > wb.size.Y + particles[i].radius || particles[i].position.Y < -wb.size.Y - particles[i].radius){
					rem.Add(i);
					return;
				}else if(particles[i].position.X > wb.size.X - particles[i].radius || particles[i].position.X < -wb.size.X + particles[i].radius || particles[i].position.Y > wb.size.Y - particles[i].radius || particles[i].position.Y < -wb.size.Y + particles[i].radius){
					particles[i].setPos(clamp(particles[i].position.X, -wb.size.X + particles[i].radius, wb.size.X - particles[i].radius), clamp(particles[i].position.Y, -wb.size.Y + particles[i].radius, wb.size.Y - particles[i].radius));
				}
			});
			
			int c = 0;
			foreach(int i in rem){
				particles.RemoveAt(i - c);
				c++;
			}
		}else{
			for(int i = 0; i < particles.Count; i++){
				if(particles[i].position.X > wb.size.X + particles[i].radius || particles[i].position.X < -wb.size.X - particles[i].radius || particles[i].position.Y > wb.size.Y + particles[i].radius || particles[i].position.Y < -wb.size.Y - particles[i].radius){
					particles.RemoveAt(i);
					i--;
					continue;
				}else if(particles[i].position.X > wb.size.X - particles[i].radius || particles[i].position.X < -wb.size.X + particles[i].radius || particles[i].position.Y > wb.size.Y - particles[i].radius || particles[i].position.Y < -wb.size.Y + particles[i].radius){
					particles[i].setPos(clamp(particles[i].position.X, -wb.size.X + particles[i].radius, wb.size.X - particles[i].radius), clamp(particles[i].position.Y, -wb.size.Y + particles[i].radius, wb.size.Y - particles[i].radius));
				}
			}
		}
	}
	
	double clamp(double x, double min, double max){
		if (x < min) return min;
		if (x > max) return max;
		return x;
	}
	
	public List<Particle> getParticlesInSelection(AABB sel){
		List<Particle> par = new List<Particle>();
		
		foreach(Particle p in particles){
			if(sel % p.position){
				par.Add(p);
			}
		}
		
		return par;
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
	
	public Particle getPreviousParticle(Particle p){
		int u = particles.IndexOf(p);
		if(u != -1 && u < 1){
			return null;
		}else if(u != -1){
			return particles[u-1];
		}
		return particles[particles.Count - 1];
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
		
		List<Task> tasks = new List<Task>();
		
		if(dbc.particles.required){
			float[] pl = new float[particles.Count * Particle.drawDataSize];
			
			if(multiThreading){
				tasks.Add(Task.Run(() => {
					Parallel.For(0, particles.Count, i => {
						Array.Copy(particles[i].drawData(), 0, pl, Particle.drawDataSize * i, Particle.drawDataSize);
					});
					p = pl;
				}));
				
			}else{
				for(int i = 0; i < particles.Count; i++){
					Array.Copy(particles[i].drawData(), 0, pl, Particle.drawDataSize * i, Particle.drawDataSize);
				}
				p = pl;
			}
		}
		
		if(dbc.velocities.required){
			List<float> vl = new List<float>(particles.Count * 2);
			
			if(multiThreading){
				tasks.Add(Task.Run(() => {
					for(int i = 0; i < particles.Count; i++){
						vl.AddRange(particles[i].velocityDrawData());
					}
					v = vl.ToArray();
				}));
			}else{
				for(int i = 0; i < particles.Count; i++){
					vl.AddRange(particles[i].velocityDrawData());
				}
				v = vl.ToArray();
			}
		}
		
		if(dbc.forces.required){
			List<float> fl = new List<float>(particles.Count);
			
			if(multiThreading){
				tasks.Add(Task.Run(() => {
					for(int i = 0; i < particles.Count; i++){
						fl.AddRange(particles[i].forcesDrawData());
					}
					f = fl.ToArray();
				}));
			}else{
				for(int i = 0; i < particles.Count; i++){
					fl.AddRange(particles[i].forcesDrawData());
				}
				f = fl.ToArray();
			}
		}
		
		if(dbc.boxes.required){
			float[] bl = new float[particles.Count * AABB.drawDataSize];
			
			if(multiThreading){
				tasks.Add(Task.Run(() => {
					Parallel.For(0, particles.Count, i => {
						Array.Copy(particles[i].boxDrawData(), 0, bl, AABB.drawDataSize * i, AABB.drawDataSize);
					});
					b = bl;
				}));
			}else{
				for(int i = 0; i < particles.Count; i++){
					Array.Copy(particles[i].boxDrawData(), 0, bl, AABB.drawDataSize * i, AABB.drawDataSize);
				}
				b = bl;
			}
		}
		
		if(dbc.collisions.required){
			List<float> cl = new List<float>(particles.Count / 2);
			
			if(multiThreading){
				tasks.Add(Task.Run(() => {
					for(int i = 0; i < particles.Count; i++){
						cl.AddRange(particles[i].collisionsDrawData());
					}
					c = cl.ToArray();
				}));
			}else{
				for(int i = 0; i < particles.Count; i++){
					cl.AddRange(particles[i].collisionsDrawData());
				}
				c = cl.ToArray();
			}
		}
		
		if(multiThreading){
			Task.WhenAll(tasks).GetAwaiter().GetResult();
		}
		
		dbc.setBuffers(p, v, f, b, c);
	}
}