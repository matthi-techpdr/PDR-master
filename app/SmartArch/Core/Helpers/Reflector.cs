using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Fasterflect;

using StringNamedFormat;

namespace SmartArch.Core.Helpers
{
    /// <summary>
    /// Represents strongly typed reflection.
    /// Article about strongly typed reflection: http://www.clariusconsulting.net/blogs/kzu/archive/2006/07/06/TypedReflection.aspx
    /// </summary>
    public static class Reflector
    {
        #region Common methods

        /// <summary>
        /// Gets the all public properties of type.
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <returns>The all public properties from specified type.</returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>()
        {
            return Reflector.GetProperties(typeof(T));
        }

        /// <summary>
        /// Gets the all public properties of type.
        /// </summary>
        /// <param name="type">The specified type.</param>
        /// <returns>
        /// The all public properties from specified type.
        /// </returns>
        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties();
        }

        #endregion

        #region Generic class
        /// <summary>
        /// Gets the concrete generic class.
        /// </summary>
        /// <param name="genericType">Type of the generic.</param>
        /// <param name="concreteGenericType">Type of the concrete generic.</param>
        /// <returns>The concrete generic class.</returns>
        public static Type GetGenericClass(Type genericType, params Type[] concreteGenericType)
        {
            var targetGenericType = genericType.MakeGenericType(concreteGenericType);

            return targetGenericType;
        }
        #endregion

        #region Method

        /// <summary>
        /// Get method info by the specified method expression.
        /// </summary>
        /// <param name="method">The method expression.</param>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <returns>The method info.</returns>
        public static MethodInfo Method<TDeclaringType>(Expression<Action<TDeclaringType>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Get method info by the specified method expression.
        /// </summary>
        /// <param name="method">The method expression.</param>
        /// <returns>The method info.</returns>
        public static MethodInfo Method(Expression<Operation> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Get method info by the specified method expression.
        /// </summary>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <param name="method">The method expression.</param>
        /// <returns>The method info.</returns>
        public static MethodInfo Method<TDeclaringType>(Expression<Operation<TDeclaringType>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Get method info by the specified method expression.
        /// </summary>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <typeparam name="TA0">The type of the argument.</typeparam>
        /// <param name="method">The method expression.</param>
        /// <returns>The method info.</returns>
        public static MethodInfo Method<TDeclaringType, TA0>(Expression<Operation<TDeclaringType, TA0>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Get method info by the specified method expression.
        /// </summary>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <typeparam name="TA0">The type of the 0 argument.</typeparam>
        /// <typeparam name="TA1">The type of the 1 argument.</typeparam>
        /// <param name="method">The method expression.</param>
        /// <returns>The method info.</returns>
        public static MethodInfo Method<TDeclaringType, TA0, TA1>(Expression<Operation<TDeclaringType, TA0, TA1>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Get method info by the specified method expression.
        /// </summary>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <typeparam name="TA0">The type of the 0 argument.</typeparam>
        /// <typeparam name="TA1">The type of the 1 argument.</typeparam>
        /// <typeparam name="TA2">The type of the 2 argument.</typeparam>
        /// <param name="method">The method expression.</param>
        /// <returns>The method info.</returns>
        public static MethodInfo Method<TDeclaringType, TA0, TA1, TA2>(Expression<Operation<TDeclaringType, TA0, TA1, TA2>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Get method info by the specified method expression.
        /// </summary>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <typeparam name="TA0">The type of the 0 argument.</typeparam>
        /// <typeparam name="TA1">The type of the 1 argument.</typeparam>
        /// <typeparam name="TA2">The type of the 2 argument.</typeparam>
        /// <typeparam name="TA3">The type of the 3 argument.</typeparam>
        /// <param name="method">The method expression.</param>
        /// <returns>The method info.</returns>
        public static MethodInfo Method<TDeclaringType, TA0, TA1, TA2, TA3>(Expression<Operation<TDeclaringType, TA0, TA1, TA2, TA3>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Get method info by the specified method expression.
        /// </summary>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <typeparam name="TA0">The type of the 0 argument.</typeparam>
        /// <typeparam name="TA1">The type of the 1 argument.</typeparam>
        /// <typeparam name="TA2">The type of the 2 argument.</typeparam>
        /// <typeparam name="TA3">The type of the 3 argument.</typeparam>
        /// <typeparam name="TA4">The type of the 4 argument.</typeparam>
        /// <param name="method">The method expression.</param>
        /// <returns>The method info.</returns>
        public static MethodInfo Method<TDeclaringType, TA0, TA1, TA2, TA3, TA4>(Expression<Operation<TDeclaringType, TA0, TA1, TA2, TA3, TA4>> method)
        {
            return GetMethodInfo(method);
        }

        /// <summary>
        /// Gets the method info by method expression.
        /// </summary>
        /// <param name="method">The method expression.</param>
        /// <returns>The method info</returns>
        private static MethodInfo GetMethodInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
            {
                throw new ArgumentNullException("method");
            }

            MethodCallExpression methodExpr = null;
            // Our Operation<T> returns an object, so first statement can be either 
            // a cast (if method does not return an object) or the direct method call.
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                // The cast is an unary expression, where the operand is the 
                // actual method call expression.
                methodExpr = ((UnaryExpression)lambda.Body).Operand as MethodCallExpression;
            }
            else
            {
                if (lambda.Body.NodeType == ExpressionType.Call)
                {
                    methodExpr = lambda.Body as MethodCallExpression;
                }
            }

            if (methodExpr == null)
            {
                throw new ArgumentException("method");
            }

            return methodExpr.Method;
        }

        #endregion

        #region Member

        /// <summary>
        /// Gets property info by the specified property expression.
        /// </summary>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <param name="property">The property expression.</param>
        /// <returns>The property info.</returns>
        public static PropertyInfo Property<TDeclaringType>(Expression<Operation<TDeclaringType>> property)
        {
            return Property((Expression)property);
        }

        /// <summary>
        /// Gets property info by the specified property expression.
        /// </summary>
        /// <param name="property">The property expression.</param>
        /// <returns>The property info.</returns>
        public static PropertyInfo Property(Expression property)
        {
            PropertyInfo info = GetMemberInfo(property) as PropertyInfo;
            if (info == null)
            {
                throw new ArgumentException("Member is not a property");
            }

            return info;
        }

        /// <summary>
        /// Gets field info by the specified field expression.
        /// </summary>
        /// <typeparam name="TDeclaringType">The type of the declaring type.</typeparam>
        /// <param name="field">The field expression.</param>
        /// <returns>The field info.</returns>
        public static FieldInfo Field<TDeclaringType>(Expression<Operation<TDeclaringType>> field)
        {
            FieldInfo info = GetMemberInfo(field) as FieldInfo;
            if (info == null)
            {
                throw new ArgumentException("Member is not a field");
            }

            return info;
        }

        /// <summary>
        /// Gets the member info by expression.
        /// </summary>
        /// <param name="member">The member expression.</param>
        /// <returns>The member info.</returns>
        private static MemberInfo GetMemberInfo(Expression member)
        {
            LambdaExpression lambda = member as LambdaExpression;
            if (lambda == null)
            {
                throw new ArgumentNullException("member");
            }

            MemberExpression memberExpr = null;

            // Our Operation<T> returns an object, so first statement can be either 
            // a cast (if method does not return an object) or the direct method call.
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                // The cast is an unary expression, where the operand is the 
                // actual method call expression.
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else
            {
                if (lambda.Body.NodeType == ExpressionType.MemberAccess)
                {
                    memberExpr = lambda.Body as MemberExpression;
                }
            }

            if (memberExpr == null)
            {
                throw new ArgumentException("member");
            }

            return memberExpr.Member;
        }

        #endregion

        #region Calls Generic
        /// <summary>
        /// Calls the generic method.
        /// </summary>
        /// <param name="obj">The instance of class with target method.</param>
        /// <param name="name">The name of target generic method.</param>
        /// <param name="type">The type of target generic method.</param>
        /// <param name="parameters">The parameters of target generic method.</param>
        /// <param name="objType">Type of the class with target method.</param>
        /// <returns>The result which returns of target generic method.</returns>
        public static object CallGeneric(object obj, string name, Type type, object[] parameters = null, Type objType = null)
        {
            objType = objType ?? obj.GetType();
            var commonMethod = objType.Method(name);
            if (!commonMethod.IsGenericMethod)
            {
                throw new ArgumentException("Can't call generic method named {name} with type {type} because this is not generic method".NamedFormat(new { name, type }));
            }

            var concreteMethod = commonMethod.MakeGenericMethod(type);
            object result = concreteMethod.Invoke(obj, parameters);

            return result;
        }
        #endregion

        #region Operations definition

        /// <summary>
        /// The delegate of parameterless operation.
        /// </summary>
        /// <returns>The operation result.</returns>
        public delegate object Operation();

        /// <summary>
        /// The delegate of parameterless operation.
        /// </summary>
        /// <typeparam name="T">The type of declaring class.</typeparam>
        /// <param name="declaringType">The declaring class.</param>
        /// <returns> The operation result.</returns>
        public delegate object Operation<in T>(T declaringType);

        /// <summary>
        /// The delegate of operation with 1 argument.
        /// </summary>
        /// <typeparam name="T">The type of declaring class.</typeparam>
        /// <param name="declaringType">The declaring class.</param>
        /// <typeparam name="TA0">The type of 0 argument.</typeparam>
        /// <param name="arg0">The 0 argument.</param>
        /// <returns>The operation result.</returns>
        public delegate object Operation<in T, in TA0>(T declaringType, TA0 arg0);

        /// <summary>
        /// The delegate of operation with 2 argument.
        /// </summary>
        /// <typeparam name="T">The type of declaring class.</typeparam>
        /// <param name="declaringType">The declaring class.</param>
        /// <typeparam name="TA0">The type of 0 argument.</typeparam>
        /// <param name="arg0">The 0 argument.</param>
        /// <typeparam name="TA1">The type of 1 argument.</typeparam>
        /// <param name="arg1">The 1 argument.</param>
        /// <returns>The operation result.</returns>
        public delegate object Operation<in T, in TA0, in TA1>(T declaringType, TA0 arg0, TA1 arg1);

        /// <summary>
        /// The delegate of operation with 3 argument.
        /// </summary>
        /// <typeparam name="T">The type of declaring class.</typeparam>
        /// <param name="declaringType">The declaring class.</param>
        /// <typeparam name="TA0">The type of 0 argument.</typeparam>
        /// <param name="arg0">The 0 argument.</param>
        /// <typeparam name="TA1">The type of 1 argument.</typeparam>
        /// <param name="arg1">The 1 argument.</param>
        /// <typeparam name="TA2">The type of 2 argument.</typeparam>
        /// <param name="arg2">The 2 argument.</param>
        /// <returns>The operation result.</returns>
        public delegate object Operation<in T, in TA0, in TA1, in TA2>(T declaringType, TA0 arg0, TA1 arg1, TA2 arg2);

        /// <summary>
        /// The delegate of operation with 4 argument.
        /// </summary>
        /// <typeparam name="T">The type of declaring class.</typeparam>
        /// <param name="declaringType">The declaring class.</param>
        /// <typeparam name="TA0">The type of 0 argument.</typeparam>
        /// <param name="arg0">The 0 argument.</param>
        /// <typeparam name="TA1">The type of 1 argument.</typeparam>
        /// <param name="arg1">The 1 argument.</param>
        /// <typeparam name="TA2">The type of 2 argument.</typeparam>
        /// <param name="arg2">The 2 argument.</param>
        /// <typeparam name="TA3">The type of 3 argument.</typeparam>
        /// <param name="arg3">The 3 argument.</param>
        /// <returns>The operation result.</returns>
        public delegate object Operation<in T, in TA0, in TA1, in TA2, in TA3>(T declaringType, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3);

        /// <summary>
        /// The delegate of operation with 5 argument.
        /// </summary>
        /// <typeparam name="T">The type of declaring class.</typeparam>
        /// <param name="declaringType">The declaring class.</param>
        /// <typeparam name="TA0">The type of 0 argument.</typeparam>
        /// <param name="arg0">The 0 argument.</param>
        /// <typeparam name="TA1">The type of 1 argument.</typeparam>
        /// <param name="arg1">The 1 argument.</param>
        /// <typeparam name="TA2">The type of 2 argument.</typeparam>
        /// <param name="arg2">The 2 argument.</param>
        /// <typeparam name="TA3">The type of 3 argument.</typeparam>
        /// <param name="arg3">The 3 argument.</param>
        /// <typeparam name="TA4">The type of 4 argument.</typeparam>
        /// <param name="arg4">The 4 argument.</param>
        /// <returns>The operation result.</returns>
        public delegate object Operation<in T, in TA0, in TA1, in TA2, in TA3, in TA4>(T declaringType, TA0 arg0, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4);

        #endregion
    }
}