














//AUTOGENERATED, DO NOTMODIFY.
//Do not edit this file directly.

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
// ReSharper disable CheckNamespace

using System;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Proto;
using System.Collections;
using System.Collections.Generic;


namespace RethinkDb.Driver.Ast {

    public partial class Day : ReqlExpr {

    
    
    
/// <summary>
/// <para>Return the day of a time object as a number between 1 and 31.</para>
/// </summary>
/// <example><para>Example: Return the users born on the 24th of any month.</para>
/// <code>r.table("users").filter(
///     r.row("birthdate").day().eq(24)
/// ).run(conn, callback)
/// </code></example>
        public Day (object arg) : this(new Arguments(arg), null) {
        }
/// <summary>
/// <para>Return the day of a time object as a number between 1 and 31.</para>
/// </summary>
/// <example><para>Example: Return the users born on the 24th of any month.</para>
/// <code>r.table("users").filter(
///     r.row("birthdate").day().eq(24)
/// ).run(conn, callback)
/// </code></example>
        public Day (Arguments args) : this(args, null) {
        }
/// <summary>
/// <para>Return the day of a time object as a number between 1 and 31.</para>
/// </summary>
/// <example><para>Example: Return the users born on the 24th of any month.</para>
/// <code>r.table("users").filter(
///     r.row("birthdate").day().eq(24)
/// ).run(conn, callback)
/// </code></example>
        public Day (Arguments args, OptArgs optargs)
         : base(TermType.DAY, args, optargs) {
        }


    



    


    

    
        /// <summary>
        /// Get a single field from an object. If called on a sequence, gets that field from every object in the sequence, skipping objects that lack it.
        /// </summary>
        /// <param name="bracket"></param>
        public new Bracket this[string bracket] => base[bracket];
        
        /// <summary>
        /// Get the nth element of a sequence, counting from zero. If the argument is negative, count from the last element.
        /// </summary>
        /// <param name="bracket"></param>
        /// <returns></returns>
        public new Bracket this[int bracket] => base[bracket];


    

    


    
    }
}
