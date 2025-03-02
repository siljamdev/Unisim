using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class KeyField : Button{
	public KeyBind key;
	string question;
	
	Vector2i corner;
	Vector2 offset;
	
	Vector2 pos;
	Vector2 textPos;
	Vector2 qPos;
	
	float size;
	
	public bool selected;
	
	public string? description;
	
	public KeyField(string q, KeyBind k, int cx, int cy, float ox, float oy) : base(){
		key = k;
		question = q;
		
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
	}
	
	public KeyField setDescription(string d){
		description = d;
		hasHover = true;
		return this;
	}
	
	public override void draw(Renderer ren, Vector2d m){
		string text = key.key.ToString();
		Vector2 size = new Vector2(text.Length * Renderer.textSize.X + 10f, Renderer.textSize.Y + 10f);
		
		if(selected){
			ren.ui.drawRect(pos, size, Renderer.fieldSelectedColor, 0.8f);
			ren.fr.drawText(text, textPos, Renderer.textSize, Renderer.textColor, 1f);
		}else{
			ren.ui.drawRect(pos, size, Renderer.fieldColor, 0.7f);
			ren.fr.drawText(text, textPos, Renderer.textSize, Renderer.fieldTextColor, 1f);
		}
		ren.fr.drawText(question, qPos, Renderer.textSize, Renderer.textColor, 1f);
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
		
		string text = key.key.ToString();
		
		Vector2 size = new Vector2(text.Length * Renderer.textSize.X + 10f, Renderer.textSize.Y + 10f);
		
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
		
		qPos = new Vector2(pos.X - question.Length * Renderer.textSize.X - 10f, pos.Y - 5f);
		
		box = new AABB(pos.Y, pos.Y - size.Y, pos.X, pos.X + size.X);
	}
}