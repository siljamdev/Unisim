using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class ImageBackButton : Button{
	public string textureName;
	
	Vector2i corner;
	Vector2 offset;
	
	Vector2 size;
	
	Vector2 pos;
	Vector2 imagPos;
	Vector2 imagSize;
	
	public Color3 color;
	
	public Color3 backColor;
	public Color3 hoverBackColor;
	
	public string? description;
	
	public ImageBackButton(string tn, int cx, int cy, float ox, float oy, float sx, float sy, Color3 c, Color3 b) : base(){
		textureName = tn;
		
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		size = new Vector2(sx, sy);
		
		color = c;
		backColor = b;
		hoverBackColor = new Color3((byte) (backColor.R * 1.2f), (byte) (backColor.G * 1.2f), backColor.B);
	}
	
	public ImageBackButton setDescription(string d){
		description = d;
		hasHover = true;
		return this;
	}
	
	public override void draw(Renderer ren, Vector2d m){
		if(box != null && box % m){
			ren.ui.drawRect(pos, size, hoverBackColor, 1f);
			ren.ui.draw(textureName, imagPos, imagSize, Renderer.selectedTextColor);
		}else{
			ren.ui.drawRect(pos, size, backColor, 1f);
			ren.ui.draw(textureName, imagPos, imagSize, color);
		}
	}
	
	public override void drawHover(Renderer ren, Vector2d m){
		Vector2 mouse = (Vector2) m;
		
		Vector2 size = new Vector2(description.Length * Renderer.textSize.X + 10f, Renderer.textSize.Y + 10f);
		
		if(mouse.X + size.X > ren.width / 2f){
			ren.ui.drawRect(mouse.X - size.X, mouse.Y + Renderer.textSize.Y + 10f, size.X, size.Y, Renderer.black, 0.5f);
			ren.fr.drawText(description, mouse.X - size.X + 5f, mouse.Y + Renderer.textSize.Y + 5f, Renderer.textSize, Renderer.textColor);
		}else{
			ren.ui.drawRect(mouse.X, mouse.Y + Renderer.textSize.Y + 10f, size.X, size.Y, Renderer.black, 0.5f);
			ren.fr.drawText(description, mouse.X + 5f, mouse.Y + Renderer.textSize.Y + 5f, Renderer.textSize, Renderer.textColor);
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
		
		imagPos = new Vector2(pos.X + 5f, pos.Y - 5f);
		imagSize = new Vector2(size.X - 10f, size.Y - 10f);
		
		box = new AABB(pos.Y, pos.Y - size.Y, pos.X, pos.X + size.X);
	}
}