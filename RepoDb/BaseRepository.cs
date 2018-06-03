﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Interfaces;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// An inherritable base object for all <b>Entity-Based Repositories</b>. This object is usually being inheritted if the 
    /// derived class is meant for entity-based operations with corresponding <i>Data Entity</i> object for data manipulations.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of <i>Data Entity</i> object to be mapped on this repository. This object must inherit the <i>RepoDb.DataEntity</i>
    /// object in order to be qualified as a repository entity.
    /// </typeparam>
    /// <typeparam name="TDbConnection">The type of the <i>System.Data.Common.DbConnection</i> object.</typeparam>
    public abstract class BaseRepository<TEntity, TDbConnection> : IBaseRepository<TEntity, TDbConnection>
        where TEntity : DataEntity
        where TDbConnection : DbConnection
    {
        private readonly DbRepository<TDbConnection> _dbRepository;

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        public BaseRepository(string connectionString)
            : this(connectionString, null, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operations by this repository.</param>
        public BaseRepository(string connectionString, int? commandTimeout)
            : this(connectionString, commandTimeout, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.Cache</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache)
            : this(connectionString, commandTimeout, cache, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.Cache</i> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.ITrace</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace)
            : this(connectionString, commandTimeout, cache, trace, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <i>RepoDb.BaseRepository</i> object.
        /// </summary>
        /// <param name="connectionString">The connection string to be used by this repository.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on every operation by this repository.</param>
        /// <param name="cache">The cache object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.Cache</i> interface.</param>
        /// <param name="trace">The trace object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.ITrace</i> interface.</param>
        /// <param name="statementBuilder">The SQL statement builder object to be used by this repository. This object must implement the <i>RepoDb.Interfaces.ITrace</i> interface.</param>
        public BaseRepository(string connectionString, int? commandTimeout, ICache cache, ITrace trace, IStatementBuilder statementBuilder)
        {
            // Fields
            Cache = (cache ?? new MemoryCache());
            Trace = trace;
            StatementBuilder = (statementBuilder ??
                StatementBuilderMapper.Get(typeof(TDbConnection))?.StatementBuilder ??
                new SqlDbStatementBuilder());

            // Repository
            _dbRepository = new DbRepository<TDbConnection>(connectionString, commandTimeout,
                Cache, Trace, StatementBuilder);
        }

        // CreateConnection

        /// <summary>
        /// Creates a new instance of database connection.
        /// </summary>
        /// <returns>An instance of new database connection.</returns>
        public TDbConnection CreateConnection()
        {
            return DbRepository.CreateConnection();
        }

        // DbRepository

        /// <summary>
        /// Gets the underlying repository used by this repository.
        /// </summary>
        public IDbRepository<TDbConnection> DbRepository => _dbRepository;

        // DbCache

        /// <summary>
        /// Gets the cache object that is being used by this repository.
        /// </summary>
        public ICache Cache { get; }

        // Trace

        /// <summary>
        /// Gets the trace object that is being used by this repository.
        /// </summary>
        public ITrace Trace { get; }

        // StatementBuilder

        /// <summary>
        /// Gets the statement builder object that is being used by this repository.
        /// </summary>
        public IStatementBuilder StatementBuilder { get; }

        // Count

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public int Count(IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public int Count(object where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public int Count(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public int Count(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction);
        }

        // CountAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public Task<int> CountAsync(IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<int> CountAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<int> CountAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<int> CountAsync(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                transaction: transaction);
        }

        // CountBig

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public long CountBig(IDbTransaction transaction = null)
        {
            return DbRepository.CountBig<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public long CountBig(object where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBig<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public long CountBig(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBig<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public long CountBig(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBig<TEntity>(where: where,
                transaction: transaction);
        }

        // CountBigAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public Task<long> CountBigAsync(IDbTransaction transaction = null)
        {
            return DbRepository.CountBigAsync<TEntity>(transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<long> CountBigAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBigAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<long> CountBigAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBigAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Counts the number of rows from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on a given query expression.</returns>
        public Task<long> CountBigAsync(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.CountBigAsync<TEntity>(where: where,
                transaction: transaction);
        }

        // BatchQuery

        /// <summary>
        /// Query the data from the database by batch. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public IEnumerable<TEntity> BatchQuery(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQuery<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        // BatchQueryAsync

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(object where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(IEnumerable<IQueryField> where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        /// <summary>
        /// Query the data from the database by batch based on a given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined on this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="page">The page of the batch to be used on this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public Task<IEnumerable<TEntity>> BatchQueryAsync(IQueryGroup where, int page, int rowsPerBatch,
            IEnumerable<IOrderField> orderBy, IDbTransaction transaction = null)
        {
            return DbRepository.BatchQueryAsync<TEntity>(
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                transaction: transaction);
        }

        // Query

        /// <summary>
        /// Query a data from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public IEnumerable<TEntity> Query(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.Query<TEntity>(
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public IEnumerable<TEntity> Query(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public IEnumerable<TEntity> Query(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public IEnumerable<TEntity> Query(IQueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        // QueryAsync

        /// <summary>
        /// Query a data from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.QueryAsync<TEntity>(transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(object where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        /// <summary>
        /// Query a data from the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <param name="top">The top number of rows to be used on this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used on this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force the repository to query from the database.
        /// </param>
        /// <returns>An enumerable list of An enumerable list of <i>Data Entity</i> object.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(IQueryGroup where, IDbTransaction transaction = null, int? top = 0,
            IEnumerable<IOrderField> orderBy = null, string cacheKey = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                transaction: transaction,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey);
        }

        // Insert

        /// <summary>
        /// Insert a data in the database.
        /// </summary>
        /// <param name="entity">The <i>Data Entity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>Data Entity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public object Insert(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity>(entity: entity,
                transaction: transaction);
        }

        // InsertAsync

        /// <summary>
        /// Insert a data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The <i>Data Entity</i> object to be inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>Data Entity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public Task<object> InsertAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.InsertAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        // InlineUpdate

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression. This update operation is a targetted column-based operation
        /// based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int InlineUpdate(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdate<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // InlineUpdateAsync

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync(object entity, object where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync(object entity, IEnumerable<IQueryField> where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way. This update operation is a targetted
        /// column-based operation based on the columns specified in the data entity.
        /// </summary>
        /// <param name="entity">The dynamic <i>Data Entity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> InlineUpdateAsync(object entity, IQueryGroup where, bool? overrideIgnore = false, IDbTransaction transaction = null)
        {
            return DbRepository.InlineUpdateAsync<TEntity>(entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
                transaction: transaction);
        }

        // Update

        /// <summary>
        /// Updates a data in the database.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update(TEntity entity, object where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Update(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        // UpdateAsync

        /// <summary>
        /// Updates a data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity, object where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity, IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The instance of <i>Data Entity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity, IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        // Delete

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete(object where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Delete(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        // DeleteAsync

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync(object where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync(IEnumerable<IQueryField> where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on a given query expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used  on this operation.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> DeleteAsync(IQueryGroup where, IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        // Merge

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Merge(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int Merge(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
        {
            return DbRepository.Merge<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        // MergeAsync

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database in an asynchronous way. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> MergeAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Merges an existing <i>Data Entity</i> object in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> MergeAsync(TEntity entity, IEnumerable<IField> qualifiers, IDbTransaction transaction = null)
        {
            return DbRepository.MergeAsync<TEntity>(entity: entity,
                qualifiers: qualifiers,
                transaction: transaction);
        }

        // BulkInsert

        /// <summary>
        /// Bulk-inserting the list of <i>Data Entity</i> objects in the database.
        /// </summary>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int BulkInsert(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
        {
            return DbRepository.BulkInsert<TEntity>(entities: entities,
                transaction: transaction);
        }

        // BulkInsertAsync

        /// <summary>
        /// Bulk-inserting the list of <i>Data Entity</i> objects in the database in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="transaction">The transaction to be used on this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> BulkInsertAsync(IEnumerable<TEntity> entities, IDbTransaction transaction = null)
        {
            return DbRepository.BulkInsertAsync<TEntity>(entities: entities,
                transaction: transaction);
        }

        // ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of <i>Data Entity</i> object.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>Data Entity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public IEnumerable<TEntity> ExecuteQuery(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQuery<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteReader</i> method of the 
        /// <i>System.Data.IDataReader</i> object and converts the result back to an enumerable list of <i>Data Entity</i> object.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>Data Entity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public Task<IEnumerable<TEntity>> ExecuteQueryAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteQueryAsync<TEntity>(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteNonQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteNonQuery</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public int ExecuteNonQuery(string commandText, object param = null, CommandType? commandType = null, int?
            commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteNonQuery(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteNonQueryAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteNonQuery</i> method of the
        /// <i>System.Data.IDataReader</i> object and returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public Task<int> ExecuteNonQueryAsync(string commandText, object param = null, CommandType? commandType = null, int?
            commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteNonQueryAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteScalar

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteScalar</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public object ExecuteScalar(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteScalar(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        // ExecuteScalarAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteScalar</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public Task<object> ExecuteScalarAsync(string commandText, object param = null, CommandType? commandType = null,
            int? commandTimeout = null, IDbTransaction transaction = null)
        {
            return DbRepository.ExecuteScalarAsync(commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }
    }
}