using Godot;
using System.Collections.Generic;
using GameData;
using LisergyGodotClient.Src.Services;
using GameDataTest;
using GodotClient;
using Game.Engine;
using System;
using LisergyGodotClient.Src.Platform;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.TechTree;
using ClientSDK;
using System.Linq;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src.Systems.Building
{
    public partial class TechTreeVisualLayout<T>
    {
        private const int NodeSize = 140;
        private readonly int IconSize = (int)Mathf.Round(NodeSize * 0.8);
        private const int HorizontalSpacing = 200;
        private const int VerticalSpacing = 220;

        public Func<NodeTree<T>, Task<TechTreeItemWidget>> CreateWidget;
        private IGameObject _root;
        private Theme _theme;
        private Control _techTreeContainer;
        public ScrollContainer ScrollContainer;
        private Dictionary<T, Vector2> _nodePositions = new Dictionary<T, Vector2>();


        public async Task Draw(IGameObject rootObject, NodeTree<T> rootNode)
        {
            ScrollContainer = new ScrollContainer();
            _root = rootObject;
            ScrollContainer.Theme = _theme;
            ScrollContainer.CustomMinimumSize = new Vector2(1000, 600);
            ScrollContainer.AnchorRight = 1.0f;
            ScrollContainer.AnchorBottom = 1.0f;
            ScrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.ShowAlways;
            ScrollContainer.VerticalScrollMode = ScrollContainer.ScrollMode.ShowAlways;
            rootObject.GetNode().AddChild(ScrollContainer);

            _techTreeContainer = new Control();
            _techTreeContainer.Theme = _theme;
            _techTreeContainer.CustomMinimumSize = new Vector2(3000, 2000);
            ScrollContainer.AddChild(_techTreeContainer);

            await PopulateTechTree(rootNode);
        }

        private void CenterViewOnRoot()
        {
            // Find the root position
            if (_nodePositions.Count > 0)
            {
                var rootPos = _nodePositions.First().Value;

                // Center scrolling on the root node
                ScrollContainer.ScrollHorizontal = (int)(rootPos.X - ScrollContainer.Size.X / 2 + NodeSize / 2);
                ScrollContainer.ScrollVertical = (int)(rootPos.Y - ScrollContainer.Size.Y / 2 + NodeSize / 2);
            }
        }

        private async Task CreateTimer(float seconds, Action callback)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            callback();
        }

        private async Task PopulateTechTree(NodeTree<T> root)
        {
            // Calculate tree dimensions first to position it properly
            Dictionary<T, Vector2> tempPositions = new Dictionary<T, Vector2>();
            CalculateNodePositionsTemp(root, Vector2.Zero, tempPositions);

            // Find the bounds of the tech tree
            Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);

            foreach (var pos in tempPositions.Values)
            {
                minPos.X = Mathf.Min(minPos.X, pos.X);
                minPos.Y = Mathf.Min(minPos.Y, pos.Y);
                maxPos.X = Mathf.Max(maxPos.X, pos.X + NodeSize);
                maxPos.Y = Mathf.Max(maxPos.Y, pos.Y + NodeSize);
            }

            // Calculate the center offset to position the tree in the middle of the container
            Vector2 treeSize = maxPos - minPos;
            Vector2 centerOffset = new Vector2(
                (_techTreeContainer.CustomMinimumSize.X - treeSize.X) / 2 - minPos.X,
                (_techTreeContainer.CustomMinimumSize.Y - treeSize.Y) / 2 - minPos.Y
            );

            // Ensure the offset is in whole pixels
            centerOffset = new Vector2(Mathf.Round(centerOffset.X), Mathf.Round(centerOffset.Y));

            // First pass: Calculate positions for all nodes based on their level in the tree
            CalculateNodePositions(root, centerOffset);

            // Second pass: Draw connection lines between nodes
            DrawConnectionLines(root);

            try
            {
                // Third pass: Create the actual building nodes
                await CreateBuildingNodes(root);
                // Center the view on the root node

                _ = CreateTimer(0.1f, CenterViewOnRoot);
            }
            catch (Exception e)
            {
                ClientServices.Analytics.TrackError(e);
            }
        }

        private void CalculateNodePositionsTemp(NodeTree<T> node, Vector2 position, Dictionary<T, Vector2> positions)
        {
            // Store this node's position
            positions[node.Data] = position;

            // Calculate positions for children
            var children = node.ChildrenNodes();
            int childCount = children.Count;

            if (childCount > 0)
            {
                float totalWidth = (childCount - 1) * HorizontalSpacing;
                float startX = position.X - totalWidth / 2;

                int i = 0;
                foreach (var child in children)
                {
                    float childX = startX + i * HorizontalSpacing;
                    float childY = position.Y + VerticalSpacing;
                    CalculateNodePositionsTemp(child, new Vector2(childX, childY), positions);
                    i++;
                }
            }
        }

        private void CalculateNodePositions(NodeTree<T> node, Vector2 position)
        {
            // Round position to whole pixels to avoid sub-pixel rendering issues
            position = new Vector2(Mathf.Round(position.X), Mathf.Round(position.Y));

            // Store this node's position
            _nodePositions[node.Data] = position;

            // Calculate positions for children
            var children = node.ChildrenNodes();
            int childCount = children.Count;

            if (childCount > 0)
            {
                float totalWidth = (childCount - 1) * HorizontalSpacing;
                float startX = position.X - totalWidth / 2;

                int i = 0;
                foreach (var child in children)
                {
                    float childX = startX + i * HorizontalSpacing;
                    float childY = position.Y + VerticalSpacing;
                    // Round child position to whole pixels
                    Vector2 childPosition = new Vector2(Mathf.Round(childX), Mathf.Round(childY));
                    CalculateNodePositions(child, childPosition);
                    i++;
                }
            }
        }

        private void DrawConnectionLines(NodeTree<T> node)
        {
            foreach (var child in node.ChildrenNodes())
            {
                Vector2 startPos = _nodePositions[node.Data] + new Vector2(NodeSize / 2, NodeSize / 2);
                Vector2 endPos = _nodePositions[child.Data] + new Vector2(NodeSize / 2, NodeSize / 2);

                // Round positions to whole pixels
                startPos = new Vector2(Mathf.Round(startPos.X), Mathf.Round(startPos.Y));
                endPos = new Vector2(Mathf.Round(endPos.X), Mathf.Round(endPos.Y));

                // Create squared-off connection with three segments
                var line = new Line2D();
                line.Width = 4;
                line.Antialiased = true;

                line.DefaultColor = new Color(0.7f, 1f, 0.7f); // Light grey

                // Starting point
                line.AddPoint(startPos);

                // Go vertical half-way
                float midY = startPos.Y + (endPos.Y - startPos.Y) / 2;

                // Round to whole pixels
                midY = Mathf.Round(midY);
                line.AddPoint(new Vector2(startPos.X, midY));

                // Go horizontal
                line.AddPoint(new Vector2(endPos.X, midY));
                line.AddPoint(endPos);

                _techTreeContainer.AddChild(line);
                DrawConnectionLines(child);
            }
        }

        private async Task CreateBuildingNodes(NodeTree<T> node)
        {
            Vector2 position = _nodePositions[node.Data];
            var container = await CreateWidget(node);
            container.Position = position;
            container.CustomMinimumSize = new Vector2(NodeSize, NodeSize);
            _techTreeContainer.AddChild(container);
            if(node.IsRoot)
            {
                container.OnClick?.Invoke(container);
            }
            foreach (var child in node.ChildrenNodes())
            {
                await CreateBuildingNodes(child);
            }
        }
    }
}
