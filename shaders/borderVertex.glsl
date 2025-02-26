#version 330 core

layout (location = 0) in vec2 aPos;

out vec2 WorldPos;

uniform float zoom;
uniform vec2 pos;

uniform vec2 resolution;

void main()
{
	//TexCoord = (projection * vec4((-pos/resolution * 2.0) + (aPos/zoom), 0.0, 1.0)).xy;
	
    //TexCoord = pos;
    WorldPos = pos - (aPos * (resolution/2.0)/zoom);
	
	//TexCoord -= aPos * vec2(resolution.x / resolution.y, 1.0) * ((resolution/2.0)/zoom);
	//TexCoord.x *= resolution.x / resolution.y;
	
	//TexCoord /= resolution;
	
	//TexCoord /= 16000.0;
	
	gl_Position = vec4(aPos, 0.0, 1.0);
}