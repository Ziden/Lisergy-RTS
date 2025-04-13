using Stride.Engine;
using Stride.Core.Mathematics;
using Stride.Games;
using Stride.Rendering;

class MyGame : Game
{
	private ModelComponent cubeModel;

	protected override void Initialize()
	{
		base.Initialize();

		var cubeModel = ModelFactory.CreateCube(GraphicsDevice);
		
		// Setup a basic scene with a cube
		var cubeEntity = new Entity
		{
			new ModelComponent
			{
				Model = Content.Load<Model>("Models/Cube")
			}
		};

		// Add camera
		var cameraEntity = new Entity("Camera")
		{
			new CameraComponent()
		};
		SceneSystem.SceneInstance.RootScene.Entities.Add(cubeEntity);
		SceneSystem.SceneInstance.RootScene.Entities.Add(cameraEntity);
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		// Rotate cube
		var entity = SceneSystem.SceneInstance.RootScene.Entities[0];
		entity.Transform.Rotation *= Quaternion.RotationY(1f * (float)gameTime.Elapsed.TotalSeconds);
	}
}

class Program
{
	static void Main(string[] args)
	{
		using (var game = new MyGame())
		{
			game.Run();
		}
	}
}