



//AUTOGENERATED, DO NOTMODIFY.
//Do not edit this file directly.

#pragma warning disable 1591
// ReSharper disable CheckNamespace

using System;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Ast;

namespace RethinkDb.Driver {
    public class ReqlPermissionError : ReqlRuntimeError {


        public ReqlPermissionError () {
        }

        public ReqlPermissionError (Exception e) : this(e.Message, e) {
        }

        public ReqlPermissionError (string message) : base(message)
        {
        }

        public ReqlPermissionError (string message, Exception innerException) : base(message, innerException)
        {
        }
        
        
    }
}
