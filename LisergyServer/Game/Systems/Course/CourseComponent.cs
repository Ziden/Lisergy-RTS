﻿using Game.DataTypes;
using Game.ECS;
using System;

namespace Game.Systems.Movement
{
    public struct CourseComponent : IComponent
    {
        public GameId CourseId;
        public TimeSpan MoveDelay;
        public CourseIntent MovementIntent;

        public override string ToString()
        {
            return $"<EntityMovementComponent Course={CourseId} MoveDelay={MoveDelay.TotalSeconds}>";
        }
    }
}