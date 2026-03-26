using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCoreSolution.AccesoDatos.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _db;
        public ICategoriaRepository Categoria { get; private set; }
        public IArticuloRepository Articulo { get; private set; }
        public ISliderRepository Slider { get; private set; }

        public ContenedorTrabajo(ApplicationDbContext context)
        {
            // Se crea un solo DbContext, todos los repositorios usan ese mismo contexto. 
            // No hay múltiples conexiones.
            _db = context;
            Categoria = new CategoriaRepository(_db);
            Articulo = new ArticuloRepository(_db);
            Slider = new SliderRepository(_db);
        }
        

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
