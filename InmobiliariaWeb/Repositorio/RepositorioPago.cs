using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaWeb.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration) { }

        public IList<Pago> ObtenerTodos()
        {
            var lista = new List<Pago>();
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdPago, IdReserva, Concepto, FechaPago, Importe, Estado, IdUsuarioCreador, IdUsuarioAnulador FROM Pago";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Estado = reader.GetBoolean("Estado"),
                    IdUsuarioCreador = reader.GetInt32("IdUsuarioCreador"),
                    IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulador")) ? null : reader.GetInt32("IdUsuarioAnulador")
                });
            }
            return lista;
        }

        public IList<Pago> ObtenerPorReserva(int idReserva)
        {
            var lista = new List<Pago>();
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdPago, IdReserva, Concepto, FechaPago, Importe, Estado, IdUsuarioCreador, IdUsuarioAnulador FROM Pago WHERE IdReserva = @idReserva";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idReserva", idReserva);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Estado = reader.GetBoolean("Estado"),
                    IdUsuarioCreador = reader.GetInt32("IdUsuarioCreador"),
                    IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulador")) ? null : reader.GetInt32("IdUsuarioAnulador")
                });
            }
            return lista;
        }

        public Pago? ObtenerPorId(int id)
        {
            Pago? pago = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT IdPago, IdReserva, Concepto, FechaPago, Importe, Estado, IdUsuarioCreador, IdUsuarioAnulador FROM Pago WHERE IdPago = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                pago = new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Estado = reader.GetBoolean("Estado"),
                    IdUsuarioCreador = reader.GetInt32("IdUsuarioCreador"),
                    IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulador")) ? null : reader.GetInt32("IdUsuarioAnulador")
                };
            }
            return pago;
        }

        public int Alta(Pago pago)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO Pago (IdReserva, Concepto, FechaPago, Importe, Estado, IdUsuarioCreador)
                        VALUES (@idReserva, @concepto, @fechaPago, @importe, 1, @idUsuarioCreador);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idReserva", pago.IdReserva);
            command.Parameters.AddWithValue("@concepto", pago.Concepto);
            command.Parameters.AddWithValue("@fechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@importe", pago.Importe);
            command.Parameters.AddWithValue("@idUsuarioCreador", pago.IdUsuarioCreador);
            connection.Open();
            pago.IdPago = Convert.ToInt32(command.ExecuteScalar());
            return pago.IdPago;
        }

        public int Modificacion(Pago pago)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE Pago SET Concepto = @concepto WHERE IdPago = @idPago";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@concepto", pago.Concepto);
            command.Parameters.AddWithValue("@idPago", pago.IdPago);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Anular(int idPago, int idUsuarioAnulador)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE Pago SET Estado = 0, IdUsuarioAnulador = @idUsuario WHERE IdPago = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idUsuario", idUsuarioAnulador);
            command.Parameters.AddWithValue("@id", idPago);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}