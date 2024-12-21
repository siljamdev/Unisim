using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Screen{
	public List<Button> buttons{get; private set;}
	
	public bool writing;
	public int selected;
	
	Renderer ren;
	
	public Screen(Renderer r, params Button[] b){
		ren = r;
		
		buttons = new List<Button>();
		
		buttons.AddRange(b);
	}
	
	public Screen setWriting(){
		writing = true;
		selected = -1;
		return this;
	}
	
	public void draw(){
		Vector2d mouse = ren.cam.mouseLastPos - new Vector2d(ren.width / 2f, ren.height / 2f);
		mouse.Y = -mouse.Y;
		
		foreach(Button b in buttons){
			if(b.active){
				b.draw(mouse);
			}
		}
	}
	
	public bool click(Vector2d m){
		Vector2d mouse = ren.cam.mouseLastPos - new Vector2d(ren.width / 2f, ren.height / 2f);
		mouse.Y = -mouse.Y;
		
		for(int i = 0; i < buttons.Count; i++){
			Button b = buttons[i];
			if(b.active && b.box != null && b.box % mouse){
				if(writing && b is Field f){
					if(selected > -1 && buttons[selected] is Field p){
						p.selected = false;
					}
					
					f.selected = true;
					selected = i;
				}else{
					if(writing && selected > -1 && buttons[selected] is Field p){
						p.selected = false;
						selected = -1;
					}
					
					if(b.action != null){
						b.action.Invoke();
					}
				}
				
				return true;
			}
		}
		
		if(writing && selected > -1 && buttons[selected] is Field e){
			e.selected = false;
			selected = -1;
		}
		return false;
	}
	
	public void tryAdd(char c){
		if(writing && selected != -1 && buttons[selected] is Field f){
			f.addChar(c);
		}
	}
	
	public void tryDel(){
		if(writing && selected != -1 && buttons[selected] is Field f){
			f.delChar();
		}
	}
	
	public void updateProj(){
		foreach(Button b in buttons){
			b.updateBox();
		}
	}
}