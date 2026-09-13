using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_TasaCambio
    {
        private readonly CD_TasaCambio _datosTasaCambio = new CD_TasaCambio();

        /// <summary>Tasa vigente para una moneda y tipo de operacion, o null si no hay ninguna configurada.</summary>
        public TasaCambio ObtenerVigente(int monedaId, string tipoOperacion) =>
            _datosTasaCambio.ObtenerVigente(monedaId, tipoOperacion);
    }
}
