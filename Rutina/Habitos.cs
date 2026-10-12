using System;
using System.Collections.Generic;
using System.Text;

namespace Rutina
{
    internal class Habitos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public Habitos(int id, string nombre, int cantidad, DateTime fecha)
        {
                Id = id;
                Nombre = nombre;
                Cantidad = cantidad;
                Fecha = fecha;
        }
    }
}
