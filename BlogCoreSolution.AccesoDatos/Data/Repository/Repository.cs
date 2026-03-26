using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BlogCoreSolution.AccesoDatos.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {   // Este es el repositorio genérico por entidad.
        protected readonly DbContext Context;
        internal DbSet<T> dbSet;

        public Repository(DbContext context) // Inyección de dependencias.
        {
            Context = context;
            this.dbSet = Context.Set<T>();
        }

        public void Add(T entity)
        {
           this.dbSet.Add(entity);
        }

        public T Get(int id)
        {
            return this.dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string? includePropierties = null)
        {
            // Se crea una consulta IQueryable a partir del dbSet del contexto
            IQueryable<T> query = this.dbSet;

            // Se aplica el filtro si se proporcionó
            if (filter != null)
            { 
                query = query.Where(filter);
            }

            // Se incluyen propiedades de navegación si fueron proporcionadas.
            if (includePropierties != null)
            {
                // Se divide la cadena (string) de propiedades por coma y se itera sobre ellas.
                foreach (var includeProperty in includePropierties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {   // Se aplica cada propiedad una por una.   
                    query = query.Include(includeProperty);
                }
            }

            // Se aplica el ordenamiento si fue proporcionado.
            if (orderBy != null)
            {
                // Se ejecuta la función de ordenamiento y se convierte la consulta en una lista.
                return orderBy(query).ToList();
            }

            // Si no se proporciona ordenamiento, simplemente se convierte la consulta en una lista.
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            // Se crea una consulta IQueryable a partir del DbSet del contexto.
            IQueryable<T> query = this.dbSet;

            // Se aplica el filtro si se proporciona
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Se incluyen propiedades de navegación si se proporcionaron
            if (includeProperties != null)
            {
                // Se divide la cadena (string) de propiedades por comas y se itera sobre ellas.
                foreach (var includeProperty in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Si no se proporciona ordenamiento, simplemente se convierte a una lista.
            return query.FirstOrDefault();
        }

        public void Remove(int id)
        {
            T entityToRemove = this.dbSet.Find(id);
        }

        public void Remove(T entity)
        {
            this.dbSet.Remove(entity);
        }
    }
}
