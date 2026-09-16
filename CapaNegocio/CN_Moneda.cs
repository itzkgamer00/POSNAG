using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Moneda
    {
        private readonly CD_Moneda _datosMoneda = new CD_Moneda();

        /// <summary>Todas las monedas (activas e inactivas), para la pantalla de administracion.</summary>
        public List<Moneda> Listar() => _datosMoneda.Listar();

        /// <summary>Monedas activas, para combos de seleccion (apertura de caja, mesa de cambio, etc.).</summary>
        public List<Moneda> ListarActivas() => _datosMoneda.ListarActivas();

        /// <summary>Registra una nueva moneda.</summary>
        /// <exception cref="ArgumentException">Si falta el nombre o el codigo.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe una moneda con ese codigo.</exception>
        public Moneda RegistrarMoneda(string nombre, string codigo, string simbolo)
        {
            nombre = ValidarNombre(nombre);
            codigo = ValidarCodigo(codigo);
            simbolo = NormalizarSimbolo(simbolo);

            if (_datosMoneda.ExisteCodigo(codigo))
                throw new InvalidOperationException("Ya existe una moneda con ese codigo.");

            var moneda = new Moneda { Nombre = nombre, Codigo = codigo, Simbolo = simbolo, Estado = true };
            moneda.MonedaId = _datosMoneda.Registrar(nombre, codigo, simbolo);
            return moneda;
        }

        /// <summary>Actualiza nombre, codigo y simbolo de una moneda existente.</summary>
        /// <exception cref="ArgumentException">Si falta el nombre o el codigo.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe otra moneda con ese codigo.</exception>
        public void ActualizarMoneda(int monedaId, string nombre, string codigo, string simbolo)
        {
            nombre = ValidarNombre(nombre);
            codigo = ValidarCodigo(codigo);
            simbolo = NormalizarSimbolo(simbolo);

            if (_datosMoneda.ExisteCodigo(codigo, monedaId))
                throw new InvalidOperationException("Ya existe otra moneda con ese codigo.");

            _datosMoneda.Actualizar(monedaId, nombre, codigo, simbolo);
        }

        /// <summary>Activa o desactiva una moneda.</summary>
        public void CambiarEstadoMoneda(int monedaId, bool estado) => _datosMoneda.CambiarEstado(monedaId, estado);

        private static string ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Debe indicar el nombre de la moneda.");
            return nombre.Trim();
        }

        private static string ValidarCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Debe indicar el codigo de la moneda (ej. USD, NIO).");
            return codigo.Trim().ToUpperInvariant();
        }

        private static string NormalizarSimbolo(string simbolo)
        {
            return string.IsNullOrWhiteSpace(simbolo) ? null : simbolo.Trim();
        }
    }
}
