//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gromero.Seguridad.Datos
{
    using System;
    using System.Collections.Generic;
    
    public partial class RecursoDetalle
    {
        public RecursoDetalle()
        {
            this.RecursoPerfiles = new HashSet<RecursoPerfil>();
        }
    
        public string IdRecursoDetalle { get; set; }
        public string IdRecurso { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string IdRecursoDetallePadre { get; set; }
        public int Nivel { get; set; }
    
        public virtual Recurso Recurso { get; set; }
        public virtual ICollection<RecursoPerfil> RecursoPerfiles { get; set; }
    }
}