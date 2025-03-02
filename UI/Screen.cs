using System;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using AshLib;

#if WINDOWS
	using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
#endif

class Screen{
	public List<Button> buttons{get; private set;}
	
	public bool keySelecting;
	
	public bool writing;
	public int selected;
	
	public bool doScroll;
	
	public Action? closeAction;
	
	Text? errorText;
	Log? scrollLog;
	
	public Screen(params Button[] b){
		buttons = new List<Button>();
		
		buttons.AddRange(b);
	}
	
	public Screen setCloseAction(Action a){
		closeAction = a;
		return this;
	}
	
	public Screen setWriting(){
		writing = true;
		selected = -1;
		return this;
	}
	
	public Screen setKeySelecting(){
		keySelecting = true;
		selected = -1;
		return this;
	}
	
	public Screen setScrollingLog(Log g){
		buttons.Add(g);
		doScroll = true;
		scrollLog = g;
		return this;
	}
	
	public void scroll(Renderer ren, float f){
		if(scrollLog != null){
			scrollLog.scroll(ren, f);
		}
	}
	
	public Screen setErrorText(Text t){
		buttons.Add(t);
		errorText = t;
		return this;
	}
	
	public void showError(Renderer ren, string s){
		if(errorText == null){
			return;
		}
		errorText.setText(ren, s);
	}
	
	public void draw(Renderer ren, bool doHover){
		Vector2d mouse = ren.cam.mouseLastPos - new Vector2d(ren.width / 2f, ren.height / 2f);
		mouse.Y = -mouse.Y;
		
		foreach(Button b in buttons){
			if(b.active){
				b.draw(ren, mouse);
			}
		}
		
		if(doHover){
			foreach(Button b in buttons){
				if(b.active && b.hasHover && b.box != null && b.box % mouse){
					b.drawHover(ren, mouse);
				}
			}
		}
	}
	
	public bool click(Renderer ren, Vector2d m, bool shift){
		Vector2d mouse = ren.cam.mouseLastPos - new Vector2d(ren.width / 2f, ren.height / 2f);
		mouse.Y = -mouse.Y;
		
		for(int i = buttons.Count - 1; i >= 0; i--){
			Button b = buttons[i];
			if(b.active && b.box != null && b.box % mouse){
				if(writing && b is Field f){
					if(selected > -1 && buttons[selected] is Field p){
						p.selected = false;
					}
					
					f.selected = true;
					selected = i;
				}else if(keySelecting && b is KeyField kf){
					if(selected > -1 && buttons[selected] is KeyField p){
						p.selected = false;
					}
					
					kf.selected = true;
					selected = i;
				}else{
					if(writing && selected > -1 && buttons[selected] is Field p){
						p.selected = false;
						selected = -1;
					}else if(keySelecting && selected > -1 && buttons[selected] is KeyField kp){
						kp.selected = false;
						selected = -1;
					}
					
					if(b.quickAction != null && shift){
						b.quickAction.Invoke();
					}else if(b.action != null){
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
	
	public void trySetKeybind(Keys k){
		if(keySelecting && selected != -1 && buttons[selected] is KeyField kf){
			kf.key.update(k);
		}
	}
	
	public WritingType tryGetWritingMode(){
		if(writing && selected > -1 && buttons[selected] is Field f){
			return f.type;
		}
		
		return WritingType.Hex;
	}
	
	public void tryAddStr(string s){
		if(writing && selected != -1 && buttons[selected] is Field f){
			f.addStr(s);
		}
	}
	
	public void tryDelChar(){
		if(writing && selected != -1 && buttons[selected] is Field f){
			f.delChar();
		}
	}
	
	public void updateProj(Renderer ren){
		foreach(Button b in buttons){
			b.updateBox(ren);
		}
	}
}