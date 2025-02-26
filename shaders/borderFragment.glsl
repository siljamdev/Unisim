#version 330 core
out vec4 FragColor;

in vec2 WorldPos;

uniform vec2 size;

uniform vec3 color;

float sdBox(in vec2 p, in vec2 b)
{
    vec2 d = abs(p)-b;
    return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
}

void main()
{
	float d = sdBox(WorldPos, size);
	if(d <= 0.0){
		discard;
	}
	float alpha = 4.5 / (d + 5.0) + 0.1;
	FragColor = vec4(color, alpha);
} 