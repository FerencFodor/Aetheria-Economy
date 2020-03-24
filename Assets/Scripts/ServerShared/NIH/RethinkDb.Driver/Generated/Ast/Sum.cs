














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

    public partial class Sum : ReqlExpr {

    
    
    
/// <summary>
/// <para>Sums all the elements of a sequence.  If called with a field name,
/// sums all the values of that field in the sequence, skipping elements
/// of the sequence that lack that field.  If called with a function,
/// calls that function on every element of the sequence and sums the
/// results, skipping elements of the sequence where that function returns
/// <code>null</code> or a non-existence error.</para>
/// </summary>
/// <example><para>Example: What's 3 + 5 + 7?</para>
/// <code>r.expr([3, 5, 7]).sum().run(conn, callback)
/// </code></example>
        public Sum (object arg) : this(new Arguments(arg), null) {
        }
/// <summary>
/// <para>Sums all the elements of a sequence.  If called with a field name,
/// sums all the values of that field in the sequence, skipping elements
/// of the sequence that lack that field.  If called with a function,
/// calls that function on every element of the sequence and sums the
/// results, skipping elements of the sequence where that function returns
/// <code>null</code> or a non-existence error.</para>
/// </summary>
/// <example><para>Example: What's 3 + 5 + 7?</para>
/// <code>r.expr([3, 5, 7]).sum().run(conn, callback)
/// </code></example>
        public Sum (Arguments args) : this(args, null) {
        }
/// <summary>
/// <para>Sums all the elements of a sequence.  If called with a field name,
/// sums all the values of that field in the sequence, skipping elements
/// of the sequence that lack that field.  If called with a function,
/// calls that function on every element of the sequence and sums the
/// results, skipping elements of the sequence where that function returns
/// <code>null</code> or a non-existence error.</para>
/// </summary>
/// <example><para>Example: What's 3 + 5 + 7?</para>
/// <code>r.expr([3, 5, 7]).sum().run(conn, callback)
/// </code></example>
        public Sum (Arguments args, OptArgs optargs)
         : base(TermType.SUM, args, optargs) {
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
