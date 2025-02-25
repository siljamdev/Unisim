#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 2) in vec3 aCol;

out vec3 col;

uniform mat4 view;
uniform mat4 projection;

uniform float pointSize;

void main()
{
	gl_Position = projection * view * vec4(aPos, 0.0, 1.0); //The position
	gl_PointSize = pointSize;
	
	col = aCol;
}