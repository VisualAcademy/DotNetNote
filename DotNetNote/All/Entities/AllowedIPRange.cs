using System;

namespace All.Entities
{
    public class AllowedIPRange
    {
        public int Id { get; set; }
        public string StartIPRange { get; set; }
        public string EndIPRange { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // 기본값 설정
        public long TenantId { get; set; }
    }
}
