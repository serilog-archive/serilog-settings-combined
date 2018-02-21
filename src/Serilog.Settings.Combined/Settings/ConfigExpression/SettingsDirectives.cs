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

namespace Serilog.Settings.ConfigExpression
{
    static class SettingsDirectives
    {
        const string UsingDirective = "using";
        const string LevelSwitchDirective = "level-switch";
        const string AuditToDirective = "audit-to";
        const string WriteToDirective = "write-to";
        const string MinimumLevelDirective = "minimum-level";
        const string MinimumLevelControlledByDirective = "minimum-level:controlled-by";
        const string EnrichWithDirective = "enrich";
        const string EnrichWithPropertyDirective = "enrich:with-property";
        const string FilterDirective = "filter";

        public static string Using(string assemblyShortName)
        {
            if (assemblyShortName == null) throw new ArgumentNullException(nameof(assemblyShortName));
            return $"{UsingDirective}:{assemblyShortName}";
        }

        public static string MinimumLevel = MinimumLevelDirective;

        public static string MinimumLevelOverride(string namespacePrefix)
        {
            if (namespacePrefix == null) throw new ArgumentNullException(nameof(namespacePrefix));

            return $"{MinimumLevelDirective}:override:{namespacePrefix}";
        }

        public static string EnrichWithProperty(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            return $"{EnrichWithPropertyDirective}:{propertyName}";
        }

        public static string ParameterlessMethodInvocation(MethodInvocationType invocationType, string methodName)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            string directivePrefix = GetDirectivePrefix(invocationType);

            return $"{directivePrefix}:{methodName}";
        }

        public static string MethodInvocationParameter(MethodInvocationType invocationType, string methodName, string parameterName)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            var directivePrefix = GetDirectivePrefix(invocationType);

            return $"{directivePrefix}:{methodName}.{parameterName}";
        }

        static string GetDirectivePrefix(MethodInvocationType invocationType)
        {
            string directivePrefix;
            switch (invocationType)
            {
                case MethodInvocationType.WriteTo:
                    directivePrefix = WriteToDirective;
                    break;
                case MethodInvocationType.Enrich:
                    directivePrefix = EnrichWithDirective;
                    break;
                case MethodInvocationType.AuditTo:
                    directivePrefix = AuditToDirective;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(invocationType), invocationType, "Unsupported Invocation type");
            }

            return directivePrefix;
        }
    }

    enum MethodInvocationType
    {
        WriteTo,
        AuditTo,
        Enrich
    }
}
