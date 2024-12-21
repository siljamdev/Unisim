#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 1) in float aRadius;
layout (location = 2) in vec3 aCol;

out vec3 color;
out float radius;

uniform mat4 view;
uniform float zoom;

void main()
{
	gl_Position = view * vec4(aPos, 0.0, 1.0); //The position
	
	color = aCol;
	radius = zoom * aRadius;
}