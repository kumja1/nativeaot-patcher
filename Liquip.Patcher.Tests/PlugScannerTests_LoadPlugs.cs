﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Liquip.API.Attributes;
using Xunit;
using Liquip.Patcher;

namespace Liquip.Patcher.Tests
{
    public class PlugScannerTests_LoadPlugs
    {
        private AssemblyDefinition CreateMockAssembly<T>()
        {
            var assemblyPath = typeof(T).Assembly.Location;
            return AssemblyDefinition.ReadAssembly(assemblyPath);
        }

        [Fact]
        public void LoadPlugs_ShouldFindPluggedClasses()
        {
            // Arrange
            var assembly = CreateMockAssembly<MockPlug>();
            var scanner = new PlugScanner();

            // Act
            var plugs = scanner.LoadPlugs(assembly);

            // Assert
            Assert.Contains(plugs, plug => plug.Name == nameof(MockPlug));
            var plug = plugs.FirstOrDefault(p => p.Name == nameof(MockPlug));
            Assert.NotNull(plug);

            var customAttributes = plug.CustomAttributes;
            Assert.Contains(customAttributes, attr => attr.AttributeType.FullName == typeof(PlugAttribute).FullName);
            var plugAttribute = customAttributes.First(attr => attr.AttributeType.FullName == typeof(PlugAttribute).FullName);

            var expected = typeof(MockTarget).FullName;
            var actual = plugAttribute.ConstructorArguments[0].Value?.ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoadPlugs_ShouldIgnoreClassesWithoutPlugAttribute()
        {
            // Arrange
            var assembly = CreateMockAssembly<NonPlug>();
            var scanner = new PlugScanner();

            // Act
            var plugs = scanner.LoadPlugs(assembly);

            // Assert
            Assert.DoesNotContain(plugs, plug => plug.Name == nameof(NonPlug));
        }

        [Fact]
        public void LoadPlugs_ShouldHandleOptionalPlugs()
        {
            // Arrange
            var assembly = CreateMockAssembly<OptionalPlug>();
            var scanner = new PlugScanner();

            // Act
            var plugs = scanner.LoadPlugs(assembly);
            var optionalPlug = plugs.FirstOrDefault(p => p.Name == nameof(OptionalPlug));

            // Assert
            Assert.NotNull(optionalPlug);

            var customAttributes = optionalPlug.CustomAttributes;
            Assert.Contains(customAttributes, attr => attr.AttributeType.FullName == typeof(PlugAttribute).FullName);
            var plugAttribute = customAttributes.First(attr => attr.AttributeType.FullName == typeof(PlugAttribute).FullName);
            Assert.Equal("OptionalTarget", plugAttribute.ConstructorArguments[0].Value);
            Assert.True((bool)plugAttribute.Properties.FirstOrDefault(p => p.Name == "IsOptional").Argument.Value);
        }
    }
}