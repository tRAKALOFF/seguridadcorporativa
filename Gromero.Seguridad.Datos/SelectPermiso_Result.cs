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
    
    public partial class SelectPermiso_Result
    {
        public SelectPermiso_Result()
        {
            this.NumeroOrden = 0;
        }
    
        public string IdPermiso { get; set; }
        public string IdOpcionUI { get; set; }
        public string IdRol { get; set; }
        public string IdRolPerfil { get; set; }
        public string IdAplicacion { get; set; }
        public string IdUsuario { get; set; }
        public bool Conceder { get; set; }
        public string OpcionPadre { get; set; }
        public string NombreRol { get; set; }
        public string NombreOpcion { get; set; }
        public string Clase { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Url { get; set; }
        public int Nivel { get; set; }
        public string IdOpcionPadre { get; set; }
        public Nullable<int> NumeroOrden { get; set; }
    }
}
