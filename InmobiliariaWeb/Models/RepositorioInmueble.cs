using MySqlConnector;

namespace InmobiliariaWeb.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration) { }

        private const string SelectBase = @"
            SELECT i.IdInmueble, i.Direccion, i.Cupo, i.IdTipoInmueble, i.Latitud, i.Longitud,
                   i.PrecioPorDia, i.PorcentajeReserva, i.IdPropietario, i.Disponible,
                   t.Descripcion AS TipoDescripcion,
                   p.Nombre AS PropNombre, p.Apellido AS PropApellido
            FROM Inmueble i
            INNER JOIN TipoInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
            INNER JOIN Propietario p ON i.IdPropietario = p.IdPropietario";

        private static Inmueble Mapear(MySqlDataReader reader)
        {
            return new Inmueble
            {
                IdInmueble = reader.GetInt32("IdInmueble"),
                Direccion = reader.GetString("Direccion"),
                Cupo = reader.GetInt32("Cupo"),
                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                TipoInmuebleDescripcion = reader.GetString("TipoDescripcion"),
                Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? null : reader.GetDecimal("Latitud"),
                Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? null : reader.GetDecimal("Longitud"),
                PrecioPorDia = reader.GetDecimal("PrecioPorDia"),
                PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
                IdPropietario = reader.GetInt32("IdPropietario"),
                PropietarioNombreCompleto = $"{reader.GetString("PropNombre")} {reader.GetString("PropApellido")}",
                Disponible = reader.GetBoolean("Disponible"),
            };
        }

        public IList<Inmueble> ObtenerLista(int pagina, int tamano)
        {
            var lista = new List<Inmueble>();
            using var connection = new MySqlConnection(connectionString);
            var sql = SelectBase + " ORDER BY i.Direccion LIMIT @tamano OFFSET @offset";
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
            var sql = "SELECT COUNT(*) FROM Inmueble";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? i = null;
            using var connection = new MySqlConnection(connectionString);
            var sql = SelectBase + " WHERE i.IdInmueble = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read()) i = Mapear(reader);
            return i;
        }

        public IList<Inmueble> BuscarPorDireccion(string q)
        {
            var lista = new List<Inmueble>();
            using var connection = new MySqlConnection(connectionString);
            var sql = SelectBase + " WHERE i.Direccion LIKE @q ORDER BY i.Direccion LIMIT 10";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@q", $"%{q}%");
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
            return lista;
        }

        public IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime desde, DateTime hasta)
        {
            var lista = new List<Inmueble>();
            using var connection = new MySqlConnection(connectionString);
            var sql = SelectBase + @"
                WHERE i.Disponible = TRUE
                AND i.IdInmueble NOT IN (
                    SELECT r.IdInmueble FROM Reserva r
                    WHERE r.FechaDesde <= @hasta
                    AND COALESCE(r.FechaTerminacionAnticipada, r.FechaHasta) >= @desde
                )
                ORDER BY i.Direccion";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@desde", desde.Date);
            command.Parameters.AddWithValue("@hasta", hasta.Date);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
            return lista;
        }

        public int Alta(Inmueble i)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO Inmueble
                        (Direccion, Cupo, IdTipoInmueble, Latitud, Longitud, PrecioPorDia, PorcentajeReserva, IdPropietario, Disponible)
                        VALUES (@direccion, @cupo, @idTipo, @lat, @lon, @precio, @porcentaje, @idProp, @disponible);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@direccion", i.Direccion);
            command.Parameters.AddWithValue("@cupo", i.Cupo);
            command.Parameters.AddWithValue("@idTipo", i.IdTipoInmueble);
            command.Parameters.AddWithValue("@lat", (object?)i.Latitud ?? DBNull.Value);
            command.Parameters.AddWithValue("@lon", (object?)i.Longitud ?? DBNull.Value);
            command.Parameters.AddWithValue("@precio", i.PrecioPorDia);
            command.Parameters.AddWithValue("@porcentaje", i.PorcentajeReserva);
            command.Parameters.AddWithValue("@idProp", i.IdPropietario);
            command.Parameters.AddWithValue("@disponible", i.Disponible);
            connection.Open();
            i.IdInmueble = Convert.ToInt32(command.ExecuteScalar());
            return i.IdInmueble;
        }

        public int Modificacion(Inmueble i)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE Inmueble SET
                        Direccion = @direccion, Cupo = @cupo, IdTipoInmueble = @idTipo,
                        Latitud = @lat, Longitud = @lon, PrecioPorDia = @precio,
                        PorcentajeReserva = @porcentaje, IdPropietario = @idProp, Disponible = @disponible
                        WHERE IdInmueble = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@direccion", i.Direccion);
            command.Parameters.AddWithValue("@cupo", i.Cupo);
            command.Parameters.AddWithValue("@idTipo", i.IdTipoInmueble);
            command.Parameters.AddWithValue("@lat", (object?)i.Latitud ?? DBNull.Value);
            command.Parameters.AddWithValue("@lon", (object?)i.Longitud ?? DBNull.Value);
            command.Parameters.AddWithValue("@precio", i.PrecioPorDia);
            command.Parameters.AddWithValue("@porcentaje", i.PorcentajeReserva);
            command.Parameters.AddWithValue("@idProp", i.IdPropietario);
            command.Parameters.AddWithValue("@disponible", i.Disponible);
            command.Parameters.AddWithValue("@id", i.IdInmueble);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int CambiarDisponibilidad(int id, bool disponible)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "UPDATE Inmueble SET Disponible = @disponible WHERE IdInmueble = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@disponible", disponible);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}