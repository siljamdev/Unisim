using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using AshLib;

public class Shader{
	
	static int activeShader;
	
	public int id {get; private set;}
	
	public Shader(string vertex, string fragment, string? geometry) {
		int vertexShader = GL.CreateShader(ShaderType.VertexShader);
		GL.ShaderSource(vertexShader, vertex);
		GL.CompileShader(vertexShader);
		
		int compileStatus;
		GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out compileStatus);
		if(compileStatus == 0){
			string log = GL.GetShaderInfoLog(vertexShader);
			throw new Exception("GLSL VERTEX SHADER COMPILING ERROR:\n" + log);
		}
		
		int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
		GL.ShaderSource(fragmentShader, fragment);
		GL.CompileShader(fragmentShader);
		
		GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out compileStatus);
		if(compileStatus == 0){
			string log = GL.GetShaderInfoLog(fragmentShader);
			throw new Exception("GLSL FRAGMENT SHADER COMPILING ERROR:\n" + log);
		}
		
		int geometryShader = 0;
		if(geometry != null){
			geometryShader = GL.CreateShader(ShaderType.GeometryShader);
			GL.ShaderSource(geometryShader, geometry);
			GL.CompileShader(geometryShader);
			
			GL.GetShader(geometryShader, ShaderParameter.CompileStatus, out compileStatus);
			if(compileStatus == 0){
				string log = GL.GetShaderInfoLog(geometryShader);
				throw new Exception("GLSL GEOMETRY SHADER COMPILING ERROR:\n" + log);
			}
		}
		
		this.id = GL.CreateProgram();
		GL.AttachShader(this.id, vertexShader);
		GL.AttachShader(this.id, fragmentShader);
		if(geometry != null){
			GL.AttachShader(this.id, geometryShader);
		}
		GL.LinkProgram(this.id);
		
		int linkStatus;
		GL.GetProgram(this.id, GetProgramParameterName.LinkStatus, out linkStatus);
		
		if(linkStatus == (int)All.False){
			string log = GL.GetProgramInfoLog(this.id);
			Console.WriteLine("GLSL SHADER LINKING ERROR:\n" + log);
		}
		
		GL.ValidateProgram(this.id);
		
		int validateStatus;
		GL.GetProgram(this.id, GetProgramParameterName.ValidateStatus, out validateStatus);
		
		if(validateStatus == (int)All.False){
			string log = GL.GetProgramInfoLog(this.id);
			Console.WriteLine("GLSL SHADER VALIDATING ERROR:\n" + log);
		}
		
		GL.DetachShader(this.id, vertexShader);
		GL.DetachShader(this.id, fragmentShader);
		if(geometry != null) {
	    	GL.DetachShader(this.id, geometryShader);
	    }
		
		GL.DeleteShader(vertexShader);
	    GL.DeleteShader(fragmentShader);
	    if(geometry != null) {
	    	GL.DeleteShader(geometryShader);
	    }
	}
	
	public static Shader generateFromAssembly(string name){
		string v = AssemblyFiles.getText("shaders." + name + "Vertex.glsl");
		string f = AssemblyFiles.getText("shaders." + name + "Fragment.glsl");
		string g = null;
		if(AssemblyFiles.exists("shaders." + name + "Geometry.glsl")){
			g = AssemblyFiles.getText("shaders." + name + "Geometry.glsl");
		}
		
		return new Shader(v, f, g);
	}
	
	public void use(){
		if(activeShader == this.id){
			return;
		}
		GL.UseProgram(this.id);
		activeShader = this.id;
	}
	
	public static void stopUsing(){
		GL.UseProgram(0);
		activeShader = 0;
	}
	
	public void setBool(string name, bool data){
		this.use();
		GL.Uniform1(GL.GetUniformLocation(this.id, name), data ? 1 : 0);
	}
	
	public void setInt(string name, int data){
		this.use();
		GL.Uniform1(GL.GetUniformLocation(this.id, name), data);
	}
	
	public void setIntArray(string name, int[] data){
		this.use();
		GL.Uniform1(GL.GetUniformLocation(this.id, name), data.Length, data);
	}
	
	public void setFloat(string name, float data){
		this.use();
		GL.Uniform1(GL.GetUniformLocation(this.id, name), data);
	}
	
	public void setMatrix4(string name, Matrix4 data){
		this.use();
		GL.UniformMatrix4(GL.GetUniformLocation(this.id, name), false, ref data);
	}
	
	public void setVector4(string name, Vector4 data){
		this.use();
		GL.Uniform4(GL.GetUniformLocation(this.id, name), data.X, data.Y, data.Z, data.W);
	}
	
	public void setVector4(string name, Color3 data, float a){
		this.use();
		GL.Uniform4(GL.GetUniformLocation(this.id, name), (float)data.R / 255f, (float)data.G / 255f, (float)data.B / 255f, a);
	}
	
	public void setVector3(string name, Color3 data){
		this.use();
		GL.Uniform3(GL.GetUniformLocation(this.id, name), (float)data.R / 255f, (float)data.G / 255f, (float)data.B / 255f);
	}
	
	public void setVector3(string name, Vector3 data){
		this.use();
		GL.Uniform3(GL.GetUniformLocation(this.id, name), data.X, data.Y, data.Z);
	}
	
	public void setVector2(string name, Vector2 data){
		this.use();
		GL.Uniform2(GL.GetUniformLocation(this.id, name), data.X, data.Y);
	}
	
	public void setVector2i(string name, Vector2i data){
		this.use();
		GL.Uniform2(GL.GetUniformLocation(this.id, name), data.X, data.Y);
	}
}