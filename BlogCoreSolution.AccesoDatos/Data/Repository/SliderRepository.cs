using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCoreSolution.AccesoDatos.Data.Repository
{
    public class SliderRepository : Repository<Slider>, ISliderRepository
    {
        private readonly ApplicationDbContext _db;

        public SliderRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(Slider slider)
        {
            var objDesdeDb = _db.Sliders.FirstOrDefault(s => s.Id == slider.Id);
            objDesdeDb.Nombre = slider.Nombre;
            objDesdeDb.Estado = slider.Estado;
            objDesdeDb.UrlImagen = slider.UrlImagen;

            // _db.SaveChanges();  // De esto se encargará la Unit of Work.
        }
    }
}
