using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CapaPresentacion.Utilidades
{
    /// <summary>
    /// Exporta datos tabulares (un DataGridView, o encabezados + filas construidos a mano) a un archivo
    /// CSV que Excel abre directamente como una hoja de calculo (la linea "sep=," fuerza la coma como
    /// delimitador sin importar la configuracion regional de Windows).
    /// </summary>
    public static class ExportadorCsv
    {
        public static void Exportar(IWin32Window propietario, DataGridView grilla, string nombreSugerido)
        {
            if (grilla == null || grilla.Rows.Count == 0 || grilla.Columns.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar. Realice una busqueda primero.", "Exportar a Excel",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var filas = new List<string[]>();
            foreach (DataGridViewRow fila in grilla.Rows)
            {
                if (fila.IsNewRow) continue;
                filas.Add(ObtenerValores(fila));
            }

            Exportar(propietario, ObtenerEncabezados(grilla), filas, nombreSugerido);
        }

        /// <summary>
        /// Exporta encabezados + filas armados por el llamador (para pantallas sin DataGridView, como un
        /// resumen de cierre de caja). <paramref name="notasFinales"/> se agrega como texto libre al final,
        /// util para observaciones que no encajan en una columna.
        /// </summary>
        public static void Exportar(IWin32Window propietario, string[] encabezados, IEnumerable<string[]> filas,
            string nombreSugerido, IEnumerable<string> notasFinales = null)
        {
            List<string[]> listaFilas = filas?.ToList() ?? new List<string[]>();

            if (encabezados == null || encabezados.Length == 0 || listaFilas.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar. Realice una busqueda primero.", "Exportar a Excel",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialogo = new SaveFileDialog
            {
                Filter = "Excel (CSV) (*.csv)|*.csv",
                FileName = $"{nombreSugerido}_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            })
            {
                if (dialogo.ShowDialog(propietario) != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("sep=,");
                    sb.AppendLine(FilaCsv(encabezados));

                    foreach (string[] fila in listaFilas)
                        sb.AppendLine(FilaCsv(fila));

                    if (notasFinales != null)
                    {
                        sb.AppendLine();
                        foreach (string nota in notasFinales)
                            sb.AppendLine(FilaCsv(new[] { nota }));
                    }

                    File.WriteAllText(dialogo.FileName, sb.ToString(), new UTF8Encoding(true));

                    MessageBox.Show("Exportacion completada.", "Exportar a Excel",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo exportar el archivo: " + ex.Message, "Exportar a Excel",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string[] ObtenerEncabezados(DataGridView grilla)
        {
            var encabezados = new string[grilla.Columns.Count];
            for (int i = 0; i < grilla.Columns.Count; i++)
                encabezados[i] = grilla.Columns[i].HeaderText;
            return encabezados;
        }

        private static string[] ObtenerValores(DataGridViewRow fila)
        {
            var valores = new string[fila.Cells.Count];
            for (int i = 0; i < fila.Cells.Count; i++)
                valores[i] = Convert.ToString(fila.Cells[i].Value);
            return valores;
        }

        private static string FilaCsv(string[] campos)
        {
            for (int i = 0; i < campos.Length; i++)
                campos[i] = "\"" + (campos[i] ?? string.Empty).Replace("\"", "\"\"") + "\"";
            return string.Join(",", campos);
        }
    }
}
