














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

    public partial class Or : ReqlExpr {

    
    
    
/// <summary>
/// <para>Compute the logical "or" of one or more values.</para>
/// </summary>
/// <example><para>Example: Return whether either <code>a</code> or <code>b</code> evaluate to true.</para>
/// <code>var a = true, b = false;
/// r.expr(a).or(b).run(conn, callback);
/// // result passed to callback
/// true
/// </code></example>
        public Or (object arg) : this(new Arguments(arg), null) {
        }
/// <summary>
/// <para>Compute the logical "or" of one or more values.</para>
/// </summary>
/// <example><para>Example: Return whether either <code>a</code> or <code>b</code> evaluate to true.</para>
/// <code>var a = true, b = false;
/// r.expr(a).or(b).run(conn, callback);
/// // result passed to callback
/// true
/// </code></example>
        public Or (Arguments args) : this(args, null) {
        }
/// <summary>
/// <para>Compute the logical "or" of one or more values.</para>
/// </summary>
/// <example><para>Example: Return whether either <code>a</code> or <code>b</code> evaluate to true.</para>
/// <code>var a = true, b = false;
/// r.expr(a).or(b).run(conn, callback);
/// // result passed to callback
/// true
/// </code></example>
        public Or (Arguments args, OptArgs optargs)
         : base(TermType.OR, args, optargs) {
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
