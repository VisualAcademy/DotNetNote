namespace DotNetNote.Models
{
    /// <summary>
    /// 캐비넷 타입 관리 모델 클래스 
    /// </summary>
    public class CabinetType
    {
        public long Id { get; set; }

        public string? Identification { get; set; }

        public bool? Show { get; set; }

        public bool Adjusted { get; set; } = false;
    }
}
