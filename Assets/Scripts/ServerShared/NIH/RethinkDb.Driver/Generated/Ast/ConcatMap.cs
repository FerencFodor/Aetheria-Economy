














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

    public partial class ConcatMap : ReqlExpr {

    
    
    
/// <summary>
/// <para>Concatenate one or more elements into a single sequence using a mapping function.</para>
/// </summary>
/// <example><para>Example: Construct a sequence of all monsters defeated by Marvel heroes. The field "defeatedMonsters" is an array of one or more monster names.</para>
/// <code>r.table('marvel').concatMap(function(hero) {
///     return hero('defeatedMonsters')
/// }).run(conn, callback)
/// </code></example>
        public ConcatMap (object arg) : this(new Arguments(arg), null) {
        }
/// <summary>
/// <para>Concatenate one or more elements into a single sequence using a mapping function.</para>
/// </summary>
/// <example><para>Example: Construct a sequence of all monsters defeated by Marvel heroes. The field "defeatedMonsters" is an array of one or more monster names.</para>
/// <code>r.table('marvel').concatMap(function(hero) {
///     return hero('defeatedMonsters')
/// }).run(conn, callback)
/// </code></example>
        public ConcatMap (Arguments args) : this(args, null) {
        }
/// <summary>
/// <para>Concatenate one or more elements into a single sequence using a mapping function.</para>
/// </summary>
/// <example><para>Example: Construct a sequence of all monsters defeated by Marvel heroes. The field "defeatedMonsters" is an array of one or more monster names.</para>
/// <code>r.table('marvel').concatMap(function(hero) {
///     return hero('defeatedMonsters')
/// }).run(conn, callback)
/// </code></example>
        public ConcatMap (Arguments args, OptArgs optargs)
         : base(TermType.CONCAT_MAP, args, optargs) {
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
