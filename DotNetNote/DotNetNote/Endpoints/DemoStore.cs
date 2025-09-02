using System.Collections.Concurrent;

namespace DotNetNote.Endpoints;

public static class DemoStore
{
    // 공통 상수
    public const string FixedJwtToken = "TestToken";
    public const string DemoClientGuid = "11111111-1111-1111-1111-111111111111";
    public const string RequestedBy = "Demo Sandbox Instance - Demo Client";
    public const string ClientName = "Demo Client Account";
    public const string ClientCode = "DEMO-CLIENT-CODE";
    public const string OrderTypeEmployment = "Employment";

    // 자동 완료까지 대기 시간(밀리초) - 요구사항: 약 1분
    public const long AutoCompleteAfterMs = 1 * 60 * 1000;

    // Applicants 저장소
    public record ApplicantSummary(
        string applicantGuid,
        string firstName,
        string lastName,
        string gender,
        string dateOfBirth,
        string email,
        string phoneNumber,
        bool textingEnabled,
        long createdDate,
        string createdBy,
        long modifiedDate,
        string modifiedBy,
        int version
    );

    public static readonly ConcurrentDictionary<string, ApplicantSummary> ApplicantsByGuid = new();

    // Products 저장소 (clientProductGuid 기준)
    public record ClientProduct(
        string clientProductGuid,
        string productGuid,
        string productName,
        bool usesQuickapp,
        long? dateModified = null
    );

    // 클라이언트별 사용중인 제품(2개만)
    public static readonly Dictionary<string, List<ClientProduct>> ProductsByClient = new()
    {
        [DemoClientGuid] = new()
        {
            new ClientProduct(
                clientProductGuid: "087854ec-541c-4530-9e37-3622017917ea",
                productGuid: "d3d9aa73-9f30-42b0-8332-84560bbadb6b",
                productName: "API Demo",
                usesQuickapp: false
            ),
            new ClientProduct(
                clientProductGuid: "7fda3b24-b1df-43fc-9ebb-28e63cffbf36",
                productGuid: "5be23a23-fcb4-4507-a460-086fb963f6af",
                productName: "Employment Product (QuickApp)",
                usesQuickapp: true,
                dateModified: 1755638691000
            )
        }
    };

    public static ClientProduct? FindProductByClientProductGuid(string clientGuid, string clientProductGuid)
    {
        if (!ProductsByClient.TryGetValue(clientGuid, out var list)) return null;
        return list.FirstOrDefault(p => p.clientProductGuid.Equals(clientProductGuid, StringComparison.OrdinalIgnoreCase));
    }

    // Orders 저장소
    public record OrderRecord(
        string orderGuid,
        string clientGuid,
        int fileNumber,
        string orderStatus,  // "pending" -> (시간 경과) -> "complete"
        string orderType,
        long orderedDate,
        string generalReportReference,
        string externalIdentifier,
        string applicantGuid,
        string applicantName,
        string clientName,
        string clientCode,
        string productName,
        string requestedBy,
        bool searchFlagged,
        long createdDate,
        string createdBy,
        long modifiedDate,
        string modifiedBy,
        string clientProductGuid,
        string applicantEmail,
        string applicantDateOfBirth,
        string applicantPhone,
        double totalCharges,
        string orderNotes,
        List<string> includedSearches,
        List<object> customFields,
        long? completedDate // complete가 되면 값 설정
    );

    public static readonly ConcurrentDictionary<string, OrderRecord> OrdersByGuid = new();

    // 파일번호 시퀀스 (샘플과 유사하게 670770부터 시작)
    private static int _fileNumber = 670769;
    public static int NextFileNumber() => System.Threading.Interlocked.Increment(ref _fileNumber);

    // 유틸
    public static string ToApplicantNameUpper(string firstName, string lastName)
        => $"{lastName?.Trim().ToUpperInvariant()}, {firstName?.Trim().ToUpperInvariant()}";

    public static void TryAutoComplete(ref OrderRecord order)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (order.orderStatus == "pending" && now - order.orderedDate >= AutoCompleteAfterMs)
        {
            var completed = order.orderedDate + 2000; // 샘플처럼 주문시각+약간 후에 완료로 표기
            order = order with
            {
                orderStatus = "complete",
                completedDate = completed,
                modifiedDate = completed
            };
            OrdersByGuid[order.orderGuid] = order; // 저장소 업데이트
        }
    }
}
