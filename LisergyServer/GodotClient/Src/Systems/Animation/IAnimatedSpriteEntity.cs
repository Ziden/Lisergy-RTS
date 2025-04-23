using Game.World;

namespace LisergyGodotClient.Src.Systems.Animation;

public interface IAnimatedSpriteEntity
{
	void UpdateAnimation(Direction d, bool moving);
}