using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Particle{
	public Vector2d position{get; private set;}
	public Vector2d velocity{get; private set;}
	Vector2d? nextVelocity;
	List<Vector2d> forces;
	Vector2d force;
	
	public Color3 color;
	public string name;
	
	public double radius{get; private set;}
	public double mass{get; private set;}
	public double charge{get; private set;}
	public double weak{get; private set;}
	
	Trajectory trajectory;
	
	bool firstTick;
	
	const bool singleForce = true;
	const double elasticConstant = 0.9d;
	public const int drawDataSize = 6;
	
	public Particle(double rad, double m, double q, double w, Color3 col){
		radius = rad;
		color = col;
		mass = m;
		charge = q;
		weak = w;
		
		nextVelocity = null;
		
		forces = new List<Vector2d>();
		if(singleForce){
			forces.Add(new Vector2d(0d, 0d));
		}
		trajectory = new Trajectory(position, velocity, radius);
	}
	
	public Particle(Vector2d pos, double rad, Color3 col) : this(pos, new Vector2d(), rad, rad * rad, 0, 0, col){
		
	}
	
	public Particle(Vector2d pos, Vector2d vel, double rad, Color3 col) : this(pos, vel, rad, rad * rad, 0, 0, col){
		
	}
	
	public Particle(Vector2d pos, Vector2d vel, double rad, double mas, Color3 col) : this(pos, vel, rad, mas, 0, 0, col){
		
	}
	
	public Particle(Vector2d pos, Vector2d vel, double rad, double m, double q, double w, Color3 col){
		position = pos;
		radius = rad;
		color = col;
		mass = m;
		charge = q;
		weak = w;
		velocity = vel;
		
		nextVelocity = null;
		
		forces = new List<Vector2d>();
		if(singleForce){
			forces.Add(new Vector2d(0d, 0d));
		}
		trajectory = new Trajectory(position, velocity, radius);
	}
	
	public static Particle Basitron{get{
		return new Particle(1, 1, 0, 0, new Color3("FFFFFF")).setName("Basitron");
	}}
	
	public static Particle Mark{get{
		return new Particle(2, 1, 1, 1, new Color3("FF0000")).setName("Mark");
	}}
	
	public static Particle Tark{get{
		return new Particle(2, 1, -1, 1, new Color3("00FF00")).setName("Tark");
	}}
	
	public static Particle Chark{get{
		return new Particle(3, 4, 6, 0, new Color3("BF00FF")).setName("Chark");
	}}
	
	public static Particle Phark{get{
		return new Particle(3, 4, -6, 0, new Color3("FF7700")).setName("Phark");
	}}
	
	public Particle translate(Vector2d p){
		position += p;
		return this;
	}
	
	public Particle translate(double x, double y){
		position += new Vector2d(x, y);
		return this;
	}
	
	public Particle addVelocity(Vector2d p){
		velocity += p;
		return this;
	}
	
	public Particle addVelocity(double x, double y){
		velocity += new Vector2d(x, y);
		return this;
	}
	
	public Particle addRadius(double r){
		if(radius + r <= 0){
			return this;
		}
		radius += r;
		return this;
	}
	
	public Particle setMass(double m){
		mass = m;
		return this;
	}
	
	public void addForce(Vector2d direction, double modulus){
		if(singleForce){
			force += direction * modulus;
		}else{
			forces.Add(direction * modulus);
		}
	}
	
	public Particle setName(string n){
		name = n;
		return this;
	}
	
	public void updateVelocity(){		
		if(singleForce){
			velocity += force / mass;
		}else{
			Vector2d acceleration = new Vector2d(0d, 0d);
			foreach(Vector2d f in forces){
				acceleration += f;
			}
			velocity += acceleration / mass;
		}
		
		trajectory.reset(position, velocity, radius);
	}
	
	void emptyForces(){
		if(singleForce){
			force = new Vector2d(0d, 0d);
		}else{
			forces.Clear();
		}
	}
	
	public void reset(){
		if(firstTick){
			position += velocity;
			if(nextVelocity != null){
				velocity = (Vector2d) nextVelocity;
				nextVelocity = null;
			}
		}else{
			firstTick = true;
		}
		
		emptyForces();
	}
	
	public static bool checkCollision(Particle a, Particle b, out Collision outT){
		if(Trajectory.getT(a.trajectory, b.trajectory, a.radius, b.radius, out Collision t)){
			outT = t;
			return true;
		}
		outT = null;
		return false;
	}
	
	public static void resolveCollision(Particle a, Particle b, Collision c){
		double t = c.t;
		
		Vector2d pa = a.trajectory.getPos(t);
		Vector2d pb = b.trajectory.getPos(t);
		
		Vector2d n = Vector2d.Normalize(pb - pa); //normal
		
		Vector2d va = a.trajectory.getAbsVel(t);
		Vector2d vb = b.trajectory.getAbsVel(t);
		
		Vector2d van = Vector2d.Dot(va, n) * n;
		Vector2d vbn = Vector2d.Dot(vb, n) * n;
		
		Vector2d vat = va - van;
		Vector2d vbt = vb - vbn;
		
		//Vector2d vanp = (((a.mass - b.mass) * van) + ((2f * b.mass) * vbn)) / (a.mass + b.mass);
		//Vector2d vbnp = (((b.mass - a.mass) * vbn) + ((2f * a.mass) * van)) / (a.mass + b.mass);
		
		double massSum = a.mass + b.mass;
		double elasticFactor = 1d + elasticConstant;
		
		Vector2d vanp = (((a.mass - b.mass * elasticConstant) * van) + ((b.mass * vbn) * elasticFactor)) / massSum;
		Vector2d vbnp = (((b.mass - a.mass * elasticConstant) * vbn) + ((a.mass * van) * elasticFactor)) / massSum;
		
		Vector2d vap = vanp + vat;
		Vector2d vbp = vbnp + vbt;
		
		vap *= 1d - t;
		vbp *= 1d - t;
		
		a.trajectory.addCollision(c, vap, a.radius);
		b.trajectory.addCollision(c, vbp, b.radius);
		
		a.trajectory.updateBox();
		b.trajectory.updateBox();
		
		a.nextVelocity = a.trajectory.getAbsVel(1d);
		b.nextVelocity = b.trajectory.getAbsVel(1d);
		
		a.velocity = a.trajectory.getPos(1d) - a.position;
		b.velocity = b.trajectory.getPos(1d) - b.position;
	}
	
	public float[] drawData(){
		return new float[]{(float) position.X, (float) position.Y, (float) radius, color.R/255f, color.G/255f, color.B/255f};
	}
	
	public float[] velocityDrawData(){
		return trajectory.getVelocitiesDrawData();
	}
	
	public float[] forcesDrawData(){
		if(singleForce){
			return new float[]{(float) position.X, (float) position.Y, (float) (position.X + force.X), (float) (position.Y + force.Y)};
		}else{
			List<float> f = new List<float>();
			for(int i = 0; i < forces.Count; i++){
				f.AddRange(new float[]{(float) position.X, (float) position.Y, (float) (position.X + forces[i].X), (float) (position.Y + forces[i].Y)});
			}
			return f.ToArray();
		}
	}
	
	public float[] boxDrawData(){
		return trajectory.box.getDrawData();
	}
	
	public float[] collisionsDrawData(){
		return trajectory.getCollisionsDrawData();
	}
}