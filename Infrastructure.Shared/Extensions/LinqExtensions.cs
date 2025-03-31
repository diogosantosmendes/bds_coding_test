using Application.Shared.Extensions;
using Application.Shared.Types;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Shared.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Filter? filter)
        {
            if (filter != null)
            {
                Type entityType = typeof(T);
                ParameterExpression parameter = Expression.Parameter(entityType);
                Expression? expression = null;
                if (filter.Group != null && filter.Logical != null && filter.Group.Count > 0)
                {
                    expression = GenerateFilterGroupExpression(entityType, parameter, filter.Logical.Value, filter.Group);
                }
                else
                {
                    expression = GenerateFilterExpression(entityType, parameter, filter);
                }

                if (expression != null)
                {
                    query = query.Where(Expression.Lambda<Func<T, bool>>(expression, parameter));
                }
            }
            return query;
        }

        private static Expression? GenerateFilterGroupExpression(Type entityType, ParameterExpression parameter, Filter.FilterLogicalOperator logical, List<Filter> group)
        {
            List<Expression> expressions = new List<Expression> { };
            foreach (var filter in group)
            {
                Expression? expression = null;
                if (filter.Group != null && filter.Logical != null && filter.Group.Count > 0)
                {
                    expression = GenerateFilterGroupExpression(entityType, parameter, filter.Logical.Value, filter.Group);
                }
                else
                {
                    expression = GenerateFilterExpression(entityType, parameter, filter);
                }

                if (expression != null)
                {
                    expressions.Add(expression);
                }
            }
            if (expressions.Count > 0)
            {
                if (expressions.Count == 1)
                {
                    return expressions[0];
                }
                else
                {
                    var isAND = logical == Filter.FilterLogicalOperator.AND;

                    var logicalExpression = isAND ?
                        Expression.And(expressions[0], expressions[1]) :
                        Expression.Or(expressions[0], expressions[1]);
                    if (expressions.Count > 2)
                    {
                        for (var i = 2; i < expressions.Count; i++)
                        {
                            logicalExpression = isAND ?
                            Expression.And(logicalExpression, expressions[i]) :
                            Expression.Or(logicalExpression, expressions[i]);
                        }
                    }

                    return logicalExpression;
                }
            }
            return null;
        }

        private static Expression? GenerateFilterExpression(Type entityType, ParameterExpression parameter, Filter filter)
        {
            if (!string.IsNullOrEmpty(filter.Field) && !string.IsNullOrEmpty(filter.Value))
            {
                var property = GetMemberExpressionForField(filter.Field, entityType, parameter);

                if (property.HasValue)
                {

                    if (property.Value.info.PropertyType == StaticTypes.String)
                    {
                        ConstantExpression constant = Expression.Constant(filter.Value, StaticTypes.String);
                        return filter.Comparator switch
                        {
                            Filter.FilterComparisionOperator.CONTAINS =>
                                Expression.Call(property.Value.expression, GetMethod(StaticTypes.String, "Contains"), constant),
                            Filter.FilterComparisionOperator.NOT_CONTAINS =>
                                Expression.Not(Expression.Call(property.Value.expression, GetMethod(StaticTypes.String, "Contains"), constant)),
                            Filter.FilterComparisionOperator.START_WITH =>
                                Expression.Call(property.Value.expression, GetMethod(StaticTypes.String, "StartsWith"), constant),
                            Filter.FilterComparisionOperator.END_WITH =>
                                Expression.Call(property.Value.expression, GetMethod(StaticTypes.String, "EndsWith"), constant),
                            Filter.FilterComparisionOperator.GREATER =>
                                Expression.GreaterThan(Expression.Call(property.Value.expression, GetMethod(StaticTypes.String, "CompareTo"), constant), Expression.Constant(0)),
                            Filter.FilterComparisionOperator.GREATER_OR_EQUAL =>
                                Expression.GreaterThanOrEqual(Expression.Call(property.Value.expression, GetMethod(StaticTypes.String, "CompareTo"), constant), Expression.Constant(0)),
                            Filter.FilterComparisionOperator.LESSER =>
                                Expression.LessThan(Expression.Call(property.Value.expression, GetMethod(StaticTypes.String, "CompareTo"), constant), Expression.Constant(0)),
                            Filter.FilterComparisionOperator.LESSER_OR_EQUAL =>
                                Expression.LessThanOrEqual(Expression.Call(property.Value.expression, GetMethod(StaticTypes.String, "CompareTo"), constant), Expression.Constant(0)),
                            Filter.FilterComparisionOperator.EQUAL =>
                                Expression.Equal(property.Value.expression, constant),
                            Filter.FilterComparisionOperator.NOT_EQUAL =>
                                Expression.NotEqual(property.Value.expression, constant),
                            _ => null
                        };
                    }
                    else if (property.Value.info.PropertyType == StaticTypes.DateTime)
                    {
                        ConstantExpression constant = Expression.Constant(DateTime.Parse(filter.Value), StaticTypes.DateTime);
                        return filter.Comparator switch
                        {
                            Filter.FilterComparisionOperator.GREATER =>
                                Expression.GreaterThan(Expression.Call(property.Value.expression, GetMethod(StaticTypes.DateTime, "CompareTo"), constant), Expression.Constant(0)),
                            Filter.FilterComparisionOperator.GREATER_OR_EQUAL =>
                                Expression.GreaterThanOrEqual(Expression.Call(property.Value.expression, GetMethod(StaticTypes.DateTime, "CompareTo"), constant), Expression.Constant(0)),
                            Filter.FilterComparisionOperator.LESSER =>
                                Expression.LessThan(Expression.Call(property.Value.expression, GetMethod(StaticTypes.DateTime, "CompareTo"), constant), Expression.Constant(0)),
                            Filter.FilterComparisionOperator.LESSER_OR_EQUAL =>
                                Expression.LessThanOrEqual(Expression.Call(property.Value.expression, GetMethod(StaticTypes.DateTime, "CompareTo"), constant), Expression.Constant(0)),
                            Filter.FilterComparisionOperator.EQUAL =>
                                Expression.Equal(property.Value.expression, constant),
                            Filter.FilterComparisionOperator.NOT_EQUAL =>
                                Expression.NotEqual(property.Value.expression, constant),
                            _ => null
                        };
                    }
                }
                else if (property.Value.info.PropertyType.IsEnum)
                {
                    ConstantExpression constant = Expression.Constant((int)Enum.Parse(property.Value.info.PropertyType, filter.Value));
                    return filter.Comparator switch
                    {
                        Filter.FilterComparisionOperator.EQUAL =>
                            Expression.Equal(Expression.Convert(property.Value.expression, StaticTypes.Int), constant),
                        Filter.FilterComparisionOperator.NOT_EQUAL =>
                            Expression.NotEqual(Expression.Convert(property.Value.expression, StaticTypes.Int), constant),
                        Filter.FilterComparisionOperator.GREATER =>
                            Expression.GreaterThan(Expression.Convert(property.Value.expression, StaticTypes.Int), constant),
                        Filter.FilterComparisionOperator.GREATER_OR_EQUAL =>
                            Expression.GreaterThanOrEqual(Expression.Convert(property.Value.expression, StaticTypes.Int), constant),
                        Filter.FilterComparisionOperator.LESSER =>
                            Expression.LessThan(Expression.Convert(property.Value.expression, StaticTypes.Int), constant),
                        Filter.FilterComparisionOperator.LESSER_OR_EQUAL =>
                            Expression.LessThanOrEqual(Expression.Convert(property.Value.expression, StaticTypes.Int), constant),
                        _ => null
                    };
                }
                else
                {
                    ConstantExpression constant = Expression.Constant(TypeDescriptor.GetConverter(property.Value.info.PropertyType).ConvertFromString(filter.Value), property.Value.info.PropertyType);
                    if (constant.Value != null)
                    {
                        return filter.Comparator switch
                        {
                            Filter.FilterComparisionOperator.GREATER =>
                                Expression.GreaterThan(property.Value.expression, constant),
                            Filter.FilterComparisionOperator.GREATER_OR_EQUAL =>
                                Expression.GreaterThanOrEqual(property.Value.expression, constant),
                            Filter.FilterComparisionOperator.LESSER =>
                                Expression.LessThan(property.Value.expression, constant),
                            Filter.FilterComparisionOperator.LESSER_OR_EQUAL =>
                                Expression.LessThanOrEqual(property.Value.expression, constant),
                            Filter.FilterComparisionOperator.EQUAL =>
                                Expression.Equal(property.Value.expression, constant),
                            Filter.FilterComparisionOperator.NOT_EQUAL =>
                                Expression.NotEqual(property.Value.expression, constant),
                            _ => null
                        };
                    }
                }
            }
            return null;
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, Sort? sort)
        {
            if (sort != null && !string.IsNullOrEmpty(sort.By))
            {
                Type entityType = typeof(T);
                ParameterExpression parameter = Expression.Parameter(entityType);
                var property = GetMemberExpressionForField(sort.By, entityType, parameter);

                if (property.HasValue)
                {
                    LambdaExpression sortExpression = Expression.Lambda(property.Value.expression, parameter);
                    return query.Provider.CreateQuery<T>(
                        Expression.Call(
                            typeof(Queryable),
                            sort.Desc == true ? "OrderByDescending" : "OrderBy",
                            new[] { entityType, property.Value.expression.Type },
                            query.Expression,
                            Expression.Quote(sortExpression)
                        )
                    );
                }
            }
            return query;
        }

        private static MethodInfo GetMethod(Type type, string methodName)
        {
            var method = type.GetMethod(methodName, new[] { type });
            if (method != null)
            {
                return method;
            }
            else
            {
                throw new InvalidOperationException($"Type {type} does not contains the method {methodName}.");
            }
        }

        private static (PropertyInfo info, MemberExpression expression)? GetMemberExpressionForField(string field, Type parentType, ParameterExpression parentParameter)
        {

            var steps = field.Split('.').Select(x => x.ToCapitalized());
            PropertyInfo? selectorPropertyInfo = parentType.GetProperty(steps.ElementAt(0));

            if (selectorPropertyInfo != null)
            {
                MemberExpression property = Expression.Property(parentParameter, selectorPropertyInfo);

                var length = steps.Count();
                if (length > 1)
                {
                    for (int i = 1; i < length; i++)
                    {
                        if (selectorPropertyInfo is not null)
                        {
                            selectorPropertyInfo = selectorPropertyInfo.PropertyType.GetProperty(steps.ElementAt(i));
                            property = Expression.Property(property, selectorPropertyInfo);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                return (selectorPropertyInfo, property);
            }
            else
            {
                return null;

            }
        }

        public static class StaticTypes
    {
        public static readonly Type String = typeof(string);
        public static readonly Type DateTime = typeof(DateTime);
        public static readonly Type Int = typeof(int);
    }

}
}
