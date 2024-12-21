using System;

class DrawBufferController{
	DrawBuffer _particles;
	public DrawBuffer particles //Particle position, radius and color 213
	{
		get{
			lock(lockObj){
				return _particles;
			}
		}
	}
	
	DrawBuffer _velocities;
	public DrawBuffer velocities //Particle position and all of its trajectories 2
	{
		get{
			lock(lockObj){
				return _velocities;
			}
		}
	}
	
	DrawBuffer _forces;
	public DrawBuffer forces //Particle forces 2
	{
		get{
			lock(lockObj){
				return _forces;
			}
		}
	}
	
	DrawBuffer _boxes;
	public DrawBuffer boxes //Particle bounding boxes position and size 22
	{
		get{
			lock(lockObj){
				return _boxes;
			}
		}
	}
	
	DrawBuffer _collisions;
	public DrawBuffer collisions //Particle bounding boxes position and size 22
	{
		get{
			lock(lockObj){
				return _collisions;
			}
		}
	}
	
	private readonly object lockObj = new object();
	
	public DrawBufferController(){
		_particles = new DrawBuffer();
		_velocities = new DrawBuffer();
		_forces = new DrawBuffer();
		_boxes = new DrawBuffer();
		_collisions = new DrawBuffer();
	}
	
	public void setBuffers(float[] part, float[] vel, float[] forc, float[] box, float[] col){
		lock(lockObj){
			_particles.setData(part);
			_velocities.setData(vel);
			_forces.setData(forc);
			_boxes.setData(box);
			_collisions.setData(col);
		}
	}
}