














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

    public partial class Le : ReqlExpr {

    
    
    
/// <summary>
/// <para>Compare values, testing if the left-hand value is less than or equal to the right-hand.</para>
/// </summary>
/// <example><para>Example: Test if a player has scored 10 points or less.</para>
/// <code>r.table('players').get(1)('score').le(10).run(conn, callback);
/// </code></example>
        public Le (object arg) : this(new Arguments(arg), null) {
        }
/// <summary>
/// <para>Compare values, testing if the left-hand value is less than or equal to the right-hand.</para>
/// </summary>
/// <example><para>Example: Test if a player has scored 10 points or less.</para>
/// <code>r.table('players').get(1)('score').le(10).run(conn, callback);
/// </code></example>
        public Le (Arguments args) : this(args, null) {
        }
/// <summary>
/// <para>Compare values, testing if the left-hand value is less than or equal to the right-hand.</para>
/// </summary>
/// <example><para>Example: Test if a player has scored 10 points or less.</para>
/// <code>r.table('players').get(1)('score').le(10).run(conn, callback);
/// </code></example>
        public Le (Arguments args, OptArgs optargs)
         : base(TermType.LE, args, optargs) {
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
