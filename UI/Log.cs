using System;
using OpenTK;
using OpenTK.Mathematics;
using AshLib;

class Log : Button{
	public string[] text;
	
	float offset;
	float scrollOffset;
	Vector2 margin;
	
	public Log(float lm, float rm, float oy, params string[] t) : base(){
		text = t;
		
		margin = new Vector2(lm, rm);
		offset = oy;
	}
	
	public void scroll(Renderer ren, float f){
		f = -f;
		if(scrollOffset <= 0f && f < 0f){
			return;
		}
		
		scrollOffset += f * 10f;
	}
	
	public override void draw(Renderer ren, Vector2d m){
		float d = offset;
		
		int maxChars = (int) ((ren.width - margin.X - margin.Y) / Renderer.textSize.X);
		
		for(int i = 0; i < text.Length; i++){
			string[] lines = divideLines(text[i], maxChars);
			for(int j = 0; j < lines.Length; j++){
				ren.fr.drawText(lines[j], -ren.width / 2f + margin.X, ren.height / 2f - d + scrollOffset, Renderer.textSize, Renderer.textColor);
				d += Renderer.textSize.Y;
			}
			d += Renderer.fieldSeparation - Renderer.textSize.Y;
		}
	}
	
	public override void drawHover(Renderer ren, Vector2d m){
		
	}
	
	public override void updateBox(Renderer ren){
		scrollOffset = 0f;
	}
	
	public static string[] divideLines(string input, int maxCharsPerLine)
    {
        if (maxCharsPerLine <= 0)
            throw new ArgumentException("Max characters per line must be greater than zero.", nameof(maxCharsPerLine));

        List<string> lines = new List<string>();
        string[] words = input.Split(' ');
        string currentLine = string.Empty;

        foreach (string word in words)
        {
            if (word.Length > maxCharsPerLine)
            {
                // Break up long words if they exceed maxCharsPerLine
                if (currentLine.Length > 0)
                {
                    lines.Add(currentLine);
                    currentLine = string.Empty;
                }

                for (int i = 0; i < word.Length; i += maxCharsPerLine)
                {
                    int length = Math.Min(maxCharsPerLine, word.Length - i);
                    lines.Add(word.Substring(i, length));
                }
            }
            else if (currentLine.Length + word.Length + 1 <= maxCharsPerLine)
            {
                // Add the word to the current line if it fits
                if (currentLine.Length > 0)
                {
                    currentLine += " ";
                }
                currentLine += word;
            }
            else
            {
                // Start a new line if the word doesn't fit
                if (currentLine.Length > 0)
                {
                    lines.Add(currentLine);
                }
                currentLine = word;
            }
        }

        // Add the last line if not empty
        if (currentLine.Length > 0)
        {
            lines.Add(currentLine);
        }

        return lines.ToArray();
    }
}