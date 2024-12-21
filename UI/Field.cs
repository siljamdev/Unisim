using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Field : Button{
	public string text;
	string question;
	
	Vector2i corner;
	Vector2 offset;
	
	Vector2 pos;
	Vector2 textPos;
	Vector2 qPos;
	
	float size;
	
	public bool selected;
	
	public string? description;
	
	public Field(Renderer r, string q, string t, int cx, int cy, float ox, float oy, float xs) : base(r){
		text = t;
		question = q;
		
		corner = new Vector2i(cx, cy);
		offset = new Vector2(ox, oy);
		
		size = xs;
		
		updateBox();
	}
	
	public Field setDescription(string d){
		description = d;
		return this;
	}
	
	public void addChar(char c){
		text += c;
	}
	
	public void delChar(){
		if(text.Length == 0){
			return;
		}
		text = text.Substring(0, text.Length - 1);
	}
	
	public override void draw(Vector2d m){
		Vector2 size = new Vector2(text.Length * ren.textSize.X + 10f, ren.textSize.Y + 10f);
		
		size.X = Math.Max(size.X, this.size);
		
		if(selected){
			ren.ui.drawRect(pos, size, new Color3("666666"), 0.8f);
		}else{
			ren.ui.drawRect(pos, size, ren.black, 0.7f);
		}
		ren.fr.drawText(text, textPos, ren.textSize, ren.textColor, 1f);
		ren.fr.drawText(question, qPos, ren.textSize, ren.textColor, 1f);
		
		
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
		
		Vector2 size = new Vector2(text.Length * ren.textSize.X + 10f, ren.textSize.Y + 10f);
		
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
		
		qPos = new Vector2(pos.X - question.Length * ren.textSize.X - 10f, pos.Y - 5f);
		
		box = new AABB(pos.Y, pos.Y - size.Y, pos.X, pos.X + size.X);
	}
}