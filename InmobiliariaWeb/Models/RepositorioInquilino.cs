using MySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioInquilino : RepositorioBase
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration) { }

        public IList<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdInquilino, Dni, Nombre, Apellido, Telefono, Email FROM Inquilinos";
            using var command = new MySqlCommand(sql, connection);
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

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdInquilino, Dni, Nombre, Apellido, Telefono, Email FROM Inquilinos WHERE IdInquilino = @id";
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

        public int Alta(Inquilino i)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO Inquilinos (Dni, Nombre, Apellido, Telefono, Email)
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
            var sql = @"UPDATE Inquilinos SET Dni = @dni, Nombre = @nombre, Apellido = @apellido,
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
            var sql = "DELETE FROM Inquilinos WHERE IdInquilino = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}
   