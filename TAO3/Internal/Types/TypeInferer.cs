using FastExpressionCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Types
{
    internal static class TypeInferer
    {
        public static void Invoke(
            object obj,
            Type genericTypeDefinition,
            Expression<Action> methodExpression)
        {
            MethodCallExpression bindedMethod = BindMethod(
                obj, 
                genericTypeDefinition, 
                methodExpression);

            Action lambdaExpression = Expression
                .Lambda<Action>(bindedMethod)
                .CompileFast();

            lambdaExpression.Invoke();
        }

        public static R Invoke<R>(
            object obj, 
            Type genericTypeDefinition, 
            Expression<Func<R>> methodExpression)
        {
            MethodCallExpression bindedMethod = BindMethod(
                obj,
                genericTypeDefinition,
                methodExpression);

            Func<R> lambdaExpression = Expression
                .Lambda<Func<R>>(bindedMethod)
                .CompileFast();

            return lambdaExpression.Invoke();
        }

        private static MethodCallExpression BindMethod(
            object obj,
            Type genericTypeDefinition,
            LambdaExpression methodExpression)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            _ = genericTypeDefinition ?? throw new ArgumentNullException(nameof(genericTypeDefinition));
            _ = methodExpression ?? throw new ArgumentNullException(nameof(methodExpression));

            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                throw new ArgumentException($"{nameof(genericTypeDefinition)} must be a generic type definition");
            }

            List<Type> genericTypeMatches = obj.GetType()
                .GetParentTypes()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == genericTypeDefinition)
                .ToList();

            if (genericTypeMatches.Count == 0)
            {
                throw new ArgumentException($"{nameof(obj)} of type {obj.GetType().FullName} does not implement the generic type definition {genericTypeDefinition.FullName}");
            }

            if (genericTypeMatches.Count > 1)
            {
                throw new ArgumentException($"{nameof(obj)} of type {obj.GetType().FullName} implements the generic type definition {genericTypeDefinition.FullName} multiple time.");
            }

            Type genericType = genericTypeMatches[0];

            MethodCallExpression? methodCallExpression = methodExpression.Body as MethodCallExpression;
            if (methodCallExpression == null)
            {
                throw new ArgumentException($"{methodExpression} must contain a MethodCallExpression");
            }

            MethodInfo templateMethodInfo = methodCallExpression.Method;
            Type[] methodGenericArguments = templateMethodInfo.GetGenericArguments();

            Type[] typeGenericArguments = genericType.GetGenericArguments();

            if (typeGenericArguments.Length > methodGenericArguments.Length)
            {
                throw new ArgumentException($"The method in {nameof(methodExpression)} must have at least {typeGenericArguments.Length} type arguments");
            }

            Type[] typeArguments = typeGenericArguments
                .Concat(methodGenericArguments.Skip(typeGenericArguments.Length))
                .ToArray();

            MethodInfo methodInfo = templateMethodInfo
                .GetGenericMethodDefinition()
                .MakeGenericMethod(typeArguments);

            if (methodInfo.IsStatic)
            {
                return Expression.Call(
                    methodInfo,
                    methodCallExpression.Arguments);
            }

            return Expression.Call(
                methodCallExpression.Object,
                methodInfo,
                methodCallExpression.Arguments);
        }
    }
}
