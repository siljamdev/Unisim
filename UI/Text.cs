using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Text : Button{
	public string text;
	
	Vector2i corner;
	Vector2 offset;
	
	Vector2 pos;
	
	public string? description;
	
	public Text(Renderer r, string t, int cx, int cy, float ox, float oy) : base(r){
		text = t;
		
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		updateBox();
	}
	
	public void setText(string t){
		text = t;
		updateBox();
	}
	
	public override void draw(Vector2d m){		
		ren.fr.drawText(text, pos, ren.textSize, ren.textColor, 1f);
	}
	
	public override void updateBox(){
		Vector2 dim = new Vector2(ren.width / 2f, ren.height / 2f);
		Vector2 cor = corner * dim;
		
		pos = cor - corner * offset;
		
		Vector2 size = new Vector2(text.Length * ren.textSize.X, ren.textSize.Y);
		
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