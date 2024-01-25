
using System.Collections.Generic;

namespace BaseApi.WebApi.Features.Users.Dto
{
    public class TreeNodeDto
    {
        public string Key { get; set; }
        public int PermissionId { get; set; }
        public string Data { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public bool Expanded { get; set; }
        public int FatherId { get; set; }
        public int TypeId { get; set; }
        public bool Active { get; set; }
        public int RoleId { get; set; }
        public int PositionId { get; set; }
        public List<TreeNodeDto> Children { get; set; }
    }
}
