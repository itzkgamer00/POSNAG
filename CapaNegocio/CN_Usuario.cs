﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Usuario
    {
        public CD_Usuario objcd_usuario = new CD_Usuario();

        public List<Usuario> Listar()
        {

            return objcd_usuario.listar();
        }
    }
}
