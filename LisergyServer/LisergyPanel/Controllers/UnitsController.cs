using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameData.Specs;
using Microsoft.AspNetCore.Mvc;

namespace LisergyPanel.Controllers
{
    [Route("api/[controller]")]
    public class UnitsController : Controller
    {
        [HttpGet("[action]")]
        public List<UnitSpec> GetSpecs()
        {
            return SpecStorage.GameSpecs.Units.Values.ToList();
        }
    }
}
