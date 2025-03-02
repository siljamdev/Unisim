using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class TextButton : Button{
	public string text;
	
	Vector2i corner;
	Vector2 offset;
	
	Vector2 pos;
	Vector2 textPos;
	
	float size;
	
	public Color3 textColor;
	public Color3 hoverTextColor;
	public Color3 color;
	public Color3 hoverColor;
	
	public string? description;
	
	public TextButton(string t, int cx, int cy, float ox, float oy, float xs, Color3 c) : base(){
		text = t;
		
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		size = xs;
		
		color = c;
		hoverColor = new Color3((byte) (color.R * 1.2f), (byte) (color.G * 1.2f), color.B);
		textColor = Renderer.textColor;
		hoverTextColor = Renderer.selectedTextColor;
	}
	
	public TextButton(string t, int cx, int cy, float ox, float oy, float xs, Color3 c, Color3 tc, Color3 hc) : base(){
		text = t;
		
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		size = xs;
		
		color = c;
		hoverColor = new Color3((byte) (color.R * 1.2f), (byte) (color.G * 1.2f), color.B);
		textColor = tc;
		hoverTextColor = hc;
	}
	
	public TextButton setDescription(string d){
		description = d;
		hasHover = true;
		return this;
	}
	
	public override void draw(Renderer ren, Vector2d m){
		Vector2 size = new Vector2(text.Length * Renderer.textSize.X + 10f, Renderer.textSize.Y + 10f);
		
		size.X = Math.Max(size.X, this.size);
		
		if(box != null && box % m){
			ren.ui.drawRect(pos, size, hoverColor, 1f);
			ren.fr.drawText(text, textPos, Renderer.textSize, hoverTextColor, 1f);
		}else{
			ren.ui.drawRect(pos, size, color, 1f);
			ren.fr.drawText(text, textPos, Renderer.textSize, textColor, 1f);
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
		
		Vector2 size = new Vector2(text.Length * Renderer.textSize.X + 10f, Renderer.textSize.Y + 10f);
		
		size.X = Math.Max(size.X, this.size);
		
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
		
		textPos = new Vector2(pos.X + 5f, pos.Y - 5f);
		
		box = new AABB(pos.Y, pos.Y - size.Y, pos.X, pos.X + size.X);
	}
}