using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCoreSolution.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider> Sliders { get; set; }
        public IEnumerable<Articulo> ListaArticulos { get; set; }
    }
}
