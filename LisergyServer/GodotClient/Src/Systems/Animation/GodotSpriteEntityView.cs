using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.World;
using Godot;

namespace LisergyGodotClient.Src.Systems.Animation
{
    /// <summary>
    /// Entity view composed of an animated sprite
    /// </summary>
    public class GodotSpriteEntityView : EntityView, IAnimatedSpriteEntity
    {
        private AnimatedSprite3D _sprite;

        public GodotSpriteEntityView(IEntity entity, IClientSDK client) : base(entity, client)
        {
            this.RunWhenRendered(() =>
            {
                _sprite = GameObject.Get<AnimatedSprite3D>();
            });
        }

        public void UpdateAnimation(Direction d, bool moving)
        {
            var name = (moving ? "walk" : "iddle") + "_";
            if (d == Direction.NORTH)
            {
                _sprite.FlipH = true;
                name += "se";
            }
            else if(d == Direction.SOUTH)
            {
                _sprite.FlipH = true;
                name += "nw";
            }
            else if (d == Direction.EAST)
            {
                _sprite.FlipH = false;
                name += "se";
            }
            else if (d == Direction.WEST)
            {
                _sprite.FlipH = false;
                name += "nw";
            }  
            if(_sprite.IsPlaying() && _sprite.Animation == name)
            {
                return;
            }
            _sprite.Stop();
            _sprite.Play(name);
        }
    }
}
