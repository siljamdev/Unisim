#version 330 core

out vec4 FragColor;

in vec2 FragPos;

uniform vec3 col;

void main()
{
	if(length(FragPos) > 1.0){
		discard;
	}
	
	FragColor = vec4(col, 0.4);
} 