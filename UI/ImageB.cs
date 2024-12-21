using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class ImageButton : Button{
	public string textureName;
	
	Vector2i corner;
	Vector2 offset;
	
	Vector2 size;
	
	Vector2 pos;
	
	public Color3 color;
	
	public string? description;
	
	public ImageButton(Renderer r, string tn, int cx, int cy, float ox, float oy, float sx, float sy, Color3 c) : base(r){
		textureName = tn;
		
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		size = new Vector2(sx, sy);
		
		color = c;
		
		updateBox();
	}
	
	public ImageButton setDescription(string d){
		description = d;
		return this;
	}
	
	public override void draw(Vector2d m){		
		ren.ui.draw(textureName, pos, size, color);
		
		if(box != null && box % m && description != null){
			onHover(m);
		}
	}
	
	void onHover(Vector2d m){
		Vector2 mouse = (Vector2) m;
		ren.ui.drawRect(mouse.X, mouse.Y + ren.textSize.Y + 10f, description.Length * ren.textSize.X + 10f, ren.textSize.Y + 10f, ren.black, 0.3f);
		ren.fr.drawText(description, mouse.X + 5f, mouse.Y + ren.textSize.Y + 5f, ren.textSize, ren.textColor);
	}
	
	public override void updateBox(){
		Vector2 dim = new Vector2(ren.width / 2f, ren.height / 2f);
		Vector2 cor = corner * dim;
		
		pos = cor - corner * offset;
		
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
		
		box = new AABB(pos.Y, pos.Y - size.Y, pos.X, pos.X + size.X);
	}
}