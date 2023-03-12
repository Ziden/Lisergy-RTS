using Assets.Code.World;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public interface IClientEntity<ServerEntityType, ClientEntityType> where ServerEntityType : WorldEntity where ClientEntityType : WorldEntity, IGameObject
    {
        ClientEntityType UpdateData(ServerEntityType serverEntity);

        void InstantiateInScene(ServerEntityType serverEntity);

    }
}
