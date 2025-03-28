using Godot;
using ClientSDK;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.Map;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using System;
using System.Linq;
using ClientSDK.Data;
using GodotClient;
using Cysharp.Threading.Tasks;

namespace LisergyGodotClient.Src
{
    /// <summary>
    /// Method extensions to make client life a bit easier
    /// </summary>
    public static class ClientExtensions
    {
        /// <summary>
        /// Checks if the given id is the id of the local player
        /// </summary>
        public static bool IsMine(this GameId id) => ClientServices.ServerSdk.Server.Player.PlayerId == id;

        /// <summary>
        /// Checks if a given entity belongs to the local player
        /// </summary>
        public static bool IsMine(this IEntity e) => e.OwnerID.IsMine();

        /// <summary>
        /// Checks if a given tile is visible to the local player
        /// </summary>
        public static bool IsVisible(this IEntity tile) => tile != null && tile.Logic.Vision.GetEntitiesViewing().Any(p => p.IsMine());

        /// <summary>
        /// Gets the Godot position of a given entity
        /// </summary>
        public static Vector3 GodotPosition(this IEntity entity) => new Vector3(entity.GetTile().X, 0, entity.GetTile().Y);

        /// <summary>
        /// Gets the Godot position of a given tile
        /// </summary>
        public static Vector3 GodotPosition(this TileModel entity) => new Vector3(entity.X, 0, entity.Y);

        /// <summary>
        /// Gets the tile of a given entity
        /// Entity must have <see cref="MapPlacementComponent"/> component
        /// </summary>
        public static TileModel GetTile(this IEntity entity)
        {
            return entity.Logic.Map.GetTile();
        }

        /// <summary>
        /// Gets the position of an entity
        /// Entity must have <see cref="MapPlacementComponent"/> component
        /// </summary>
        public static Location GetPosition(this IEntity entity)
        {
            if (entity.EntityType == EntityType.Tile) return entity.Get<TileDataComponent>().Position;
            return entity.Get<MapPlacementComponent>().Position;
        }

        /// <summary>
        /// Validates a given object is not null
        /// </summary>
        public static T Required<T>(this T element)
        {
            if (element == null) throw new Exception($"Validation error: {typeof(T)} cannot be null");
            return element;
        }

        public static IEntityView GetView(this IEntity entity)
        {
            return ClientServices.ServerSdk.Server.Views.GetEntityView(entity);
        }

        public static T GetView<T>(this IEntity entity) => (T)entity.GetView();

        #region System
        public static string ToReadableString(this TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} d ", span.Days) : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} h ", span.Hours) : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} m ", span.Minutes) : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} s", span.Seconds) : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 s";

            return formatted;
        }
        #endregion

        #region UI
        /// <summary>
        /// Hides a tab without removing it from the TabContainer
        /// </summary>
        /// <param name="tabs">The TabContainer</param>
        /// <param name="tabIndex">Index of the tab to hide</param>
        public static void HideTab(this TabContainer tabs, int tabIndex)
        {
            if (tabs == null || tabIndex < 0 || tabIndex >= tabs.GetTabCount()) return;

            // Get the control at the specified tab index
            Control tabContent = tabs.GetTabControl(tabIndex);
            if (tabContent != null)
            {
                // Hide the tab content but keep it in the hierarchy
                tabContent.Visible = false;

                // This sets the tab as disabled in the tab bar
                tabs.SetTabDisabled(tabIndex, true);
            }
        }

        /// <summary>
        /// Shows a previously hidden tab
        /// </summary>
        /// <param name="tabs">The TabContainer</param>
        /// <param name="tabIndex">Index of the tab to show</param>
        public static void ShowTab(this TabContainer tabs, int tabIndex)
        {
            if (tabs == null || tabIndex < 0 || tabIndex >= tabs.GetTabCount()) return;

            // Get the control at the specified tab index
            Control tabContent = tabs.GetTabControl(tabIndex);
            if (tabContent != null)
            {
                // Show the tab content
                tabContent.Visible = true;

                // Enable the tab in the tab bar
                tabs.SetTabDisabled(tabIndex, false);
            }
        }

        /// <summary>
        /// Hides a tab by name without removing it from the TabContainer
        /// </summary>
        /// <param name="tabs">The TabContainer</param>
        /// <param name="tabName">Name of the tab to hide</param>
        public static void HideTabByName(this TabContainer tabs, string tabName)
        {
            if (tabs == null) return;

            for (int i = 0; i < tabs.GetTabCount(); i++)
            {
                if (tabs.GetTabTitle(i) == tabName)
                {
                    HideTab(tabs, i);
                    return;
                }
            }
        }

        /// <summary>
        /// Shows a previously hidden tab by name
        /// </summary>
        /// <param name="tabs">The TabContainer</param>
        /// <param name="tabName">Name of the tab to show</param>
        public static void ShowTabByName(this TabContainer tabs, string tabName)
        {
            if (tabs == null) return;

            for (int i = 0; i < tabs.GetTabCount(); i++)
            {
                if (tabs.GetTabTitle(i) == tabName)
                {
                    ShowTab(tabs, i);
                    return;
                }
            }
        }

        // I also noticed you had a duplicate RemoveTabByName method, so here's just one:
        public static void RemoveTabByName(this TabContainer tabs, string tabName)
        {
            if (tabs == null) return;

            for (int i = 0; i < tabs.GetTabCount(); i++)
            {
                if (tabs.GetTabTitle(i) == tabName)
                {
                    RemoveTab(tabs, i);
                    return;
                }
            }
        }

        public static void RemoveTab(this TabContainer tabs, int tabIndex)
        {
            if (tabs == null || tabIndex < 0 || tabIndex >= tabs.GetTabCount()) return;

            // Get the control at the specified tab index
            Control tabContent = tabs.GetTabControl(tabIndex);
            if (tabContent != null)
            {
                // Remove it from tab container
                tabs.RemoveChild(tabContent);
                tabContent.QueueFree();
            }
        }
        #endregion

        #region UniTask
        public static void ForgetHandled(this UniTask task)
        {
            try
            {
                task.Forget();
            }
            catch (Exception e)
            {
                ClientServices.Analytics.TrackError(e);
            }
        }

        public static void ForgetHandled<T>(this UniTask<T> task)
        {
            try
            {
                task.Forget();
            }
            catch (Exception e)
            {
                ClientServices.Analytics.TrackError(e);
            }
        }


        public static void ForgetHandled(this UniTaskVoid task)
        {
            try
            {
                task.Forget();
            }
            catch (Exception e)
            {
                ClientServices.Analytics.TrackError(e);
            }
        }
        #endregion

        #region Position Conversions
        /// <summary>
        /// Converts a Location to a Vector2.
        /// </summary>
        public static Vector2 ToGodotVector2(this Location location)
        {
            return new Vector2(location.X, location.Y);
        }

        /// <summary>
        /// Converts a Location to a Vector3.
        /// </summary>
        public static Vector3 ToGodotVector3(this Location location, float z = 0)
        {
            return new Vector3(location.X, z, location.Y);
        }

        /// <summary>
        /// Creates a Location from a Vector2.
        /// </summary>
        public static Location ToLocation(this Vector2 vector)
        {
            return new Location((int)vector.X, (int)vector.Y);
        }

        /// <summary>
        /// Creates a Location from a Vector3.
        /// </summary>
        public static Location ToLocation(this Vector3 vector)
        {
            return new Location((int)vector.X, (int)vector.Y);
        }

        /// <summary>
        /// Converts a Location to a Transform3D.
        /// </summary>
        public static Transform3D ToTransform3D(this Location location, Basis basis, float y = 0)
        {
            return new Transform3D(basis, new Vector3(location.X, y, location.Y));
        }

        /// <summary>
        /// Creates a Location from a Transform3D.
        /// </summary>
        public static Location ToLocation(this Transform3D transform)
        {
            return new Location((int)transform.Origin.X, (int)transform.Origin.X);
        }
        #endregion

        #region Node
        public static Vector2 WorldToScreenPosition(this Camera3D camera, Vector3 worldPosition)
        {
            // Convert the 3D world position to a 2D screen position
            return camera.UnprojectPosition(worldPosition);
        }

        public static T GetNode<T>(this IGameObject o) where T : Node
        {
            return (T)((GodotGameObject)o).Node;
        }

        public static Node GetNode(this IGameObject o) 
        {
            return (Node)((GodotGameObject)o).Node;
        }

        public static Node GetNode(this IEntity o)
        {
            return o.GetView().GameObject.GetNode();
        }

        public static T FindFirstOfType<T>(this Node node) where T : class
        {
            if (node is T tt)
            {
                return tt;
            }
            foreach (Node child in node.GetChildren())
            {
                T result = child.FindFirstOfType<T>();
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        #endregion
    }
}
