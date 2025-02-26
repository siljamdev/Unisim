using System;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

struct Formule{
	Vector2d pos;
	Vector2d vel;
	
	public Formule(Vector2d start, Vector2d increment, double tstart, double tend){
		pos = start + increment - ((tend * increment) / (tend - tstart));
		vel = increment / (tend - tstart);
	}
	
	public Vector2d getPos(double t){
		return pos + t * vel;
	}
	
	public override string ToString(){
		return pos + " + t * " + vel;
	}
	
	public static bool getT(Formule f1, WorldBorder wb, double r1, int x, out double t, out Vector2d n){
		double t2 = 10d;
		Vector2d normal = Vector2d.Zero;
		switch(x){
			case 0:
			t2 = (wb.size.X - r1 - f1.pos.X) / f1.vel.X;
			normal = new Vector2d(-1d, 0d);
			break;
			
			case 1:
			t2 = (-wb.size.X + r1 - f1.pos.X) / f1.vel.X;
			normal = new Vector2d(1d, 0d);
			break;
			
			case 2:
			t2 = (wb.size.Y - r1 - f1.pos.Y) / f1.vel.Y;
			normal = new Vector2d(0d, -1d);
			break;
			
			case 3:
			t2 = (-wb.size.Y + r1 - f1.pos.Y) / f1.vel.Y;
			normal = new Vector2d(0d, 1d);
			break;
		}
		
		if(t2 >= 0d && t2 <= 1d){
			n = normal;
			t = t2;
			return true;
		}
		
		t = 0d;
		n = Vector2d.Zero;
		return false;
	}
	
	public static bool getT(Formule f1, Formule f2, double r1, double r2, out double t){
		double a = f1.vel.X - f2.vel.X;
		double b = f1.vel.Y - f2.vel.Y;
		double c = f1.pos.X - f2.pos.X;
		double d = f1.pos.Y - f2.pos.Y;
		
		double g = Math.Pow(r1 + r2, 2d);
		double h = a * c;
		double k = b * d;
		double m = Math.Pow(a, 2d);
		double n = Math.Pow(b, 2d);
		
		double delta = 4d * (2d * h * k + m * (g - (float) Math.Pow(d, 2d)) + n * (g - (float) Math.Pow(c, 2d)));
		
		if(!(delta >= 0d)){
			t = 0d;
			return false;
		}
		
		double sigma = -2d * (h + k);
		
		double epsilon = 2d * (m + n);
		
		double sqDelta = Math.Sqrt(delta);
		
		double t1 = (sigma + sqDelta) / epsilon;
		double t2 = (sigma - sqDelta) / epsilon;
		
		if(t1 >= 0d && t1 <= 1d){
			if(t2 >= 0d && t2 <= 1d){
				t = Math.Min(t1, t2);
				return true;
			}else{
				t = t1;
				return true;
			}
		}else{
			if(t2 >= 0d && t2 <= 1d){
				t = t2;
				return true;
			}else{
				t = 0d;
				return false;
			}
		}
	}
}