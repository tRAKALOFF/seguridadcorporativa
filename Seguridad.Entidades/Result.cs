﻿using System;
using System.Collections.Generic;

namespace Seguridad.Entidades
{
    public class Result
    {
        /// <summary>
        /// <br/><b>Tipo:</b> Result()
        /// </summary>
        public Result()
        {
            IdError = Guid.NewGuid();
            Success = false;
            ErrCode = string.Empty;
            Message = string.Empty;
            Messages = new List<Result>();
        }

        /// <summary>
        /// Describe si el resultado fue satisfactorio [Verdarero | Falso] 
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Describe el código de error de negocio
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// Describe el mensaje de error 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Describe el Identificador del error
        /// </summary>
        public Guid IdError { get; set; }

        /// <summary>
        /// Contiene una coleccion del tipo List<Result> que contiene descripcion de errores adicionales
        /// </summary>
        public List<Result> Messages { get; set; }
    }
}
