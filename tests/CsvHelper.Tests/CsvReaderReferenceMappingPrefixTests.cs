﻿// Copyright 2009-2022 Josh Close
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// https://github.com/JoshClose/CsvHelper
using System.IO;
using System.Linq;
using Xunit;
using CsvHelper.Configuration;
using System.Globalization;

namespace CsvHelper.Tests
{
	
	public class CsvReaderReferenceMappingPrefixTests
	{
		[Fact]
		public void ReferencesWithPrefixTest()
		{
			using (var stream = new MemoryStream())
			using (var reader = new StreamReader(stream))
			using (var writer = new StreamWriter(stream))
			using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				csv.Context.RegisterClassMap<AMap>();

				writer.WriteLine("Id,BPrefix_Id,C.CId");
				writer.WriteLine("a1,b1,c1");
				writer.WriteLine("a2,b2,c2");
				writer.WriteLine("a3,b3,c3");
				writer.WriteLine("a4,b4,c4");
				writer.Flush();
				stream.Position = 0;

				var list = csv.GetRecords<A>().ToList();

				Assert.NotNull(list);
				Assert.Equal(4, list.Count);

				for (var i = 0; i < 4; i++)
				{
					var rowId = i + 1;
					var row = list[i];
					Assert.Equal("a" + rowId, row.Id);
					Assert.Equal("b" + rowId, row.B.Id);
					Assert.Equal("c" + rowId, row.B.C.Id);
				}
			}
		}

		private class A
		{
			public string Id { get; set; }

			public B B { get; set; }
		}

		private class B
		{
			public string Id { get; set; }

			public C C { get; set; }
		}

		private class C
		{
			public string Id { get; set; }
		}

		private sealed class AMap : ClassMap<A>
		{
			public AMap()
			{
				Map(m => m.Id);
				References<BMap>(m => m.B).Prefix("BPrefix_");
			}
		}

		private sealed class BMap : ClassMap<B>
		{
			public BMap()
			{
				Map(m => m.Id);
				References<CMap>(m => m.C).Prefix();
			}
		}

		private sealed class CMap : ClassMap<C>
		{
			public CMap()
			{
				Map(m => m.Id).Name("CId");
			}
		}
	}
}
