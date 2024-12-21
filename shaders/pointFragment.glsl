#version 330 core

out vec4 FragColor;

in vec3 col;

void main()
{
	vec2 coord = gl_PointCoord - vec2(0.5); // Center the coordinate system

    if (length(coord) > 0.5){
        discard;
	}
	
	FragColor = vec4(col, 0.5);
}