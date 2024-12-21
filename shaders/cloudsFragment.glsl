#version 330 core
out vec4 FragColor;

in vec2 TexCoord;

uniform sampler2D tex;
uniform float iTime;

uniform mat4 projection;

void main()
{
	float time = iTime / 10.0;
	
	//vec2 wind = vec2(0.02*time + 0.1*cos(0.08*time+2.3) + 0.11*sin(0.05*time+0.5), 0.023*time + 0.1*cos(0.098*time+0.9) + 0.11*sin(0.034*time));
	vec2 wind = vec2(0.02 * time + 0.1 * cos(0.08 * time + 2.3) + 0.11 * sin(0.05 * time + 0.5) + 0.05 * sin(0.03 * time * cos(0.07 * time)) - 0.02 * cos(0.11 * time + 3.1) + 0.15 * sin(TexCoord.y - 3.7),
	0.023 * time + 0.1 * cos(0.098 * time + 0.9) + 0.11 * sin(0.034 * time) - 0.06 * sin(0.045 * time * sin(0.02 * time)) + 0.03 * cos(0.09 * time + 4.5) + 0.15 * cos(TexCoord.x + 9.9));
	
	vec2 coords = (vec4(TexCoord + wind, 0.0, 1.0)).xy;
	
	float noise = mix(texture(tex, coords).g, texture(tex, coords + vec2(0.24 + 0.002*time, 0.59 - 0.0018*time)).g, 0.5 + 0.2*cos(0.09*time));
	
	float serp = smoothstep(0.8, 1.0, texture(tex, coords + vec2(0.24 + 0.002*time, 0.59 - 0.0018*time)).g);
	
	float mm = 0.52 + 0.008 * sin(0.65*time + 0.3) - 0.1 * serp;
	float mx = 0.7 + 0.04 * sin(0.73*time);
	float xm = 0.82 + 0.03 * cos(0.42*time);
	float xx = 0.9 + 0.03 * sin(0.81*time) + 0.02 * cos(0.16 * time * coords.y);
	
	float alpha = smoothstep(mm, mx, noise) * (1.0 - smoothstep(xm, xx, noise)) * (0.1 + 0.03 * sin(0.15*time));
	
	FragColor = vec4(0.8, 0.8, 0.8, alpha);
} 