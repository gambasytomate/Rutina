using System.Data;
using System.Globalization;

namespace Rutina
{
    public class Rutina
    {
        public static void Main(string[] args)
        {
            try
            {
                db.EjecutarNonQuery(SQL_CREAR_TABLA);
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo preparar la base de datos: " + ex.Message);
                return;
            }

            int opcion = 0;
            while (opcion != 5)
            {
                MostrarMenu();
                while (!int.TryParse(Console.ReadLine(), out opcion))
                {
                    Console.WriteLine("Eso no es un número válido, prueba otra vez:");
                }
                ElegirAccion(opcion);
            }
        }

        //MOSTRAR MENU PRINCIPAL
        public static void MostrarMenu()
        {
            Console.WriteLine("REGISTRO DE HÁBITOS");
            Console.WriteLine("1. Añadir hábito");
            Console.WriteLine("2. Ver hábitos");
            Console.WriteLine("3. Actualizar hábito");
            Console.WriteLine("4. Eliminar hábito");
            Console.WriteLine("5. Salir");
            Console.Write("Opción: ");
        }

        private static List<Habitos> listaHabitos = new List<Habitos>();
        //private static int siguienteId = 1;

        //SWITCH PARA DECIDIR
        private static void ElegirAccion(int opcion)
        {
            switch (opcion)
            {
                case 1:
                    Console.WriteLine("AÑADIR HÁBITO");
                    AnyadirHabito();
                    break;
                case 2:
                    Console.WriteLine("VER HÁBITO");
                    VerHabito();
                    break;
                case 3:
                    Console.WriteLine("ACTUALIZAR HÁBITO");
                    ActualizarHabito();
                    break;
                case 4:
                    Console.WriteLine("ELIMINAR HÁBITO");
                    EliminarHabito();
                    break;
                case 5:
                    Console.WriteLine("Adiós!!!");
                    break;
                default: Console.WriteLine("Número no válido"); break;
            }

        }
        //LISTA DE COSAS QUE HACER
        private static void EliminarHabito()
        {
            if (listaHabitos.Count == 0)
            {
                Console.WriteLine("La lista está vacía");
                return;
            }

            VerHabito();
            Console.WriteLine("Introduce el id del hábito que quieres eliminar: ");
            int idBuscado = int.Parse(Console.ReadLine());


            var parametros = new Dictionary<string, object?> { ["@id"] = idBuscado };
            int filas = db.EjecutarNonQuery(SQL_ELIMINAR, parametros);

            Console.WriteLine(filas > 0 ? "Hábito eliminado." : "No existe ningún hábito con ese Id.");
        }

        private static void ActualizarHabito()
        {
            if (ListarHabitos().Count == 0)
            {
                Console.WriteLine("La lista está vacía");
                return;
            }

            VerHabito();

            // Id
            int idBuscado;
            Console.WriteLine("Introduce el id del hábito que quieres actualizar: ");
            while (!int.TryParse(Console.ReadLine(), out idBuscado) || idBuscado <= 0)
            {
                Console.WriteLine("Introduce un número entero mayor que 0:");
            }

            // Comprobar que existe antes de pedir los datos nuevos
            Dictionary<string, object?> porId = new Dictionary<string, object?>();
            porId["@id"] = idBuscado;
            DataTable resultado = db.EjecutarConsulta(SQL_POR_ID, porId);
            if (resultado.Rows.Count == 0)
            {
                Console.WriteLine("No existe ningún hábito con ese Id.");
                return;
            }

            Console.WriteLine("Introduce los nuevos datos:");

            // Nombre
            string? nuevoNombre;
            do
            {
                Console.WriteLine("Nombre:");
                nuevoNombre = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(nuevoNombre));

            // Cantidad
            int nuevoCantidad;
            Console.WriteLine("Veces:");
            while (!int.TryParse(Console.ReadLine(), out nuevoCantidad) || nuevoCantidad <= 0)
            {
                Console.WriteLine("Introduce un número entero mayor que 0:");
            }

            // Fecha
            DateTime nuevoFecha;
            while (true)
            {
                Console.Write("Fecha (dd/MM/yyyy) o escribe 'hoy': ");
                string? entrada = Console.ReadLine()?.Trim().ToLower();

                if (entrada == "hoy")
                {
                    nuevoFecha = DateTime.Today;
                    break;
                }
                if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out nuevoFecha))
                {
                    break;
                }
                Console.WriteLine("Fecha no válida, prueba otra vez.");
            }

            Dictionary<string, object?> parametros = new Dictionary<string, object?>();
            parametros["@nombre"] = nuevoNombre.Trim();
            parametros["@cantidad"] = nuevoCantidad;
            parametros["@fecha"] = nuevoFecha.ToString("yyyy-MM-dd");
            parametros["@id"] = idBuscado;
            db.EjecutarNonQuery(SQL_ACTUALIZAR, parametros);
            Console.WriteLine("Hábito actualizado.");
        }

        private static void VerHabito()
        {
            List<Habitos> habitos = ListarHabitos();

            if (habitos.Count == 0)
            {
                Console.WriteLine("No hay hábitos registrados.");
                return;
            }

            foreach (Habitos h in habitos)
            {
                Console.WriteLine($"[{h.Id}] {h.Nombre} - {h.Cantidad} - {h.Fecha:dd/MM/yyyy}");
            }
        }

        private static void AnyadirHabito()
        {
            // Nombre
            string? nombreHabito;
            do
            {
                Console.WriteLine("Cuál ha sido el hábito?(Escribelo)");
                nombreHabito = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(nombreHabito));

            // Cantidad
            int cantidadHabito;
            Console.WriteLine("Cuántas veces lo has hecho?");
            while (!int.TryParse(Console.ReadLine(), out cantidadHabito) || cantidadHabito <= 0)
            {
                Console.WriteLine("Introduce un número entero mayor que 0:");
            }

            // Fecha
            DateTime fecha;
            while (true)
            {
                Console.Write("Fecha (dd/MM/yyyy) o escribe 'hoy': ");
                string? entrada = Console.ReadLine()?.Trim().ToLower();

                if (entrada == "hoy")
                {
                    fecha = DateTime.Today;
                    break;
                }

                if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out fecha))
                {
                    break;
                }
                Console.WriteLine("Fecha no válida, prueba otra vez.");
            }

            Dictionary<string, object?> parametros = new Dictionary<string, object?>();
            parametros["@nombre"] = nombreHabito.Trim();
            parametros["@cantidad"] = cantidadHabito;
            parametros["@fecha"] = fecha.ToString("yyyy-MM-dd");
            db.EjecutarNonQuery(SQL_INSERTAR, parametros);
            Console.WriteLine("Hábito añadido.");
        }


        //BBDD
        //CREAR TABLA
        private const string SQL_CREAR_TABLA = @"
        CREATE TABLE IF NOT EXISTS Habitos (
        Id       INTEGER PRIMARY KEY AUTOINCREMENT,
        Nombre   TEXT    NOT NULL,
        Cantidad INTEGER NOT NULL,
        Fecha    TEXT    NOT NULL
        );";
        //INSERTAR HABITO
        private const string SQL_INSERTAR = @"
        INSERT INTO Habitos (Nombre, Cantidad, Fecha)
        VALUES (@nombre, @cantidad, @fecha);";
        //LISTAR
        private const string SQL_LISTAR = "SELECT * FROM Habitos ORDER BY Fecha, Id;";
        //SELECCIONAR
        private const string SQL_POR_ID = "SELECT * FROM Habitos WHERE Id = @id;";
        //ACTUALIZAR
        private const string SQL_ACTUALIZAR = @"
        UPDATE Habitos
        SET Nombre = @nombre, Cantidad = @cantidad, Fecha = @fecha
        WHERE Id = @id;";
        //ELIMINAR
        private const string SQL_ELIMINAR = "DELETE FROM Habitos WHERE Id = @id;";

        private static readonly DataBaseManager db =
            new DataBaseManager(Path.Combine(AppContext.BaseDirectory, "habitos.db"));


        // Lee la tabla de la base de datos y la convierte en una lista de Habitos
        private static List<Habitos> ListarHabitos()
        {
            List<Habitos> lista = new List<Habitos>();
            DataTable tabla = db.EjecutarConsulta(SQL_LISTAR);
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new Habitos(
                    Convert.ToInt32(fila["Id"]),
                    fila["Nombre"].ToString()!,
                    Convert.ToInt32(fila["Cantidad"]),
                    DateTime.ParseExact(fila["Fecha"].ToString()!, "yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }
            return lista;
        }

    }
}