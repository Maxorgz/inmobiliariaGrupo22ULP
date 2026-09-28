using MySqlConnector;
using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Repositorio
{
    public class RepositorioImagen : RepositorioBase, IRepositorioImagen
    {
        public RepositorioImagen(IConfiguration configuration) : base(configuration) { }

        public int Alta(Imagen imagen)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO Imagen (IdInmueble, Url) VALUES (@idInmueble, @url);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
            command.Parameters.AddWithValue("@url", imagen.Url);
            connection.Open();
            imagen.IdImagen = Convert.ToInt32(command.ExecuteScalar());
            return imagen.IdImagen;
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "DELETE FROM Imagen WHERE IdImagen = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public Imagen? ObtenerPorId(int id)
        {
            Imagen? img = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdImagen, IdInmueble, Url FROM Imagen WHERE IdImagen = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                img = new Imagen
                {
                    IdImagen = reader.GetInt32("IdImagen"),
                    IdInmueble = reader.GetInt32("IdInmueble"),
                    Url = reader.GetString("Url")
                };
            }
            return img;
        }

        public IList<Imagen> BuscarPorInmueble(int idInmueble)
        {
            var lista = new List<Imagen>();
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdImagen, IdInmueble, Url FROM Imagen WHERE IdInmueble = @idInmueble";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idInmueble", idInmueble);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Imagen
                {
                    IdImagen = reader.GetInt32("IdImagen"),
                    IdInmueble = reader.GetInt32("IdInmueble"),
                    Url = reader.GetString("Url")
                });
            }
            return lista;
        }
    }
}