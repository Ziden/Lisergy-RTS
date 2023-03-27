using System.Collections;
using System.Text;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using YamlDotNet.Serialization;

namespace FlgImageOptimizer
{
	/// <summary>
	/// Runs trough Art & Addressable assets to:
	/// - Compress .png and .tga files pixel formats
	/// - Update those textures unity metadata for even more compression
	/// - Scan for meshes metadata and trigger bone optimizations & compression for reduced GPU cost & memory usage
	/// - Required Libraries: YamlDotNet & ImageSharp
	/// </summary>
	class Program
	{
		public static readonly string ART_FOLDER = "/Users/gabrielslomka/code/blast-royale/Assets/Art";
		public static readonly string ADDRESSABLES = "/Users/gabrielslomka/code/blast-royale/Assets/AddressableResources";

		static void Main(string[] args)
		{
			foreach (var file in Directory.GetFiles(ADDRESSABLES, "*.*", SearchOption.AllDirectories))
			{
				try
				{
					ProcessFile(file);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
				}
			}

			foreach (var file in Directory.GetFiles(ART_FOLDER, "*.*", SearchOption.AllDirectories))
			{
				try
				{
					ProcessFile(file);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
				}
			}
		}

		/// <summary>
		/// Process given file. Will check what to do based on file extension
		/// </summary>
		private static void ProcessFile(string file)
		{
			if (file.Contains(".meta"))
			{
				if (file.Contains(".png") || file.Contains(".tga"))
				{
					CompressImageMeta(file);
				}
				else if (file.Contains(".fbx"))
				{
					CompressModelMeta(file);
				}
				return;
			}
			if (file.Contains(".tga"))
			{
				CompressEncodeTga(file);
			}

			if (file.Contains(".png"))
			{
				CompressEncodePng(file);
			}
		}


		/// <summary>
		/// Changes unity max texture size and compression methods for the given unity texture importer
		/// </summary>
		public static void CompressImageMeta(string file)
		{
			var deserializer = new DeserializerBuilder().Build();
			var res = deserializer.Deserialize<dynamic>(File.ReadAllText(file));
			if (res["TextureImporter"] is IDictionary)
			{
				Console.WriteLine("Compressing texture " + file);
				res["TextureImporter"]["maxTextureSize"] = "512";
				res["TextureImporter"]["textureCompression"] = "3";
				res["TextureImporter"]["crunchedCompression"] = "1";
				var platformSettings = res["TextureImporter"]["platformSettings"];
				foreach (var setting in platformSettings)
				{
					setting["textureCompression"] = "3";
					setting["crunchedCompression"] = "1";
					setting["maxTextureSize"] = "512";
				}
				Console.WriteLine("Updating " + file);
				var serializer = new SerializerBuilder().Build();
				var yaml = (string)serializer.Serialize(res);
				File.Delete(file);
				File.WriteAllText(file, yaml, Encoding.UTF8);
			}
		}

		/// <summary>
		/// Toggles bone optimization and compress meshes of the given unity model importer
		/// </summary>
		public static void CompressModelMeta(string file)
		{
			var deserializer = new DeserializerBuilder().Build();
			var res = deserializer.Deserialize<dynamic>(File.ReadAllText(file));
			if (res["ModelImporter"] is IDictionary)
			{
				Console.WriteLine("Optimizing model " + file);
				res["ModelImporter"]["animations"]["animationCompression"] = "3";
				res["ModelImporter"]["meshes"]["meshCompression"] = "3";
				res["ModelImporter"]["meshes"]["optimizeBones"] = "1";
				Console.WriteLine("Updating " + file);
				var serializer = new SerializerBuilder().Build();
				var yaml = (string)serializer.Serialize(res);
				File.Delete(file);
				File.WriteAllText(file, yaml, Encoding.UTF8);
			}
		}

		/// <summary>
		/// Encodes a Tga with RunLength compression & pixel depth of 16 bits
		/// </summary>
		private static void CompressEncodeTga(string file)
		{
			Console.WriteLine("compressing tga " + file);
			var tgaE = new TgaEncoder()
			{
				Compression = TgaCompression.RunLength,
				BitsPerPixel = TgaBitsPerPixel.Pixel16
			};
			using (var image = Image.Load(file))
			{
				image.SaveAsTga(file, tgaE);
			}
		}

		/// <summary>
		/// Encodes a png with 2bit detph and with pixel compression
		/// </summary>
		private static void CompressEncodePng(string file)
		{
			Console.WriteLine("compressing png " + file);
			using (var image = Image.Load(file))
			{
				image.SaveAsPng(file, new PngEncoder()
				{
					CompressionLevel = PngCompressionLevel.BestCompression,
					BitDepth = PngBitDepth.Bit2,
					Quantizer = new OctreeQuantizer(),
					SkipMetadata = true
				});
			}
		}
	}
}