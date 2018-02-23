// Copyright 2013-2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Settings.ConfigExpression
{
    class ConfigurationExpressionSettingsSerializer
    {
        MethodCallExpression _methodCallExpression;

        public ConfigurationExpressionSettingsSerializer(Expression<Func<LoggerConfiguration, LoggerConfiguration>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            _methodCallExpression = expression.Body as MethodCallExpression ?? throw new ArgumentException("Expression's body should be a Method call", $"{nameof(expression)}.{nameof(expression.Body)}");
        }

        public IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs()
        {
            return FromRightToLeft(_methodCallExpression).Reverse().SelectMany(x => x);
        }

        static IEnumerable<List<KeyValuePair<string, string>>> FromRightToLeft(MethodCallExpression methodCallExp)
        {
            if (methodCallExp == null) throw new ArgumentNullException(nameof(methodCallExp));

            Expression current = methodCallExp;

            while (current is MethodCallExpression)
            {
                var methodCall = (MethodCallExpression)current;
                var method = methodCall.Method;
                var methodName = method.Name;
                var (methodTarget, normalizedMethodArguments) = ExtractNormalizedTargetAndArguments(methodCall);

                current = methodTarget.Expression;

                switch (methodTarget.Member.Name)
                {
                    case nameof(LoggerConfiguration.MinimumLevel):
                        if (methodName == nameof(LoggerMinimumLevelConfiguration.Override))
                        {
                            // .MinimumLevel.Override(string namespace, LogEventLevel overridenLevel)
                            var overrideNamespace = ((ConstantExpression)normalizedMethodArguments[0]).Value.ToString();
                            var overrideLevel = ConvertExpressionToSettingValue(normalizedMethodArguments[1]);

                            yield return new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>(SettingsDirectives.MinimumLevelOverride(overrideNamespace), overrideLevel)
                            };
                            continue;
                        }
                        // .MinimumLevel.Debug(), MinimumLevel.Information() etc ...
                        if (!Enum.TryParse(methodName, out LogEventLevel minimumLevel))
                            throw new NotImplementedException($"Not supported : MinimumLevel.{methodName}");
                        yield return new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(SettingsDirectives.MinimumLevel, minimumLevel.ToString())
                        };
                        continue;
                    case nameof(LoggerConfiguration.Enrich):
                        if (methodName == nameof(LoggerEnrichmentConfiguration.WithProperty))
                        {
                            // .Enrich.WithProperty(string propertyName, object propertyValue, bool destructureObjects)
                            var enrichPropertyName = ((ConstantExpression)normalizedMethodArguments[0]).Value.ToString();
                            var enrichWithArgument = normalizedMethodArguments[1];
                            var enrichmentValue = ConvertExpressionToSettingValue(enrichWithArgument);
                            yield return new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>(SettingsDirectives.EnrichWithProperty(enrichPropertyName), enrichmentValue)
                            };
                            continue;
                        }
                        else
                        {
                            // method .Enrich.FromLogContext()
                            // or extension method .Enrich.WithBar(param1, param2)
                            yield return SerializeMethodInvocation(MethodInvocationType.Enrich, method, normalizedMethodArguments);
                            continue;

                        }
                    case nameof(LoggerConfiguration.WriteTo):
                        yield return SerializeMethodInvocation(MethodInvocationType.WriteTo, method, normalizedMethodArguments);
                        continue;
                    case nameof(LoggerConfiguration.AuditTo):
                        yield return SerializeMethodInvocation(MethodInvocationType.AuditTo, method, normalizedMethodArguments);
                        continue;
                    default:
                        throw new NotSupportedException($"Not supported : LoggerConfiguration.{methodTarget.Member.Name}");
                }
            }
        }

        static List<KeyValuePair<string, string>> SerializeMethodInvocation(MethodInvocationType methodInvocationType, MethodInfo method, IReadOnlyList<Expression> normalizedMethodArguments)
        {
            var methodName = method.Name;
            var normalizedMethodParameters = ExtractNormalizedParameters(method);
            var resultingDirectives = new List<KeyValuePair<string, string>>();
            // using  
            var enrichAssembly = method.DeclaringType.GetTypeInfo().Assembly;
            var assemblyShortName = enrichAssembly.GetName().Name;
            if (assemblyShortName != "Serilog")
            {
                resultingDirectives.Add(new KeyValuePair<string, string>(SettingsDirectives.Using(assemblyShortName), $"{enrichAssembly.FullName}"));
            }
            var args = normalizedMethodArguments
                .Zip(normalizedMethodParameters, (expression, param) => new
                {
                    MethodArgument = expression,
                    Parameter = param
                })
                .Select(x => new
                {
                    ParamName = x.Parameter.Name,
                    ParamValue = ConvertExpressionToSettingValue(x.MethodArgument)
                })
                .Where(x => x.ParamValue != null);

            var directives2 = args.Select(x => new KeyValuePair<string, string>(SettingsDirectives.MethodInvocationParameter(methodInvocationType, methodName, x.ParamName), x.ParamValue)).ToList();
            if (directives2.Count > 0)
            {
                resultingDirectives.AddRange(directives2);
            }
            else
            {
                resultingDirectives.Add(new KeyValuePair<string, string>(SettingsDirectives.ParameterlessMethodInvocation(methodInvocationType, methodName), ""));
            }
            return resultingDirectives;
        }


        /// <summary>
        /// Extract target and parameters in a consistent way, whether method is a "regular" method call
        /// or an extension method (actually a sttic method where the first parameter is the target)
        /// </summary>
        /// <returns></returns>
        static (MemberExpression target, IReadOnlyList<Expression> normalizedArguments) ExtractNormalizedTargetAndArguments(MethodCallExpression methodCall)
        {
            var method = methodCall.Method;
            MemberExpression leftSide;
            List<Expression> methodArguments;
            if (method.IsStatic)
            {
                // extension method : the first argument is the target
                leftSide = (MemberExpression)methodCall.Arguments[0];
                methodArguments = methodCall.Arguments.Skip(1).ToList();
            }
            else
            {
                // regular method 
                leftSide = (MemberExpression)methodCall.Object;
                methodArguments = methodCall.Arguments.ToList();
            }

            return (target: leftSide, normalizedArguments: methodArguments.AsReadOnly());
        }

        static IReadOnlyList<ParameterInfo> ExtractNormalizedParameters(MethodInfo method)
        {
            if (method.IsStatic)
            {
                // extension method : the first parameter is actually the target !
                return method.GetParameters().Skip(1).ToList().AsReadOnly();
            }

            return method.GetParameters().ToList().AsReadOnly();
        }

        static string ConvertExpressionToSettingValue(Expression exp)
        {
            if (exp == null) throw new ArgumentNullException(nameof(exp));
            switch (exp)
            {
                case ConstantExpression constantExp:
                    if (constantExp.Value == null) return null;
                    return $"{constantExp.Value}";

                // a boolean is a UnaryExpression Convert(true), for some reason 
                case UnaryExpression unaryExp:
                    return $"{unaryExp.Operand}";

                case NewExpression newExp:
                    // constructor new Uri(string uri)
                    if (newExp.Type == typeof(Uri))
                    {
                        return ((ConstantExpression)newExp.Arguments[0]).Value.ToString();
                    }
                    throw new NotImplementedException($"Not supported : new {newExp.Type}(...)");

                default:
                    throw new NotImplementedException($"Cannot extract a string value from `{exp}`");
            }
        }
    }
}
