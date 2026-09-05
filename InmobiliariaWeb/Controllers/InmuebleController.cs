using MySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioReserva : RepositorioBase, IRepositorioReserva
    {
        public RepositorioReserva(IConfiguration configuration) : base(configuration) { }

        private const string SelectBase = @"
            SELECT r.IdReserva, r.IdInquilino, r.IdInmueble, r.FechaDesde, r.FechaHasta,
                   r.FechaHastaOriginal, r.FechaTerminacionAnticipada, r.Multa,
                   inq.Nombre AS InqNombre, inq.Apellido AS InqApellido,
                   inm.Direccion AS InmDireccion
            FROM Reserva r
            INNER JOIN Inquilino inq ON r.IdInquilino = inq.IdInquilino
            INNER JOIN Inmueble inm ON r.IdInmueble = inm.IdInmueble";

        private static Reserva Mapear(MySqlDataReader reader)
        {
            return new Reserva
            {
                IdReserva = reader.GetInt32("IdReserva"),
                IdInquilino = reader.GetInt32("IdInquilino"),
                InquilinoNombreCompleto = $"{reader.GetString("InqNombre")} {reader.GetString("InqApellido")}",
                IdInmueble = reader.GetInt32("IdInmueble"),
                InmuebleDireccion = reader.GetString("InmDireccion"),
                FechaDesde = reader.GetDateTime("FechaDesde"),
                FechaHasta = reader.GetDateTime("FechaHasta"),
                FechaHastaOriginal = reader.GetDateTime("FechaHastaOriginal"),
                FechaTerminacionAnticipada = reader.IsDBNull(reader.GetOrdinal("FechaTerminacionAnticipada"))
                    ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) ? null : reader.GetDecimal("Multa"),
            };
        }

        public IList<Reserva> ObtenerLista(int pagina, int tamano)
        {
            var lista = new List<Reserva>();
            using var connection = new MySqlConnection(connectionString);
            var sql = SelectBase + " ORDER BY r.FechaDesde DESC LIMIT @tamano OFFSET @offset";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@tamano", tamano);
            command.Parameters.AddWithValue("@offset", (pagina - 1) * tamano);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
            return lista;
        }

        public int ObtenerCantidad()
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT COUNT(*) FROM Reserva";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? r = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = SelectBase + " WHERE r.IdReserva = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read()) r = Mapear(reader);
            return r;
        }

        public bool ExisteSolapamiento(int idInmueble, DateTime desde, DateTime hasta, int idReservaExcluir)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT COUNT(*) FROM Reserva
                        WHERE IdInmueble = @idInmueble
                        AND IdReserva <> @idExcluir
                        AND FechaDesde <= @hasta
                        AND COALESCE(FechaTerminacionAnticipada, FechaHasta) >= @desde";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idInmueble", idInmueble);
            command.Parameters.AddWithValue("@idExcluir", idReservaExcluir);
            command.Parameters.AddWithValue("@desde", desde.Date);
            command.Parameters.AddWithValue("@hasta", hasta.Date);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public int Alta(Reserva r)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO Reserva
                        (IdInquilino, IdInmueble, FechaDesde, FechaHasta, FechaHastaOriginal)
                        VALUES (@idInquilino, @idInmueble, @desde, @hasta, @hastaOriginal);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
            command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
            command.Parameters.AddWithValue("@desde", r.FechaDesde.Date);
            command.Parameters.AddWithValue("@hasta", r.FechaHasta.Date);
            command.Parameters.AddWithValue("@hastaOriginal", r.FechaHasta.Date);
            connection.Open();
            r.IdReserva = Convert.ToInt32(command.ExecuteScalar());
            return r.IdReserva;
        }

        public int Modificacion(Reserva r)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE Reserva SET
                        IdInquilino = @idInquilino, IdInmueble = @idInmueble,
                        FechaDesde = @desde, FechaHasta = @hasta,
                        FechaTerminacionAnticipada = @terminacion, Multa = @multa
                        WHERE IdReserva = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idInquilino", r.IdInquilino);
            command.Parameters.AddWithValue("@idInmueble", r.IdInmueble);
            command.Parameters.AddWithValue("@desde", r.FechaDesde.Date);
            command.Parameters.AddWithValue("@hasta", r.FechaHasta.Date);
            command.Parameters.AddWithValue("@terminacion", (object?)r.FechaTerminacionAnticipada?.Date ?? DBNull.Value);
            command.Parameters.AddWithValue("@multa", (object?)r.Multa ?? DBNull.Value);
            command.Parameters.AddWithValue("@id", r.IdReserva);
            connection.Open();
            return command.ExecuteNonQuery();
        }

    }
}