using System;
using System.Collections.Generic;
using BaseApi.WebApi.Features.Users.Entities;

namespace BaseApi.WebApi.Features.Users.Dto
{
    public class RoleDto :Role
    {
        public List<TreeNodeDto> Detail { get; set; }


        public RoleDto()
        {
            Detail = new List<TreeNodeDto>();
        }
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Description)) throw new System.Exception("Debe ingresar una descripción");
            if (this.Detail.Count == 0) throw new System.Exception("Debe ingresar al menos un permiso");
            return true;
        }

    }
}
