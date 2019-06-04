﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondoraWH.Models
{
    public interface IApiResult
    {
        bool Success { get; set; }
    }

    public interface IApiResult<T> : IApiResult where T : class
    {
        /// <summary>
        /// Result specific payload object
        /// </summary>
        T Payload { get; set; }
    }

    /// <summary>
    /// Result of the API request
    /// </summary>
    public class ApiResult : IApiResult
    {
        /// <summary>
        /// Indicates if the request was successfull or not.
        /// <c>true</c> if the request was handled successfully, <c>false</c> otherwise.
        /// </summary>
        [DefaultValue(true)]
        public bool Success { get; set; }

        /// <summary>
        /// Error(s) accociated with the API request.
        /// </summary>   
        public List<ApiError> Errors { get; set; }
    }

    public class ApiResult<T> : ApiResult, IApiResult<T> where T : class
    {
        /// <summary>
        /// The payload of the response. Type depends on the API request.
        /// </summary>
        public T Payload { get; set; }
    }
}
