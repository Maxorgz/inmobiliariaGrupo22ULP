using MySqlConnector;
using Microsoft.Extensions.Configuration;
using InmobiliariaWeb.Models;

namespace InmobiliariaWeb.Repositorio
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration) { }

        public IList<Inquilino> ObtenerTodos(int pagina, int tamanio)
        {
            if (pagina < 1) pagina = 1;
            if (tamanio < 1) tamanio = 10;

            long offset = ((long)pagina - 1) * tamanio;
            var lista = new List<Inquilino>();
            using var connection = new MySqlConnection(connectionString);
            
            var sql = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, IsActive
                        FROM Inquilino
                        WHERE IsActive = 1
                        ORDER BY IdInquilino
                        LIMIT @tamanio OFFSET @offset";
            
            using var command = new MySqlCommand(sql, connection);
                
            command.Parameters.AddWithValue("@tamanio", tamanio);
            command.Parameters.AddWithValue("@offset", offset);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Inquilino
                {
                    IdInquilino = reader.GetInt32("IdInquilino"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Dni = reader.GetString("Dni"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                    Email = reader.GetString("Email"),
                    IsActive = reader.GetBoolean("IsActive"),
                });
            }
            return lista;
        }

        public int ObtenerTotal()
        {
           using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT COUNT(*) FROM Inquilino WHERE IsActive = 1";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdInquilino, Dni, Nombre, Apellido, Telefono, Email FROM Inquilino WHERE IdInquilino = @id AND IsActive = 1";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                inquilino = new Inquilino
                {
                    IdInquilino = reader.GetInt32("IdInquilino"),
                    Dni = reader.GetString("Dni"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                    Email = reader.GetString("Email"),
                };
            }
            return inquilino;
        }

        public IList<Inquilino> BuscarPorNombre(string q)
        {
            var lista = new List<Inquilino>();
            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT IdInquilino, Dni, Nombre, Apellido, Telefono, Email FROM Inquilino
                        WHERE Nombre LIKE @q OR Apellido LIKE @q
                        ORDER BY Apellido, Nombre
                        LIMIT 10";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@q", $"%{q}%");
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Inquilino
                {
                    IdInquilino = reader.GetInt32("IdInquilino"),
                    Dni = reader.GetString("Dni"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                    Email = reader.GetString("Email"),
                });
            }
            return lista;
        }

        public int Alta(Inquilino i)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO Inquilino (Dni, Nombre, Apellido, Telefono, Email)
                        VALUES (@dni, @nombre, @apellido, @telefono, @email);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@dni", i.Dni);
            command.Parameters.AddWithValue("@nombre", i.Nombre);
            command.Parameters.AddWithValue("@apellido", i.Apellido);
            command.Parameters.AddWithValue("@telefono", (object?)i.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", i.Email);
            connection.Open();
            i.IdInquilino = Convert.ToInt32(command.ExecuteScalar());
            return i.IdInquilino;
        }

        public int Modificacion(Inquilino i)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE Inquilino SET Dni = @dni, Nombre = @nombre, Apellido = @apellido,
                        Telefono = @telefono, Email = @email WHERE IdInquilino = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@dni", i.Dni);
            command.Parameters.AddWithValue("@nombre", i.Nombre);
            command.Parameters.AddWithValue("@apellido", i.Apellido);
            command.Parameters.AddWithValue("@telefono", (object?)i.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", i.Email);
            command.Parameters.AddWithValue("@id", i.IdInquilino);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "UPDATE Inquilino SET IsActive = 0 WHERE IdPropietario = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}