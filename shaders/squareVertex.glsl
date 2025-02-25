#version 330 core

layout (location = 0) in vec2 aPos;

out vec2 endPos;

uniform mat4 view;

uniform vec2 startPos;
uniform vec2 endPosU;

void main()
{
	endPos = (view * vec4(endPosU, 0.0, 1.0)).xy;
	gl_Position = view * vec4(startPos + aPos, 0.0, 1.0); //The position
}