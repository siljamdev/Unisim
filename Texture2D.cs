using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using StbImageSharp;

public class Texture2D{
	
	static int activeTexture;
	
	public int id{get; private set;}
	
	public int width{get; private set;}
	public int height{get; private set;}
	
	public PixelInternalFormat internalFormat{get; private set;}
	
	public Texture2D(ImageResult image, TextureParams tp){	
		this.internalFormat = PixelInternalFormat.Rgba8;
		
		this.width = image.Width; //Extract this needed values
		this.height = image.Height;
		
		this.id = GL.GenTexture(); //Generate the handle for the texture
		GL.BindTexture(TextureTarget.Texture2D, this.id); //bind it
		
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) tp.wrapS); //Set the wrap parameters
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) tp.wrapT);
		
		if(tp.wrapS == TextureWrapMode.ClampToBorder || tp.wrapT == TextureWrapMode.ClampToBorder){
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[]{tp.borderColor.X, tp.borderColor.Y, tp.borderColor.Z}); //if needed set the border color
		}
		
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) tp.filterMin); //set upscaling/downscaling filter options
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) tp.filterMax);

		
		GL.TexImage2D(TextureTarget.Texture2D, 0, this.internalFormat, this.width, this.height, 0, tp.imageFormat, PixelType.UnsignedByte, image.Data); //actually generate the texture
		
		//if needed, generate mipmaps
		if(tp.filterMin == TextureMinFilter.NearestMipmapNearest || tp.filterMin == TextureMinFilter.LinearMipmapNearest || tp.filterMin == TextureMinFilter.NearestMipmapLinear || tp.filterMin == TextureMinFilter.LinearMipmapLinear){
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}
    	
    	GL.BindTexture(TextureTarget.Texture2D, 0); //unbind texture
		activeTexture = 0;
	}
	
	public static Texture2D generateFromFile(string path, TextureParams tp){
		ImageResult image = ImageResult.FromMemory(File.ReadAllBytes(path), ConvertFormat(tp.imageFormat));
		if (image == null || image.Data == null){
			throw new Exception("Image loading failed from:" + path);
		}
		Texture2D t = new Texture2D(image, tp);
		return t;
	}
	
	public static Texture2D generateFromBytes(byte[] data, TextureParams tp){
		ImageResult image = ImageResult.FromMemory(data, ConvertFormat(tp.imageFormat));
		if (image == null || image.Data == null){
			throw new Exception("Image loading failed byte array");
		}
		Texture2D t = new Texture2D(image, tp);
		return t;
	}
	
	public static Texture2D generateFromAssembly(string name, TextureParams tp){
		byte[] data = AssemblyFiles.get("res.textures." + name);
		ImageResult image = ImageResult.FromMemory(data, ConvertFormat(tp.imageFormat));
		if (image == null || image.Data == null){
			throw new Exception("Image loading failed byte array");
		}
		Texture2D t = new Texture2D(image, tp);
		return t;
	}
	
	static ColorComponents ConvertFormat(PixelFormat pixelFormat){
        switch (pixelFormat){
            case PixelFormat.Rgb:
                return ColorComponents.RedGreenBlue;
            case PixelFormat.Rgba:
                return ColorComponents.RedGreenBlueAlpha;
            // Add more cases for other pixel formats as needed
            default:
                throw new ArgumentException("Unsupported pixel format in conversion");
        }
    }
	
	public void bind(){
		if(activeTexture == this.id){
			return;
		}
		GL.ActiveTexture(TextureUnit.Texture0);
		GL.BindTexture(TextureTarget.Texture2D, this.id);
		activeTexture = this.id;
	}
	
	public static void unbind(){
		GL.ActiveTexture(TextureUnit.Texture0);
		GL.BindTexture(TextureTarget.Texture2D, 0);
		activeTexture = 0;
	}
}

public struct TextureParams{
	
	public static readonly TextureParams Default = new TextureParams();
	public static readonly TextureParams Smooth = new TextureParams(){filterMin = TextureMinFilter.LinearMipmapLinear, filterMax = TextureMagFilter.Linear};
	public static readonly TextureParams Noise = new TextureParams(){filterMin = TextureMinFilter.NearestMipmapLinear, filterMax = TextureMagFilter.Linear, wrapS = TextureWrapMode.MirroredRepeat, wrapT = TextureWrapMode.MirroredRepeat};
	
	public PixelFormat imageFormat = PixelFormat.Rgba;
	
	public TextureWrapMode wrapS = TextureWrapMode.Repeat;
	public TextureWrapMode wrapT = TextureWrapMode.Repeat;
	public Vector3 borderColor = new Vector3(0f, 0f, 0f);
	
	public TextureMinFilter filterMin = TextureMinFilter.NearestMipmapNearest;
	public TextureMagFilter filterMax = TextureMagFilter.Nearest;
	
	public TextureParams(){ //Constructor for setting defaults

	}
}