














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

    public partial class Filter : ReqlExpr {

    
    
    
/// <summary>
/// <para>Return all the elements in a sequence for which the given predicate is true. The return value of <code>filter</code> will be the same as the input (sequence, stream, or array). Documents can be filtered in a variety of ways&mdash;ranges, nested values, boolean conditions, and the results of anonymous functions.</para>
/// </summary>
/// <example><para>Example: Get all users who are 30 years old.</para>
/// <code>r.table('users').filter({age: 30}).run(conn, callback);
/// </code>
/// <para>The predicate <code>{age: 30}</code> selects documents in the <code>users</code> table with an <code>age</code> field whose value is <code>30</code>. Documents with an <code>age</code> field set to any other value <em>or</em> with no <code>age</code> field present are skipped.</para></example>
        public Filter (object arg) : this(new Arguments(arg), null) {
        }
/// <summary>
/// <para>Return all the elements in a sequence for which the given predicate is true. The return value of <code>filter</code> will be the same as the input (sequence, stream, or array). Documents can be filtered in a variety of ways&mdash;ranges, nested values, boolean conditions, and the results of anonymous functions.</para>
/// </summary>
/// <example><para>Example: Get all users who are 30 years old.</para>
/// <code>r.table('users').filter({age: 30}).run(conn, callback);
/// </code>
/// <para>The predicate <code>{age: 30}</code> selects documents in the <code>users</code> table with an <code>age</code> field whose value is <code>30</code>. Documents with an <code>age</code> field set to any other value <em>or</em> with no <code>age</code> field present are skipped.</para></example>
        public Filter (Arguments args) : this(args, null) {
        }
/// <summary>
/// <para>Return all the elements in a sequence for which the given predicate is true. The return value of <code>filter</code> will be the same as the input (sequence, stream, or array). Documents can be filtered in a variety of ways&mdash;ranges, nested values, boolean conditions, and the results of anonymous functions.</para>
/// </summary>
/// <example><para>Example: Get all users who are 30 years old.</para>
/// <code>r.table('users').filter({age: 30}).run(conn, callback);
/// </code>
/// <para>The predicate <code>{age: 30}</code> selects documents in the <code>users</code> table with an <code>age</code> field whose value is <code>30</code>. Documents with an <code>age</code> field set to any other value <em>or</em> with no <code>age</code> field present are skipped.</para></example>
        public Filter (Arguments args, OptArgs optargs)
         : base(TermType.FILTER, args, optargs) {
        }


    



    
///<summary>
/// "default": "T_EXPR"
///</summary>
        public Filter this[object optArgs] {
            get
            {
                var newOptArgs = OptArgs.FromMap(this.OptArgs).With(optArgs);
        
                return new Filter (this.Args, newOptArgs);
            }
        }
        
///<summary>
/// "default": "T_EXPR"
///</summary>
    public Filter this[OptArgs optArgs] {
        get
        {
            var newOptArgs = OptArgs.FromMap(this.OptArgs).With(optArgs);
    
            return new Filter (this.Args, newOptArgs);
        }
    }
    
///<summary>
/// "default": "T_EXPR"
///</summary>
        public Filter OptArg(string key, object val){
            
            var newOptArgs = OptArgs.FromMap(this.OptArgs).With(key, val);
        
            return new Filter (this.Args, newOptArgs);
        }
        internal Filter optArg(string key, object val){
        
            return this.OptArg(key, val);
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
