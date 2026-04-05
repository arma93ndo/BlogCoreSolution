using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCoreSolution.AccesoDatos.Data.Repository.IRepository
{
    public  interface IContenedorTrabajo : IDisposable
    {
        // Aquí se agregan los distintos repositorios que controlará esta Unit of Work.
        ICategoriaRepository Categoria { get; }
        IArticuloRepository Articulo { get; }
        ISliderRepository Slider { get; }
        IUsuarioRepository Usuario { get; }

        void Save(); // Se usará para persistir los datos hechos al contexto.
    }
}
