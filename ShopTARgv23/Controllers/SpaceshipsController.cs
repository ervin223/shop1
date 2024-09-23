﻿using ShopTARgv23.Models.Spaceships;
using Microsoft.AspNetCore.Mvc;
using ShopTARgv23.Data;

namespace ShopTARgv23.Controllers
{
    public class SpaceshipsController : Controller
    {

        private readonly ShopTARgv23Context _context;

        public SpaceshipsController
            (
                ShopTARgv23Context context
            )
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var result = _context.Spaceships
                .Select(x => new SpaceshipsIndexViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    BuildDate = x.BuildDate,
                    EnginePower = x.EnginePower,
                });

            return View();
        }
    }
}
