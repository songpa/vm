﻿using System;
using System.Data.SqlClient;
using System.Linq;

namespace vm.Aspects.Model.EFRepository
{
    /// <summary>
    /// Class SqlExceptionExtensions. Extension methods for categorizing instances of <see cref="T:SqlException"/>.
    /// </summary>
    public static class SqlExceptionExtensions
    {
        /// <summary>
        /// Determines whether an exception is caused by an SQL Server connection problem.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// 	<see langword="true"/> if the exception is an SQL connection problem (deadlock); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsSqlConnectionProblem(
            this Exception exception)
        {
            if (exception==null)
                return false;

            var sqlException = exception as SqlException;

            if (sqlException == null)
                sqlException = exception.InnerException as SqlException;

            if (sqlException == null)
                return false;

            return sqlException.IsConnectionProblem();
        }

        /// <summary>
        /// Determines whether an exception is caused by an SQL Server transaction problem.
        /// </summary>
        /// <param name="exception">The x.</param>
        /// <returns>
        /// 	<see langword="true"/> if the exception is an SQL transaction problem (deadlock); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsSqlTransactionProblem(
            this Exception exception)
        {
            if (exception==null)
                return false;

            var sqlException = exception as SqlException;

            if (sqlException == null)
                sqlException = exception.InnerException as SqlException;

            if (sqlException == null)
                return false;

            return sqlException.IsTransactionProblem();
        }

        static readonly int[] _sqlConnectionNumbers = new int[]
        {
            -2,
            -1,
            2,
            53,
            64,
            121,
            233,
            1231,
            10054,
            10060,
            10061,
        };

        static readonly int[] _sqlTransactionNumbers = new int[]
        {
            1204,
            1205,
            1222,
        };

        /// <summary>
        /// Determines whether an exception is caused by an SQL Server connection problem.
        /// </summary>
        /// <param name="sqlException">The exception.</param>
        /// <returns>
        /// 	<see langword="true"/> if the exception is an SQL connection problem; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsConnectionProblem(
            this SqlException sqlException)
        {
            if (sqlException==null)
                return false;

            return _sqlConnectionNumbers.Contains(sqlException.Number);
        }

        /// <summary>
        /// Determines whether an exception is caused by an SQL Server transaction problem.
        /// </summary>
        /// <param name="sqlException">The x.</param>
        /// <returns>
        /// 	<see langword="true"/> if the exception is an SQL transaction problem (deadlock); otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsTransactionProblem(
            this SqlException sqlException)
        {
            if (sqlException==null)
                return false;

            return _sqlTransactionNumbers.Contains(sqlException.Number);
        }
    }
}
