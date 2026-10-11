// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 「查验卡券码」请求（官方 <c>code/checkcode</c>；B2c 落地券码运维族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeCheckRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置待查验的券码列表（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public List<string> CodeList { get; set; } = new();
}

/// <summary>
/// 「查验卡券码」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeCheckResponse : MpResponse
{
    /// <summary>获取或设置已存在的券码（官方 <c>exist_code</c>）。</summary>
    [JsonPropertyName("exist_code")]
    public string[]? ExistCodeList { get; set; }

    /// <summary>获取或设置不存在的券码（官方 <c>not_exist_code</c>）。</summary>
    [JsonPropertyName("not_exist_code")]
    public string[]? NotExistCodeList { get; set; }
}

/// <summary>
/// 「导入新券码」请求（官方 <c>code/deposit</c>；[code, code] 形态由微信生成、无需方自导时不用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeDepositRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置待导入券码列表（官方 <c>code</c>，必填；单次上限 100 个）。</summary>
    [JsonPropertyName("code")]
    public List<string> CodeList { get; set; } = new();
}

/// <summary>
/// 「查询导入券码数量」请求（官方 <c>code/getdepositcount</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeGetDepositCountRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}

/// <summary>
/// 「查询导入券码数量」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeGetDepositCountResponse : MpResponse
{
    /// <summary>获取或设置已导入券码数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>
/// 「设置券码不可用」请求（官方 <c>code/unavailable</c>）。
/// </summary>
/// <remarks><b>覆盖删除语义</b>：置为失效后该券码不可再核销（官方约束：一般用于风险防控）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeUnavailableRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，自定义 code 的卡券必填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置失效原因（官方 <c>reason</c>，可选）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// 「更新券码」请求（官方 <c>code/update</c>；仅官方生成的 code 支持替换）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeUpdateRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，自定义 code 的卡券必填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置原券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置新券码（官方 <c>new_code</c>，必填）。</summary>
    [JsonPropertyName("new_code")]
    public string NewCode { get; set; } = string.Empty;
}
