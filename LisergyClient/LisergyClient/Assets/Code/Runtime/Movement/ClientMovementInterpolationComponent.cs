using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using Game.ECS;
using Game.Tile;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Assets.Code.Runtime.Movement
{
    /// <summary>
    /// Client side component to handle movement interpolation
    /// </summary>
    public class ClientMovementInterpolationComponent : IReferenceComponent
    {
        internal List<TileEntity> InterpolingPath;
        internal TweenerCore<Vector3, Path, PathOptions> TweenPath;
    }
}
