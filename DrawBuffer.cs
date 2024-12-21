using System;

class DrawBuffer{
	public bool required;
	
	float[] data;
	
	public void setData(float[] d){
		data = d;
	}
	
	public void setData(List<float> d){
		if(d == null){
			return;
		}
		data = d.ToArray();
	}
	
	public float[] getData(){
		return data;
	}
}