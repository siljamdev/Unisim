using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

class KeyBind{
	bool lastPressed;
	
	bool sticky;
	
	bool usesModifier;
	
	Keys key;
	Keys modifier;
	
	public KeyBind(Keys k, bool s){
		key = k;
		sticky = s;
		lastPressed = false;
	}
	
	public KeyBind(Keys k, Keys m, bool s){
		key = k;
		modifier = m;
		sticky = s;
		lastPressed = false;
		usesModifier = true;
	}
	
	public bool isActive(KeyboardState kbd){
		if(kbd.IsKeyDown(key)){
			if(!lastPressed || sticky){
				lastPressed = true;
				return true;
			}
			return false;
		}else{
			lastPressed = false;
			return false;
		}
	}
	
	public byte isActiveMod(KeyboardState kbd){
		if(!usesModifier){
			return 0;
		}
		if(kbd.IsKeyDown(key)){
			if(!lastPressed || sticky){
				lastPressed = true;
				if(kbd.IsKeyDown(modifier)){
					return 2;
				}
				return 1;
			}
			return 0;
		}else{
			lastPressed = false;
			return 0;
		}
	}
}