﻿using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to map a .NET CLR type or a class property into a property handler object.
    /// </summary>
    public static class PropertyHandlerMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, object> m_maps = new ConcurrentDictionary<int, object>();

        #endregion

        #region Methods

        #region Type Level

        /*
         * Add
         */

        /// <summary>
        /// Type Level: Adds a mapping between the .NET CLR Type and a property handler.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, TPropertyHandler>(TPropertyHandler propertyHandler,
            bool force = false) =>
            Add(typeof(TType), propertyHandler, force);

        /// <summary>
        /// Type Level: Adds a mapping between the .NET CLR Type and a property handler.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            object propertyHandler,
            bool force = false)
        {
            // Guard the type
            GuardPresence(type);
            Guard(propertyHandler?.GetType());

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var value = (object)null;

            // Try get the mappings
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Override the existing one
                    m_maps.TryUpdate(key, propertyHandler, value);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The property handler mapping for '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                m_maps.TryAdd(key, propertyHandler);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Type Level: Gets the mapped property handler for .NET CLR Type.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>An instance of mapped property handler for .NET CLR Type.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>() =>
            Get<TPropertyHandler>(typeof(TType));

        /// <summary>
        /// Type Level: Gets the mapped property handler for .NET CLR Type.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>An instance of mapped property handler for .NET CLR Type.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(Type type)
        {
            // Check the presence
            GuardPresence(type);

            // Variables for the cache
            var value = (object)null;

            // get the value
            m_maps.TryGetValue(type.FullName.GetHashCode(), out value);

            // Check the result
            if (value == null || value is TPropertyHandler)
            {
                return (TPropertyHandler)value;
            }

            // Throw an exception
            throw new InvalidTypeException($"The cache item is not convertible to '{typeof(TPropertyHandler).FullName}' type.");
        }

        /*
         * Remove
         */

        /// <summary>
        /// Type Level: Removes an existing property handler mapping.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type.</typeparam>
        public static void Remove<T>() =>
            Remove(typeof(T));

        /// <summary>
        /// Type Level: Removes an existing property handler mapping.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        public static void Remove(Type type)
        {
            // Check the presence
            GuardPresence(type);

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var existing = (object)null;

            // Try get the value
            m_maps.TryRemove(key, out existing);
        }

        #endregion

        #region Property Level

        /*
         * Add
         */

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(expression, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            TPropertyHandler propertyHandler,
            bool force)
            where TEntity : class =>
            Add<TPropertyHandler>(ExpressionExtension.GetProperty<TEntity>(expression), propertyHandler, force);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(propertyName, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="propertyHandler">The instance of property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler,
            bool force)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(propertyName, "PropertyName");

            // Get the property
            var property = TypeExtension.GetProperty<TEntity>(propertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{propertyName}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add<TPropertyHandler>(property, propertyHandler, force);
        }

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(field, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler,
            bool force)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(field, "Field");

            // Get the property
            var property = TypeExtension.GetProperty<TEntity>(field.Name);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{field.Name}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add<TPropertyHandler>(property, propertyHandler, force);
        }

        /// <summary>
        /// Property Level: Adds a property handler into a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TPropertyHandler>(ClassProperty classProperty,
            TPropertyHandler propertyHandler) =>
            Add<TPropertyHandler>(classProperty.PropertyInfo, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler into a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TPropertyHandler>(ClassProperty classProperty,
            TPropertyHandler propertyHandler,
            bool force) =>
            Add<TPropertyHandler>(classProperty?.PropertyInfo, propertyHandler, force);

        /// <summary>
        /// Property Level: Adds a property handler into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TPropertyHandler>(PropertyInfo propertyInfo,
            TPropertyHandler propertyHandler) =>
            Add<TPropertyHandler>(propertyInfo, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TPropertyHandler>(PropertyInfo propertyInfo,
            TPropertyHandler propertyHandler,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");
            ThrowNullReferenceException(propertyHandler, "PropertyHandler");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (object)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, propertyHandler, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"A property handler mapping to '{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, propertyHandler);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Property Level: Gets the mapped property handler of the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get<TPropertyHandler>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Gets the mapped property handler of the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(string propertyName)
            where TEntity : class =>
            Get<TPropertyHandler>(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Property Level: Gets the mapped property handler of the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(Field field)
            where TEntity : class =>
            Get<TPropertyHandler>(TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Property Level: Gets the mapped property handler on a specific <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(ClassProperty classProperty) =>
            Get<TPropertyHandler>(classProperty.PropertyInfo);

        /// <summary>
        /// Property Level: Gets the mapped property handler on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (object)null;
            var result = default(TPropertyHandler);

            // Try get the value
            if (m_maps.TryGetValue(key, out value) == true)
            {
                result = Converter.ToType<TPropertyHandler>(value);
            }

            // Return the value
            return result;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Property Level: Removes a mapped property handler from a class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Remove<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Remove(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Removes a mapped property handler from a class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        public static void Remove<TEntity>(string propertyName)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(propertyName, "PropertyName");

            // Get the property
            var property = TypeExtension.GetProperty<TEntity>(propertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{propertyName}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Remove(property);
        }

        /// <summary>
        /// Property Level: Removes a mapped property handler from a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        public static void Remove<TEntity>(Field field)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(field, "Field");

            // Get the property
            var property = TypeExtension.GetProperty<TEntity>(field.Name);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{field.Name}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Remove(property);
        }

        /// <summary>
        /// Property Level: Removes a mapped property handler from a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        public static void Remove(ClassProperty classProperty) =>
            Remove(classProperty.PropertyInfo);

        /// <summary>
        /// Property Level: Removes a mapped property handler from a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        public static void Remove(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (object)null;

            // Try to remove the value
            m_maps.TryRemove(key, out value);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached property handlers.
        /// </summary>
        public static void Clear()
        {
            m_maps.Clear();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Throws an exception if null.
        /// </summary>
        private static void GuardPresence(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("Property handler type.");
            }
        }

        /// <summary>
        /// Throws an exception if the type does not implemented the <see cref="IPropertyHandler{TInput, TResult}"/> interface.
        /// </summary>
        private static void Guard(Type type)
        {
            GuardPresence(type);
            var isInterfacedTo = type.IsInterfacedTo(typeof(IPropertyHandler<,>));
            if (isInterfacedTo == false)
            {
                throw new InvalidTypeException($"Type '{type.FullName}' must implement the '{typeof(IPropertyHandler<,>).FullName}' interface.");
            }
        }

        /// <summary>
        /// Validates the target object presence.
        /// </summary>
        /// <typeparam name="TType">The type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="argument">The name of the argument.</param>
        private static void ThrowNullReferenceException<TType>(TType obj,
            string argument)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"The argument '{argument}' cannot be null.");
            }
        }

        #endregion

        #endregion
    }
}