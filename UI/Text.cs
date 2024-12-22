using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Text : Button{
	public string text;
	
	Color3 color;
	
	Vector2i corner;
	Vector2 offset;
	
	Vector2 pos;
	
	public Text(string t, int cx, int cy, float ox, float oy, Color3 c) : base(){
		text = t;
		
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		color = c;
	}
	
	public Text(string t, int cx, int cy, float ox, float oy) : this(t, cx, cy, ox, oy, Renderer.textColor){
		
	}
	
	public void setText(Renderer ren, string t){
		text = t;
		updateBox(ren);
	}
	
	public override void draw(Renderer ren, Vector2d m){		
		ren.fr.drawText(text, pos, Renderer.textSize, color, 1f);
	}
	
	public override void drawHover(Renderer ren, Vector2d m){
		
	}
	
	public override void updateBox(Renderer ren){
		Vector2 dim = new Vector2(ren.width / 2f, ren.height / 2f);
		Vector2 cor = corner * dim;
		
		pos = cor - corner * offset;
		
		Vector2 size = new Vector2(text.Length * Renderer.textSize.X, Renderer.textSize.Y);
		
		if(corner.X == 1){
			pos.X -= size.X;
		}
		
		if(corner.Y == -1){
			pos.Y += size.Y;
		}
		
		if(corner.X == 0){
			pos.X -= size.X / 2f;
			pos.X += offset.X;
		}
		
		if(corner.Y == 0){
			pos.Y += size.Y / 2f;
			pos.Y += offset.Y;
		}
	}
}