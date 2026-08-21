using MySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioPropietario : RepositorioBase
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }

        public IList<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email FROM Propietarios";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Propietario
                {
                    IdPropietario = reader.GetInt32("IdPropietario"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Dni = reader.GetString("Dni"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                    Email = reader.GetString("Email"),
                });
            }
            return lista;
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? p = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email FROM Propietarios WHERE IdPropietario = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                p = new Propietario
                {
                    IdPropietario = reader.GetInt32("IdPropietario"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Dni = reader.GetString("Dni"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                    Email = reader.GetString("Email"),
                };
            }
            return p;
        }

        public int Alta(Propietario p)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email, Clave)
                        VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nombre", p.Nombre);
            command.Parameters.AddWithValue("@apellido", p.Apellido);
            command.Parameters.AddWithValue("@dni", p.Dni);
            command.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", p.Email);
            command.Parameters.AddWithValue("@clave", (object?)p.Clave ?? DBNull.Value);
            connection.Open();
            p.IdPropietario = Convert.ToInt32(command.ExecuteScalar());
            return p.IdPropietario;
        }

        public int Modificacion(Propietario p)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE Propietarios SET Nombre = @nombre, Apellido = @apellido, Dni = @dni,
                        Telefono = @telefono, Email = @email WHERE IdPropietario = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nombre", p.Nombre);
            command.Parameters.AddWithValue("@apellido", p.Apellido);
            command.Parameters.AddWithValue("@dni", p.Dni);
            command.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", p.Email);
            command.Parameters.AddWithValue("@id", p.IdPropietario);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "DELETE FROM Propietarios WHERE IdPropietario = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}