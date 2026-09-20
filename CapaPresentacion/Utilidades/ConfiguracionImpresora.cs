using System;
using System.Drawing.Printing;
using System.Linq;
using CapaPresentacion.Properties;

namespace CapaPresentacion.Utilidades
{
    /// <summary>
    /// Acceso centralizado a la configuracion de la impresora de tiquetes, elegida por el usuario
    /// en Configuracion &gt; Impresora y persistida en la configuracion de usuario de la aplicacion.
    /// </summary>
    public static class ConfiguracionImpresora
    {
        /// <summary>Nombre de la impresora de Windows a usar. Vacio = impresora predeterminada del sistema.</summary>
        public static string NombreImpresora
        {
            get => Settings.Default.ImpresoraTiquete ?? string.Empty;
            set { Settings.Default.ImpresoraTiquete = value ?? string.Empty; Settings.Default.Save(); }
        }

        /// <summary>Ancho de papel (mm) a forzar en el tiquete. 0 = usar el tamano configurado en la impresora.</summary>
        public static int AnchoPapelMM
        {
            get => Settings.Default.AnchoPapelTiqueteMM;
            set { Settings.Default.AnchoPapelTiqueteMM = value; Settings.Default.Save(); }
        }

        /// <summary>Si es true, se muestra el cuadro de dialogo de impresion antes de imprimir un tiquet.</summary>
        public static bool MostrarDialogoImpresion
        {
            get => Settings.Default.MostrarDialogoImpresionTiquete;
            set { Settings.Default.MostrarDialogoImpresionTiquete = value; Settings.Default.Save(); }
        }

        /// <summary>Nombres de las impresoras instaladas en Windows.</summary>
        public static string[] ImpresorasInstaladas()
        {
            return PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
        }

        /// <summary>
        /// Aplica la impresora y (si esta definido) el ancho de papel configurados a un PrintDocument
        /// recien creado, antes de imprimir o de mostrar el PrintDialog.
        /// </summary>
        public static void Aplicar(PrintDocument documento)
        {
            if (documento == null) throw new ArgumentNullException(nameof(documento));

            documento.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            string impresora = NombreImpresora;
            if (!string.IsNullOrWhiteSpace(impresora) && ImpresorasInstaladas().Contains(impresora))
            {
                documento.PrinterSettings.PrinterName = impresora;
            }

            int anchoMM = AnchoPapelMM;
            if (anchoMM > 0)
            {
                const int altoContinuoMM = 3276; // alto de un rollo continuo tipico de impresora de tiquetes
                int anchoCentesimas = (int)Math.Round(anchoMM * 100 / 25.4);
                int altoCentesimas = (int)Math.Round(altoContinuoMM * 100 / 25.4);
                documento.DefaultPageSettings.PaperSize = new PaperSize("Tiquete", anchoCentesimas, altoCentesimas);
            }
        }
    }
}
