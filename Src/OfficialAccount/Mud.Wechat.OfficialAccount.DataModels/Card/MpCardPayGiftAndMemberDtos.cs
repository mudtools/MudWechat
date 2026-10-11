// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 付款礼品卡规则基础信息（官方 <c>base_info</c>；add 与 getbyid 共用形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftBaseInfo
{
    /// <summary>获取或设置适用商户号列表（官方 <c>mchid_list</c>，必填）。</summary>
    [JsonPropertyName("mchid_list")]
    public List<string> MchIdList { get; set; } = new();

    /// <summary>获取或设置生效时间（官方 <c>begin_time</c>，秒级 Unix 时间戳，必填）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>获取或设置失效时间（官方 <c>end_time</c>，秒级 Unix 时间戳，必填）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置规则状态（官方 <c>status</c>；仅 getbyid 响应返回）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>；仅 getbyid 响应返回）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>；仅 getbyid 响应返回）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}

/// <summary>
/// 付款礼品卡会员卡规则（官方 <c>member_rule</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftMemberRule
{
    /// <summary>获取或设置会员卡 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置最低消费金额（官方 <c>least_cost</c>，单位分，必填）。</summary>
    [JsonPropertyName("least_cost")]
    public int LeastCost { get; set; }

    /// <summary>获取或设置最高优惠金额（官方 <c>max_cost</c>，单位分，必填）。</summary>
    [JsonPropertyName("max_cost")]
    public int MaxCost { get; set; }

    /// <summary>获取或设置跳转链接（官方 <c>jump_url</c>，可选）。</summary>
    [JsonPropertyName("jump_url")]
    public string? JumpUrl { get; set; }

    /// <summary>获取或设置互联网品牌 ID（官方 <c>app_brand_id</c>，可选）。</summary>
    [JsonPropertyName("app_brand_id")]
    public string? AppBrandId { get; set; }

    /// <summary>获取或设置互联网品牌 Pass ID（官方 <c>app_brand_pass</c>，可选）。</summary>
    [JsonPropertyName("app_brand_pass")]
    public string? AppBrandPass { get; set; }
}

/// <summary>
/// 付款礼品卡规则（官方 <c>rule_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftRule
{
    /// <summary>获取或设置规则类型（官方 <c>type</c>；当前固定 RULE_TYPE_PAY_MEMBER_CARD）。</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "RULE_TYPE_PAY_MEMBER_CARD";

    /// <summary>获取或设置规则基础信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpCardPayGiftBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置会员卡规则（官方 <c>member_rule</c>，可选）。</summary>
    [JsonPropertyName("member_rule")]
    public MpCardPayGiftMemberRule? MemberRule { get; set; }
}

/// <summary>
/// 「创建付款礼品卡规则」请求（官方 <c>paygiftcard/add</c>；需商户号与支付资质）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftAddRequest
{
    /// <summary>获取或设置规则信息（官方 <c>rule_info</c>，必填）。</summary>
    [JsonPropertyName("rule_info")]
    public MpCardPayGiftRule RuleInfo { get; set; } = new();
}

/// <summary>
/// 规则创建失败的商户项（官方 <c>fail_mchid_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftFailedMerchant
{
    /// <summary>获取或设置商户号（官方 <c>mchid</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>获取或设置错误码（官方 <c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public int ErrorCode { get; set; }

    /// <summary>获取或设置错误信息（官方 <c>errmsg</c>，可选）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }

    /// <summary>获取或设置占用规则 ID（官方 <c>occupy_rule_id</c>，可选）。</summary>
    [JsonPropertyName("occupy_rule_id")]
    public int? OccupyRuleId { get; set; }

    /// <summary>获取或设置占用方 AppId（官方 <c>occupy_appid</c>，可选）。</summary>
    [JsonPropertyName("occupy_appid")]
    public string? OccupyAppId { get; set; }
}

/// <summary>
/// 「创建付款礼品卡规则」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftAddResponse : MpResponse
{
    /// <summary>获取或设置规则 ID（官方 <c>rule_id</c>）。</summary>
    [JsonPropertyName("rule_id")]
    public int RuleId { get; set; }

    /// <summary>获取或设置创建失败商户列表（官方 <c>fail_mchid_list</c>，可选）。</summary>
    [JsonPropertyName("fail_mchid_list")]
    public MpCardPayGiftFailedMerchant[]? FailMchIdList { get; set; }

    /// <summary>获取或设置创建成功商户列表（官方 <c>succ_mchid_list</c>）。</summary>
    [JsonPropertyName("succ_mchid_list")]
    public string[]? SuccMchIdList { get; set; }
}

/// <summary>
/// 「批量查询付款礼品卡规则」请求（官方 <c>paygiftcard/batchget</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftBatchGetRequest
{
    /// <summary>获取或设置规则类型（官方 <c>type</c>；当前固定 RULE_TYPE_PAY_MEMBER_CARD）。</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "RULE_TYPE_PAY_MEMBER_CARD";

    /// <summary>获取或设置是否查询生效规则（官方 <c>effective</c>，必填）。</summary>
    [JsonPropertyName("effective")]
    public bool Effective { get; set; }

    /// <summary>获取或设置起始偏移（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置拉取数量（官方 <c>count</c>，必填；官方默认 10）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; } = 10;
}

/// <summary>
/// 「批量查询付款礼品卡规则」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftBatchGetResponse : MpResponse
{
    /// <summary>获取或设置规则列表（官方 <c>rule_list</c>）。</summary>
    [JsonPropertyName("rule_list")]
    public MpCardPayGiftRule[]? RuleList { get; set; }

    /// <summary>获取或设置规则总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}

/// <summary>
/// 「删除付款礼品卡规则」请求（官方 <c>paygiftcard/delete</c>）。
/// </summary>
/// <remarks><b>覆盖删除语义</b>：规则删除后立即失效，已领取用户不受影响。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftDeleteRequest
{
    /// <summary>获取或设置规则 ID（官方 <c>rule_id</c>，必填）。</summary>
    [JsonPropertyName("rule_id")]
    public int RuleId { get; set; }
}

/// <summary>
/// 「查询付款礼品卡规则」请求（官方 <c>paygiftcard/getbyid</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftGetByIdRequest
{
    /// <summary>获取或设置规则 ID（官方 <c>rule_id</c>，必填）。</summary>
    [JsonPropertyName("rule_id")]
    public int RuleId { get; set; }
}

/// <summary>
/// 「查询付款礼品卡规则」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftGetByIdResponse : MpResponse
{
    /// <summary>获取或设置规则信息（官方 <c>rule_info</c>）。</summary>
    [JsonPropertyName("rule_info")]
    public MpCardPayGiftRule? RuleInfo { get; set; }
}
