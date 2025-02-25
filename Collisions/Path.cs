using System;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Path{
	public Vector2d start{get; private set;}
	public Vector2d increment{get; private set;}
	
	public double tstart{get; private set;}
	public double tend{get; private set;}
	
	public Collision startCollision{get; private set;}
	public Collision endCollision{get; private set;}
	
	public AABB box{get; private set;}
	
	public Path(Vector2d s, Vector2d i, double ts, double te, double r){
		start = s;
		increment = i;
		tstart = ts;
		tend = te;
		
		updateBox(r);
	}
	
	public Path(Vector2d s, Vector2d i, double ts, double te, double r, Collision sc){
		start = s;
		increment = i;
		tstart = ts;
		tend = te;
		
		startCollision = sc;
		
		updateBox(r);
	}
	
	public Formule getFormule(){
		return new Formule(start, increment, tstart, tend);
	}
	
	public Vector2d getAbsoluteVelocity(){
		return increment / (tend - tstart);
	}
	
	public void setUntilEnd(double r){
		increment = increment * ((1d - tstart) / (tend - tstart));
		
		endCollision = null;
		
		tend = 1d;
		
		updateBox(r);
	}
	
	public void setEnd(Collision j, double r){
		increment = increment * ((j.t - tstart) / (tend - tstart));
		
		if(endCollision != null){
			endCollision.cancel();
		}
		
		tend = j.t;
		endCollision = j;
		
		updateBox(r);
	}
	
	public void delete(Trajectory t){
		if(endCollision != null){
			endCollision.cancel(t);
		}
	}
	
	void updateBox(double r){
		box = new AABB(start, start + increment);
		box.expand(r);
	}
	
	public Vector2d getPos(double t){
		if(tstart == tend){
			return start;
		}
		return start + ((t - tstart) / (tend - tstart)) * increment;
	}
	
	public bool containsT(double t){
		return t >= tstart && t <= tend;
	}
	
	public static bool getT(Path a, Path b, Trajectory ta, Trajectory tb, double r1, double r2, out Collision outT){
		if(!(a.tstart <= b.tend && a.tend >= b.tstart)){
			outT = null;
			return false;
		}
		
		if(!(a.box % b.box)){
			outT = null;
			return false;
		}
		
		if(a.endCollision == b.endCollision || a.startCollision == b.endCollision || a.endCollision == b.startCollision || a.startCollision == b.startCollision){
			outT = null;
			return false;
		}
		
		/* if(Vector2d.Distance(a.getPos(a.tstart), a.getPos(b.tstart)) > r1 + r2 && Vector2d.Distance(a.getPos(a.tend), a.getPos(b.tend)) > r1 + r2){
			outT = null;
			return false;
		} */
		
		if(Formule.getT(a.getFormule(), b.getFormule(), r1, r2, out double t) && !intersect(a, b, r1, r2) && a.containsT(t) && b.containsT(t)){
			outT = new Collision(ta, tb, t, r1, r2);
			return true;
		}
		
		outT = null;
		return false;
	}
	
	public static bool intersect(Path a, Path b, double r1, double r2){
		double t = Math.Max(a.tstart, b.tstart);
		double dist = Vector2d.Distance((a.getPos(t)), (b.getPos(t)));
		double radSum = (r1 + r2);
		
		if(dist < radSum){
			return true;
		}
		return false;
	}
	
	public override string ToString(){
		return "Position: " + start + " Increment: " + increment + " (" + tstart + "=>" + tend + ")";
	}
}