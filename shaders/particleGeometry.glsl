#version 330 core

layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

in float radius[];
in vec3 color[];

out vec3 col;
out vec2 FragPos;
out float rad;

uniform mat4 projection;

//turns out traingle strips are special or smth
void main()
{
	col = color[0];
	rad = radius[0];
	
	//Upper triangle
	
	FragPos = vec2(-1.0, 1.0);
	vec4 p = vec4(-radius[0], radius[0], 0.0, 0.0);
	gl_Position = projection * (gl_in[0].gl_Position + p);
	EmitVertex();
	
	FragPos = vec2(1.0, 1.0);
	p = vec4(radius[0], radius[0], 0.0, 0.0);
	gl_Position = projection * (gl_in[0].gl_Position + p);
	EmitVertex();
	
	FragPos = vec2(-1.0, -1.0);
	p = vec4(-radius[0], -radius[0], 0.0, 0.0);
	gl_Position = projection * (gl_in[0].gl_Position + p);
	EmitVertex();
	
	//Down triangle
	
	/*FragPos = vec2(-1.0, -1.0);
	p = vec4(-radius[0], -radius[0], 0.0, 0.0);
	gl_Position = projection * (gl_in[0].gl_Position + p);
	EmitVertex(); */
	
	FragPos = vec2(1.0, -1.0);
	p = vec4(radius[0], -radius[0], 0.0, 0.0);
	gl_Position = projection * (gl_in[0].gl_Position + p);
	EmitVertex();
	
	/* FragPos = vec2(1.0, 1.0);
	p = vec4(radius[0], radius[0], 0.0, 0.0);
	gl_Position = projection * (gl_in[0].gl_Position + p);
	EmitVertex(); */
	
	EndPrimitive();
}