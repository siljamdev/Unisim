using System;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;

class Collision{
	public double t{get; private set;}
	
	public Trajectory a{get; private set;}
	public Trajectory b{get; private set;}
	
	public bool isWithWB;
	
	public WorldBorder wb;
	
	public Vector2d wallNormal;
	
	double ra;
	double rb;
	
	public Collision(Trajectory t1, Trajectory t2, double time, double rad1, double rad2){
		t = time;
		
		a = t1;
		b = t2;
		
		ra = rad1;
		rb = rad2;
	}
	
	public Collision(Trajectory t1, WorldBorder wo, double time, double rad1, Vector2d n){
		t = time;
		
		a = t1;
		wb = wo;
		isWithWB = true;
		wallNormal = n;
		
		ra = rad1;
	}
	
	public void cancel(){
		a.cancelCollision(this, ra);
		
		if(!isWithWB){
			b.cancelCollision(this, rb);
		}
	}
	
	public void cancel(Trajectory c){
		if(isWithWB){
			return;
		}
		if(a == c){
			b.cancelCollision(this, rb);
		}else if(b == c){
			a.cancelCollision(this, ra);
		}
	}
	
	static Collision chooseEarliest(Collision[] cl){
		if(cl.Length < 1){
			return null;
		}
		
		Collision c = cl[0];
		
		for(int i = 1; i < cl.Length; i++){
			if(cl[i].t < c.t){
				c = cl[i];
			}
		}
		
		return c;
	}
	
	public static bool operator == (Collision a, Collision b){
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)){
			return false;
		}

        return ReferenceEquals(a, b);
    }

    public static bool operator !=(Collision a, Collision b){
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)){
			return false;
		}

        return !ReferenceEquals(a, b);
    }
}