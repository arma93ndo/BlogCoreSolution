using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCoreSolution.AccesoDatos.Data.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoriaRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public IEnumerable<SelectListItem> GetListaCategoria()
        {
            return _db.Categorias.Select(c => new SelectListItem {
                Text = c.Nombre,
                Value = c.Id.ToString()
            });
        }

        public void Update(Categoria categoria)
        {
            var objDesdeDb = _db.Categorias.FirstOrDefault(s => s.Id == categoria.Id);
            objDesdeDb.Nombre = categoria.Nombre;
            objDesdeDb.Orden = categoria.Orden;

            // _db.SaveChanges();  // De esto se encargará la Unit of Work.
        }
    }
}
