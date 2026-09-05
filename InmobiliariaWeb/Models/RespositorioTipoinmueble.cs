using MySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
    {
        public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration) { }

        public IList<TipoInmueble> ObtenerLista(int pagina, int tamano)
        {
            var lista = new List<TipoInmueble>();
            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT IdTipoInmueble, Descripcion FROM TipoInmueble
                        ORDER BY Descripcion
                        LIMIT @tamano OFFSET @offset";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@tamano", tamano);
            command.Parameters.AddWithValue("@offset", (pagina - 1) * tamano);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new TipoInmueble
                {
                    IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                    Descripcion = reader.GetString("Descripcion"),
                });
            }
            return lista;
        }

        public int ObtenerCantidad()
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT COUNT(*) FROM TipoInmueble";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? t = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdTipoInmueble, Descripcion FROM TipoInmueble WHERE IdTipoInmueble = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                t = new TipoInmueble
                {
                    IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                    Descripcion = reader.GetString("Descripcion"),
                };
            }
            return t;
        }

        public IList<TipoInmueble> BuscarPorDescripcion(string q)
        {
            var lista = new List<TipoInmueble>();
            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT IdTipoInmueble, Descripcion FROM TipoInmueble
                        WHERE Descripcion LIKE @q
                        ORDER BY Descripcion
                        LIMIT 10";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@q", $"%{q}%");
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new TipoInmueble
                {
                    IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                    Descripcion = reader.GetString("Descripcion"),
                });
            }
            return lista;
        }

        public int Alta(TipoInmueble t)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO TipoInmueble (Descripcion) VALUES (@descripcion);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@descripcion", t.Descripcion);
            connection.Open();
            t.IdTipoInmueble = Convert.ToInt32(command.ExecuteScalar());
            return t.IdTipoInmueble;
        }

        public int Modificacion(TipoInmueble t)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "UPDATE TipoInmueble SET Descripcion = @descripcion WHERE IdTipoInmueble = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@descripcion", t.Descripcion);
            command.Parameters.AddWithValue("@id", t.IdTipoInmueble);
            connection.Open();
            return command.ExecuteNonQuery();
        }
        
    }
}