using MySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration) { }

        public IList<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdUsuario, Nombre, Apellido, Email, Clave, Avatar, Rol FROM Usuario";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Usuario
                {
                    IdUsuario = reader.GetInt32("IdUsuario"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    Clave = reader.GetString("Clave"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                    Rol = reader.GetInt32("Rol")
                });
            }
            return lista;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? usuario = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdUsuario, Nombre, Apellido, Email, Clave, Avatar, Rol FROM Usuario WHERE IdUsuario = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                usuario = new Usuario
                {
                    IdUsuario = reader.GetInt32("IdUsuario"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    Clave = reader.GetString("Clave"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                    Rol = reader.GetInt32("Rol")
                };
            }
            return usuario;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? usuario = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdUsuario, Nombre, Apellido, Email, Clave, Avatar, Rol FROM Usuario WHERE Email = @email";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                usuario = new Usuario
                {
                    IdUsuario = reader.GetInt32("IdUsuario"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    Clave = reader.GetString("Clave"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                    Rol = reader.GetInt32("Rol")
                };
            }
            return usuario;
        }

        public int Alta(Usuario usuario)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO Usuario (Nombre, Apellido, Email, Clave, Avatar, Rol) 
                        VALUES (@nombre, @apellido, @email, @clave, @avatar, @rol);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nombre", usuario.Nombre);
            command.Parameters.AddWithValue("@apellido", usuario.Apellido);
            command.Parameters.AddWithValue("@email", usuario.Email);
            command.Parameters.AddWithValue("@clave", usuario.Clave);
            command.Parameters.AddWithValue("@avatar", (object?)usuario.Avatar ?? DBNull.Value);
            command.Parameters.AddWithValue("@rol", usuario.Rol);
            connection.Open();
            usuario.IdUsuario = Convert.ToInt32(command.ExecuteScalar());
            return usuario.IdUsuario;
        }

        public int Modificacion(Usuario usuario)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE Usuario SET Nombre = @nombre, Apellido = @apellido, 
                        Email = @email, Rol = @rol WHERE IdUsuario = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nombre", usuario.Nombre);
            command.Parameters.AddWithValue("@apellido", usuario.Apellido);
            command.Parameters.AddWithValue("@email", usuario.Email);
            command.Parameters.AddWithValue("@rol", usuario.Rol);
            command.Parameters.AddWithValue("@id", usuario.IdUsuario);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int ModificarPerfil(Usuario usuario)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE Usuario SET Nombre = @nombre, Apellido = @apellido, 
                        Clave = @clave, Avatar = @avatar WHERE IdUsuario = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nombre", usuario.Nombre);
            command.Parameters.AddWithValue("@apellido", usuario.Apellido);
            command.Parameters.AddWithValue("@clave", usuario.Clave);
            command.Parameters.AddWithValue("@avatar", (object?)usuario.Avatar ?? DBNull.Value);
            command.Parameters.AddWithValue("@id", usuario.IdUsuario);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "DELETE FROM Usuario WHERE IdUsuario = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}