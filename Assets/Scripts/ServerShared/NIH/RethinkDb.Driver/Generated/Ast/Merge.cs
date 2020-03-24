














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

    public partial class Merge : ReqlExpr {

    
    
    
/// <summary>
/// <para>Merge two or more objects together to construct a new object with properties from all. When there is a conflict between field names, preference is given to fields in the rightmost object in the argument list. <code>merge</code> also accepts a subquery function that returns an object, which will be used similarly to a <a href="/api/javascript/map/">map</a> function.</para>
/// </summary>
/// <example><para>Example: Equip Thor for battle.</para>
/// <code>r.table('marvel').get('thor').merge(
///     r.table('equipment').get('hammer'),
///     r.table('equipment').get('pimento_sandwich')
/// ).run(conn, callback)
/// </code></example>
        public Merge (object arg) : this(new Arguments(arg), null) {
        }
/// <summary>
/// <para>Merge two or more objects together to construct a new object with properties from all. When there is a conflict between field names, preference is given to fields in the rightmost object in the argument list. <code>merge</code> also accepts a subquery function that returns an object, which will be used similarly to a <a href="/api/javascript/map/">map</a> function.</para>
/// </summary>
/// <example><para>Example: Equip Thor for battle.</para>
/// <code>r.table('marvel').get('thor').merge(
///     r.table('equipment').get('hammer'),
///     r.table('equipment').get('pimento_sandwich')
/// ).run(conn, callback)
/// </code></example>
        public Merge (Arguments args) : this(args, null) {
        }
/// <summary>
/// <para>Merge two or more objects together to construct a new object with properties from all. When there is a conflict between field names, preference is given to fields in the rightmost object in the argument list. <code>merge</code> also accepts a subquery function that returns an object, which will be used similarly to a <a href="/api/javascript/map/">map</a> function.</para>
/// </summary>
/// <example><para>Example: Equip Thor for battle.</para>
/// <code>r.table('marvel').get('thor').merge(
///     r.table('equipment').get('hammer'),
///     r.table('equipment').get('pimento_sandwich')
/// ).run(conn, callback)
/// </code></example>
        public Merge (Arguments args, OptArgs optargs)
         : base(TermType.MERGE, args, optargs) {
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
