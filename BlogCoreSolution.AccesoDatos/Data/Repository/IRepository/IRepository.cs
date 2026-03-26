using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BlogCoreSolution.AccesoDatos.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);

        IEnumerable<T> GetAll(
            Expression<Func<T, bool>>? filter = null, // Este método permite FILTRAR utilizando .Where()
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, // Permite ordenar ascendetemente o descendentemente
            string? includePropierties = null // Contiene las relaciones (con otras entidades) para poder usar el .Include()
        );

        T GetFirstOrDefault( // Este método obtiene un solo registro. Muy usado al Editar una única entidad, para hacer validaciones, etc.
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null
        );

        void Add(T entity); // Agrega una entidad al contexto. No guarda en la BBDD.

        void Remove(int id);

        void Remove(T entity);
    }
}
