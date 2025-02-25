#version 330 core

layout (points) in;
layout (line_strip, max_vertices = 5) out;

in vec2 endPos[];

uniform mat4 projection;

//turns out traingle strips are special or smth
void main()
{	
	vec4 p = gl_in[0].gl_Position;
	gl_Position = projection * p;
	EmitVertex();
	
	p = vec4(endPos[0].x, gl_in[0].gl_Position.y, 0.0, 1.0);
	gl_Position = projection * p;
	EmitVertex();
	
	p = vec4(endPos[0].x, endPos[0].y, 0.0, 1.0);
	gl_Position = projection * p;
	EmitVertex();
	
	
	p = vec4(gl_in[0].gl_Position.x, endPos[0].y, 0.0, 1.0);
	gl_Position = projection * p;
	EmitVertex();
	
	//Repeat last vertex so loop is closed
	p = gl_in[0].gl_Position;
	gl_Position = projection * p;
	EmitVertex();
	
	EndPrimitive();
}