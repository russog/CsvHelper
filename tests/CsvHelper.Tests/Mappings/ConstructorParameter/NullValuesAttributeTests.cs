﻿// Copyright 2009-2022 Josh Close
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// https://github.com/JoshClose/CsvHelper
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Tests.Mocks;
using Xunit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelper.Tests.Mappings.ConstructorParameter
{
	
    public class NullValuesAttributeTests
    {
		[Fact]
		public void AutoMap_WithBooleanFalseValuesAttribute_CreatesParameterMaps()
		{
			var context = new CsvContext(new CsvConfiguration(CultureInfo.InvariantCulture));
			var map = context.AutoMap<Foo>();

			Assert.Equal(2, map.ParameterMaps.Count);
			Assert.Empty(map.ParameterMaps[0].Data.TypeConverterOptions.NullValues);
			Assert.Single(map.ParameterMaps[1].Data.TypeConverterOptions.NullValues);
			Assert.Equal("NULL", map.ParameterMaps[1].Data.TypeConverterOptions.NullValues[0]);
		}

		[Fact]
		public void GetRecords_WithBooleanFalseValuesAttribute_HasHeader_CreatesRecords()
		{
			var parser = new ParserMock
			{
				{ "id", "name" },
				{ "1", "NULL" },
			};
			using (var csv = new CsvReader(parser))
			{
				var records = csv.GetRecords<Foo>().ToList();

				Assert.Single(records);
				Assert.Equal(1, records[0].Id);
				Assert.Null(records[0].Name);
			}
		}

		[Fact]
		public void GetRecords_WithBooleanFalseValuesAttribute_NoHeader_CreatesRecords()
		{
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = false,
			};
			var parser = new ParserMock(config)
			{
				{ "1", "NULL" },
			};
			using (var csv = new CsvReader(parser))
			{
				var records = csv.GetRecords<Foo>().ToList();

				Assert.Single(records);
				Assert.Equal(1, records[0].Id);
				Assert.Null(records[0].Name);
			}
		}

		[Fact]
		public void WriteRecords_WithBooleanFalseValuesAttribute_DoesntUseParameterMaps()
		{
			var records = new List<Foo>
			{
				new Foo(1, null),
			};

			using (var writer = new StringWriter())
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteRecords(records);

				var expected = new StringBuilder();
				expected.Append("Id,Name\r\n");
				expected.Append("1,\r\n");

				Assert.Equal(expected.ToString(), writer.ToString());
			}
		}

		private class Foo
		{
			public int Id { get; private set; }

			public string Name { get; private set; }

			public Foo(int id, [NullValues("NULL")] string name)
			{
				Id = id;
				Name = name;
			}
		}
	}
}
