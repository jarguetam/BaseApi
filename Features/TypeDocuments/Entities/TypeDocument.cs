using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseApi.WebApi.Features.TypeDocuments.Entities
{
	public class TypeDocument
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedBy { get; set; }
		public DateTime UpdateDate { get; set; }
		public int UpdateBy { get; set; }

		public bool IsValid()
		{
			if ((string.IsNullOrEmpty(this.Name))) throw new Exception("Debe ingresar el nombre");
			return true;
		}

		public class Map
		{
			public Map(EntityTypeBuilder<TypeDocument> builder)
            {
				builder.HasKey(x => x.Id);
				builder.Property(x => x.Name).HasColumnName("Name");
				builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
				builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate");
				builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
				builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
				builder.ToTable("TypeDocument");
            }
	    }



	}
}
