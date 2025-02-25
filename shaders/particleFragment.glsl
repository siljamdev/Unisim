#version 330 core

out vec4 FragColor;

in vec3 col;
in vec2 FragPos;
in float rad;

void main()
{
	if(length(FragPos) > 1.0){
		discard;
	}
	
	float alpha = smoothstep(1.0, 1.0 - 1.0/rad, length(FragPos));
	
	FragColor = vec4(col, alpha);
} 