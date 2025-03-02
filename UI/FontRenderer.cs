using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using AshLib;

class FontRenderer{	
	char[] map;
	string mapRaw;
	
	int unfoundSymbolPos;
	
	Texture2D font;
	
	int glyphRows; //Rows of the font texture
	int glyphColumns; //columns of the font texture
	
	Mesh fontMesh;
	
	Shader fontShader;
	
	public FontRenderer(Mesh m, Texture2D t, int row, int col, string r){
		glyphRows = row;
		glyphColumns = col;
		font = t;
		mapRaw = r;
		generateMap();
		fontMesh = m;
		
		fontShader = Shader.generateFromAssembly("font");
	}
	
	public FontRenderer(Mesh m, Texture2D t, int row, int col){
		glyphRows = row;
		glyphColumns = col;
		font = t;
		mapRaw = "ABCDEFGHIJKLMNOPQRSTUVWXYZ .,:+-*/\\'\"$()[]^?!%~º1234567890 |□#<>abcdefghijklmnopqrstuvwxyz ;&@`_{}Ññ"; //default map
		generateMap();
		fontMesh = m;
		
		fontShader = Shader.generateFromAssembly("font");
	}
	
	public void setProjection(Matrix4 m){
		fontShader.setMatrix4("projection", m);
	}
	
	void generateMap(){
		this.map = mapRaw.ToCharArray();
		for(int i = 0; i < this.map.Length; i++){
			if(this.map[i] == '□'){
				this.unfoundSymbolPos = i;
				break;
			}
		}
	}
	
	public int[] textToMap(string text){
		int[] l = new int[text.Length];
		char[] c = text.ToCharArray();
		
		for(int i = 0; i < c.Length; i++){
			for(int j = 0; j < this.map.Length; j++){
				if(c[i] == this.map[j]){
					l[i] = j;
					break;
				}
				if(j == this.map.Length - 1){
					l[i] = this.unfoundSymbolPos;
				}
			}
		}
		return l;
	}
	
	public void drawText(string text, Vector2 pos, Vector2 sca, Color3 col, float alpha){
		int[] l = textToMap(text);
		
		fontShader.use();
		fontShader.setIntArray("letters", l); //Set the letters so it knows wich glyphs to actually choose
		fontShader.setVector2("position", pos); //Starting position
		fontShader.setVector2("size", sca); //Size will be static no matter the size of the window
		fontShader.setVector2i("glyphStructure", new Vector2i(this.glyphRows, this.glyphColumns)); //Pass the number of rows and cols
		fontShader.setVector4("col", col, alpha); //Pass the color
		
		font.bind();
		
		fontMesh.drawInstanced(l.Length);
	}
	
	public void drawText(string text, float xpos, float ypos, float xsca, float ysca, Color3 col){
		drawText(text, new Vector2(xpos, ypos), new Vector2(xsca, ysca), col, 1f);
	}
	
	public void drawText(string text, float xpos, float ypos, float xsca, float ysca, Color3 col, float alpha){
		drawText(text, new Vector2(xpos, ypos), new Vector2(xsca, ysca), col, alpha);
	}
	
	public void drawText(string text, float xpos, float ypos, float sca, Color3 col){
		drawText(text, new Vector2(xpos, ypos), new Vector2(sca, sca), col, 1f);
	}
	
	public void drawText(string text, float xpos, float ypos, float sca, Color3 col, float alpha){
		drawText(text, new Vector2(xpos, ypos), new Vector2(sca, sca), col, alpha);
	}
	
	public void drawText(string text, float xpos, float ypos, Vector2 sca, Color3 col){
		drawText(text, new Vector2(xpos, ypos), sca, col, 1f);
	}
	
	public void drawText(string text, float xpos, float ypos, Vector2 sca, Color3 col, float alpha){
		drawText(text, new Vector2(xpos, ypos), sca, col, alpha);
	}
}