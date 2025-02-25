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
	
	public static bool getFullTyping(KeyboardState kbd, out char c){
		if(kbd.IsKeyDown(Keys.A)){
			c = 'A';
			return true;
		}else if(kbd.IsKeyDown(Keys.B)){
			c = 'B';
			return true;
		}else if(kbd.IsKeyDown(Keys.C)){
			c = 'C';
			return true;
		}else if(kbd.IsKeyDown(Keys.D)){
			c = 'D';
			return true;
		}else if(kbd.IsKeyDown(Keys.E)){
			c = 'E';
			return true;
		}else if(kbd.IsKeyDown(Keys.F)){
			c = 'F';
			return true;
		}else if(kbd.IsKeyDown(Keys.G)){
			c = 'G';
			return true;
		}else if(kbd.IsKeyDown(Keys.H)){
			c = 'H';
			return true;
		}else if(kbd.IsKeyDown(Keys.I)){
			c = 'I';
			return true;
		}else if(kbd.IsKeyDown(Keys.J)){
			c = 'J';
			return true;
		}else if(kbd.IsKeyDown(Keys.K)){
			c = 'K';
			return true;
		}else if(kbd.IsKeyDown(Keys.L)){
			c = 'L';
			return true;
		}else if(kbd.IsKeyDown(Keys.M)){
			c = 'M';
			return true;
		}else if(kbd.IsKeyDown(Keys.N)){
			c = 'N';
			return true;
		}else if(kbd.IsKeyDown(Keys.O)){
			c = 'O';
			return true;
		}else if(kbd.IsKeyDown(Keys.P)){
			c = 'P';
			return true;
		}else if(kbd.IsKeyDown(Keys.Q)){
			c = 'Q';
			return true;
		}else if(kbd.IsKeyDown(Keys.R)){
			c = 'R';
			return true;
		}else if(kbd.IsKeyDown(Keys.S)){
			c = 'S';
			return true;
		}else if(kbd.IsKeyDown(Keys.T)){
			c = 'T';
			return true;
		}else if(kbd.IsKeyDown(Keys.U)){
			c = 'U';
			return true;
		}else if(kbd.IsKeyDown(Keys.V)){
			c = 'V';
			return true;
		}else if(kbd.IsKeyDown(Keys.W)){
			c = 'W';
			return true;
		}else if(kbd.IsKeyDown(Keys.X)){
			c = 'X';
			return true;
		}else if(kbd.IsKeyDown(Keys.Y)){
			c = 'Y';
			return true;
		}else if(kbd.IsKeyDown(Keys.Z)){
			c = 'Z';
			return true;
		}else if(kbd.IsKeyDown(Keys.Space)){
			c = ' ';
			return true;
		}if(kbd.IsKeyDown(Keys.Period)){
			c = '.';
			return true;
		}else if(kbd.IsKeyDown(Keys.Slash)){
			c = '-';
			return true;
		}else if(kbd.IsKeyDown(Keys.D0)){
			c = '0';
			return true;
		}else if(kbd.IsKeyDown(Keys.D1)){
			c = '1';
			return true;
		}else if(kbd.IsKeyDown(Keys.D2)){
			c = '2';
			return true;
		}else if(kbd.IsKeyDown(Keys.D3)){
			c = '3';
			return true;
		}else if(kbd.IsKeyDown(Keys.D4)){
			c = '4';
			return true;
		}else if(kbd.IsKeyDown(Keys.D5)){
			c = '5';
			return true;
		}else if(kbd.IsKeyDown(Keys.D6)){
			c = '6';
			return true;
		}else if(kbd.IsKeyDown(Keys.D7)){
			c = '7';
			return true;
		}else if(kbd.IsKeyDown(Keys.D8)){
			c = '8';
			return true;
		}else if(kbd.IsKeyDown(Keys.D9)){
			c = '9';
			return true;
		}
		
		c = ' ';
		return false;
	}
	
	public static bool getStringTyping(KeyboardState kbd, out char c){
		if(kbd.IsKeyDown(Keys.A)){
			c = 'a';
			return true;
		}else if(kbd.IsKeyDown(Keys.B)){
			c = 'b';
			return true;
		}else if(kbd.IsKeyDown(Keys.C)){
			c = 'c';
			return true;
		}else if(kbd.IsKeyDown(Keys.D)){
			c = 'd';
			return true;
		}else if(kbd.IsKeyDown(Keys.E)){
			c = 'e';
			return true;
		}else if(kbd.IsKeyDown(Keys.F)){
			c = 'f';
			return true;
		}else if(kbd.IsKeyDown(Keys.G)){
			c = 'g';
			return true;
		}else if(kbd.IsKeyDown(Keys.H)){
			c = 'h';
			return true;
		}else if(kbd.IsKeyDown(Keys.I)){
			c = 'i';
			return true;
		}else if(kbd.IsKeyDown(Keys.J)){
			c = 'j';
			return true;
		}else if(kbd.IsKeyDown(Keys.K)){
			c = 'k';
			return true;
		}else if(kbd.IsKeyDown(Keys.L)){
			c = 'l';
			return true;
		}else if(kbd.IsKeyDown(Keys.M)){
			c = 'm';
			return true;
		}else if(kbd.IsKeyDown(Keys.N)){
			c = 'n';
			return true;
		}else if(kbd.IsKeyDown(Keys.O)){
			c = 'o';
			return true;
		}else if(kbd.IsKeyDown(Keys.P)){
			c = 'p';
			return true;
		}else if(kbd.IsKeyDown(Keys.Q)){
			c = 'q';
			return true;
		}else if(kbd.IsKeyDown(Keys.R)){
			c = 'r';
			return true;
		}else if(kbd.IsKeyDown(Keys.S)){
			c = 's';
			return true;
		}else if(kbd.IsKeyDown(Keys.T)){
			c = 't';
			return true;
		}else if(kbd.IsKeyDown(Keys.U)){
			c = 'u';
			return true;
		}else if(kbd.IsKeyDown(Keys.V)){
			c = 'v';
			return true;
		}else if(kbd.IsKeyDown(Keys.W)){
			c = 'w';
			return true;
		}else if(kbd.IsKeyDown(Keys.X)){
			c = 'x';
			return true;
		}else if(kbd.IsKeyDown(Keys.Y)){
			c = 'y';
			return true;
		}else if(kbd.IsKeyDown(Keys.Z)){
			c = 'z';
			return true;
		}else if(kbd.IsKeyDown(Keys.Space)){
			c = ' ';
			return true;
		}
		
		c = ' ';
		return false;
	}
	
	public static bool getHexTyping(KeyboardState kbd, out char c){
		if(kbd.IsKeyDown(Keys.D0)){
			c = '0';
			return true;
		}else if(kbd.IsKeyDown(Keys.D1)){
			c = '1';
			return true;
		}else if(kbd.IsKeyDown(Keys.D2)){
			c = '2';
			return true;
		}else if(kbd.IsKeyDown(Keys.D3)){
			c = '3';
			return true;
		}else if(kbd.IsKeyDown(Keys.D4)){
			c = '4';
			return true;
		}else if(kbd.IsKeyDown(Keys.D5)){
			c = '5';
			return true;
		}else if(kbd.IsKeyDown(Keys.D6)){
			c = '6';
			return true;
		}else if(kbd.IsKeyDown(Keys.D7)){
			c = '7';
			return true;
		}else if(kbd.IsKeyDown(Keys.D8)){
			c = '8';
			return true;
		}else if(kbd.IsKeyDown(Keys.D9)){
			c = '9';
			return true;
		}else if(kbd.IsKeyDown(Keys.A)){
			c = 'A';
			return true;
		}else if(kbd.IsKeyDown(Keys.B)){
			c = 'B';
			return true;
		}else if(kbd.IsKeyDown(Keys.C)){
			c = 'C';
			return true;
		}else if(kbd.IsKeyDown(Keys.D)){
			c = 'D';
			return true;
		}else if(kbd.IsKeyDown(Keys.E)){
			c = 'E';
			return true;
		}else if(kbd.IsKeyDown(Keys.F)){
			c = 'F';
			return true;
		}
		
		c = ' ';
		return false;
	}
	
	public static bool getFloatPositiveTyping(KeyboardState kbd, out char c){
		if(kbd.IsKeyDown(Keys.Period)){
			c = '.';
			return true;
		}else if(kbd.IsKeyDown(Keys.D0)){
			c = '0';
			return true;
		}else if(kbd.IsKeyDown(Keys.D1)){
			c = '1';
			return true;
		}else if(kbd.IsKeyDown(Keys.D2)){
			c = '2';
			return true;
		}else if(kbd.IsKeyDown(Keys.D3)){
			c = '3';
			return true;
		}else if(kbd.IsKeyDown(Keys.D4)){
			c = '4';
			return true;
		}else if(kbd.IsKeyDown(Keys.D5)){
			c = '5';
			return true;
		}else if(kbd.IsKeyDown(Keys.D6)){
			c = '6';
			return true;
		}else if(kbd.IsKeyDown(Keys.D7)){
			c = '7';
			return true;
		}else if(kbd.IsKeyDown(Keys.D8)){
			c = '8';
			return true;
		}else if(kbd.IsKeyDown(Keys.D9)){
			c = '9';
			return true;
		}
		
		c = ' ';
		return false;
	}
	
	public static bool getFloatTyping(KeyboardState kbd, out char c){
		if(kbd.IsKeyDown(Keys.Period)){
			c = '.';
			return true;
		}else if(kbd.IsKeyDown(Keys.Slash)){
			c = '-';
			return true;
		}else if(kbd.IsKeyDown(Keys.D0)){
			c = '0';
			return true;
		}else if(kbd.IsKeyDown(Keys.D1)){
			c = '1';
			return true;
		}else if(kbd.IsKeyDown(Keys.D2)){
			c = '2';
			return true;
		}else if(kbd.IsKeyDown(Keys.D3)){
			c = '3';
			return true;
		}else if(kbd.IsKeyDown(Keys.D4)){
			c = '4';
			return true;
		}else if(kbd.IsKeyDown(Keys.D5)){
			c = '5';
			return true;
		}else if(kbd.IsKeyDown(Keys.D6)){
			c = '6';
			return true;
		}else if(kbd.IsKeyDown(Keys.D7)){
			c = '7';
			return true;
		}else if(kbd.IsKeyDown(Keys.D8)){
			c = '8';
			return true;
		}else if(kbd.IsKeyDown(Keys.D9)){
			c = '9';
			return true;
		}
		
		c = ' ';
		return false;
	}
	
	public static bool getIntTyping(KeyboardState kbd, out char c){
		if(kbd.IsKeyDown(Keys.D0)){
			c = '0';
			return true;
		}else if(kbd.IsKeyDown(Keys.D1)){
			c = '1';
			return true;
		}else if(kbd.IsKeyDown(Keys.D2)){
			c = '2';
			return true;
		}else if(kbd.IsKeyDown(Keys.D3)){
			c = '3';
			return true;
		}else if(kbd.IsKeyDown(Keys.D4)){
			c = '4';
			return true;
		}else if(kbd.IsKeyDown(Keys.D5)){
			c = '5';
			return true;
		}else if(kbd.IsKeyDown(Keys.D6)){
			c = '6';
			return true;
		}else if(kbd.IsKeyDown(Keys.D7)){
			c = '7';
			return true;
		}else if(kbd.IsKeyDown(Keys.D8)){
			c = '8';
			return true;
		}else if(kbd.IsKeyDown(Keys.D9)){
			c = '9';
			return true;
		}
		
		c = ' ';
		return false;
	}
}