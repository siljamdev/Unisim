using System;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Trajectory{
	List<Path> paths;
	
	public AABB box{get; private set;}
	
	bool cancelled;
	
	public Trajectory(Vector2d p, Vector2d v, double r){
		paths = new List<Path>();
		paths.Add(new Path(p, v, 0d, 1d, r));
		
		updateBox();
	}
	
	public void reset(Vector2d p, Vector2d v, double r){
		paths.Clear();
		paths.Add(new Path(p, v, 0d, 1d, r));
		
		updateBox();
	}
	
	public Vector2d getPos(double t){
		foreach(Path p in paths){
			if(p.containsT(t)){
				return p.getPos(t);
			}
		}
		return Vector2d.Zero;
	}
	
	public Vector2d getAbsVel(double t){
		foreach(Path p in paths){
			if(p.containsT(t)){
				return p.getAbsoluteVelocity();
			}
		}
		return Vector2d.Zero;
	}
	
	public void addCollision(Collision c, Vector2d relVel, double radius){
		int i;
		for(i = 0; i < paths.Count; i++){
			if(paths[i].containsT(c.t)){
				break;
			}
		}
		
		cancelled = true;
		
		paths[i].setEnd(c, radius);
		
		for(int j = i + 1; j < paths.Count; j++){
			paths[j].delete(this);
		}
		
		paths.RemoveRange(i + 1, paths.Count - i - 1);
		
		paths.Add(new Path(getPos(c.t), relVel, c.t, 1d, radius, c));
		
		/* for(int k = 0; k < paths.Count; k++){
			if(paths[k].tstart == paths[k].tend){
				paths.RemoveAt(k);
			}
		} */
		
		cancelled = false;
	}
	
	public void cancelCollision(Collision c, double r){
		if(cancelled){
			return;
		}
		
		int i = -1;
		for(i = 0; i < paths.Count; i++){
			if(paths[i].endCollision == c && paths[i].tend == c.t){
				break;
			}
			if(i == paths.Count - 1){
				return;
			}
		}
		
		cancelled = true;
		
		for(int j = i + 1; j < paths.Count; j++){
			paths[j].delete(this);
		}
		
		paths[i].setUntilEnd(r);
		
		paths.RemoveRange(i + 1, paths.Count - i - 1);
		
		cancelled = false;
	}
	
	public void updateBox(){
		box = paths[0].box;
		for(int i = 1; i < paths.Count; i++){
			box += paths[i].box;
		}
	}
	
	public static bool getT(Trajectory a, WorldBorder wb, double r1, out Collision outT){
		if(wb.contains(a.box)){
			outT = null;
			return false;
		}
		
		//List<Collision> ts = new List<Collision>();
		
		/* for(int i = 0; i < a.paths.Count; i++){
			if(Path.getT(a.paths[i], wb, a, r1, out Collision t)){
				ts.Add(t);
			}
		} */
		
		if(Path.getT(a.paths[a.paths.Count - 1], wb, a, r1, out Collision t)){
			outT = t;
			return true;
		}
		
		/* if(ts.Count > 0){
			Collision min = ts[0];
			
			foreach(Collision c in ts){
				if (c.t < min.t){
					min = c;
				}
			}
			
			outT = min;
			return true;
		} */
		
		outT = null;
		return false;
	}
	
	public static bool getT(Trajectory a, Trajectory b, double r1, double r2, out Collision outT){
		if(!(a.box % b.box)){
			outT = null;
			return false;
		}
		
		List<Collision> ts = new List<Collision>();
		
		for(int i = 0; i < a.paths.Count; i++){
			for(int j = 0; j < b.paths.Count; j++){
				if(Path.getT(a.paths[i], b.paths[j], a, b, r1, r2, out Collision t)){
					ts.Add(t);
				}
			}
		}
		
		if(ts.Count > 0){
			Collision min = ts[0];
			
			foreach(Collision c in ts){
				if (c.t < min.t){
					min = c;
				}
			}
			
			outT = min;
			return true;
		}
		
		outT = null;
		return false;
	}
	
	public float[] getVelocitiesDrawData(){
		List<float> f = new List<float>();
		for(int i = 0; i < paths.Count; i++){
			f.AddRange(new float[]{(float) paths[i].start.X, (float) paths[i].start.Y, (float) (paths[i].start.X + paths[i].increment.X), (float) (paths[i].start.Y + paths[i].increment.Y)});
		}
		return f.ToArray();
	}
	
	public float[] getCollisionsDrawData(){
		List<float> f = new List<float>();
		for(int i = 0; i < paths.Count; i++){
			if(paths[i].endCollision != null){
				f.AddRange(new float[]{(float) (paths[i].start.X + paths[i].increment.X), (float) (paths[i].start.Y + paths[i].increment.Y)});
			}
		}
		return f.ToArray();
	}
	
	public override string ToString(){
		return ToString(0);
	}
	
	public string ToString(int s){
		StringBuilder sb = new StringBuilder();
		
		sb.Append(new string(' ', s) + "Trajectory: ");
		
		foreach(Path p in paths){
			sb.Append(new string(' ', s) + p.ToString());
			sb.Append("\n");
		}
		return sb.ToString();
	}
}