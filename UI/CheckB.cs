using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class CheckButton : Button{	
	public string? description;
	
	Vector2i corner;
	Vector2 offset;
	
	Vector2 pos;
	
	Vector2 size;
	
	public Color3 tickColor;
	public Color3 hoverTickColor;
	public Color3 color;
	public Color3 hoverColor;
	
	public bool on;
	
	public CheckButton(bool o, int cx, int cy, float ox, float oy, float xs, float ys, Color3 c) : base(){
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		size = new Vector2(xs, ys);
		
		on = o;
		
		color = c;
		hoverColor = new Color3((byte) (color.R * 1.2f), (byte) (color.G * 1.2f), color.B);
		tickColor = Renderer.textColor;
		hoverTickColor = Renderer.selectedTextColor;
		
		setAction(toggle);
	}
	
	public CheckButton(bool o, int cx, int cy, float ox, float oy, float xs, float ys, Color3 c, Color3 tc, Color3 hc) : base(){
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		size = new Vector2(xs, ys);
		
		on = o;
		
		color = c;
		hoverColor = new Color3((byte) (color.R * 1.2f), (byte) (color.G * 1.2f), color.B);
		tickColor = tc;
		hoverTickColor = hc;
		
		setAction(toggle);
	}
	
	public CheckButton setDescription(string d){
		description = d;
		hasHover = true;
		return this;
	}
	
	void toggle(){
		on = !on;
	}
	
	public override void draw(Renderer ren, Vector2d m){
		if(box != null && box % m){
			ren.ui.drawRect(pos, size, hoverColor, 1f);
			if(on){
				ren.ui.draw("tick", pos, size, hoverTickColor);
			}
		}else{
			ren.ui.drawRect(pos, size, color, 1f);
			if(on){
				ren.ui.draw("tick", pos, size, tickColor);
			}
		}
	}
	
	public override void drawHover(Renderer ren, Vector2d m){
		Vector2 mouse = (Vector2) m;
		
		Vector2 size = new Vector2(description.Length * Renderer.textSize.X + 10f, Renderer.textSize.Y + 10f);
		
		if(mouse.X + size.X <= ren.width / 2f){
			ren.ui.drawRect(mouse.X, mouse.Y + Renderer.textSize.Y + 10f, size.X, size.Y, Renderer.black, 0.5f);
			ren.fr.drawText(description, mouse.X + 5f, mouse.Y + Renderer.textSize.Y + 5f, Renderer.textSize, Renderer.textColor);
		}else{
			if((mouse.X + size.X) - (ren.width / 2f) <= (-ren.width / 2f) - (mouse.X - size.X)){
				ren.ui.drawRect(mouse.X, mouse.Y + Renderer.textSize.Y + 10f, size.X, size.Y, Renderer.black, 0.5f);
				ren.fr.drawText(description, mouse.X + 5f, mouse.Y + Renderer.textSize.Y + 5f, Renderer.textSize, Renderer.textColor);
			}else{
				ren.ui.drawRect(mouse.X - size.X, mouse.Y + Renderer.textSize.Y + 10f, size.X, size.Y, Renderer.black, 0.5f);
				ren.fr.drawText(description, mouse.X - size.X + 5f, mouse.Y + Renderer.textSize.Y + 5f, Renderer.textSize, Renderer.textColor);
			}
		}
	}
	
	public override void updateBox(Renderer ren){
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