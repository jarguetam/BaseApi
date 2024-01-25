using System.Collections.Generic;

namespace BaseApi.WebApi.Features.Common.Dto
{
    public class MenuDto
    {
        public int MenuId { get; set; }
        public string Label { get; set; }
        public string RouterLink { get; set; }
        public string Icon { get; set; }
        public int PositionId { get; set; }
        public List<MenuDto> Items { get; set; }
    }
}
