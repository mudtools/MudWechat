// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 「卡券帐号级统计」请求（官方 <c>getcardbizuininfo</c>；在通用日期区间上追加卡券来源过滤）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardBizUinInfoRequest : MpDateRangeRequest
{
    /// <summary>获取或设置卡券来源（官方 <c>cond_source</c>；0=全部，1=普通券（公众号）来源，2=小程序来源）。</summary>
    [JsonPropertyName("cond_source")]
    public int CardSource { get; set; }
}

/// <summary>
/// 「卡券券级统计」请求（官方 <c>getcardcardinfo</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardCardInfoRequest : MpCardBizUinInfoRequest
{
    /// <summary>获取或设置指定卡券 ID（官方 <c>card_id</c>，可选；不传返回全部卡券合计）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}

/// <summary>
/// 「会员卡统计」请求（官方 <c>getcardmembercarddetail</c>；与 info 端点不同——本端点官方要求指定 card_id）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardMemberCardDetailRequest : MpDateRangeRequest
{
    /// <summary>获取或设置会员卡 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}

/// <summary>
/// 卡券帐号级统计单日数据项（官方 <c>getcardbizuininfo</c> 的 <c>list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardBizUinData
{
    /// <summary>获取或设置统计日期（官方 <c>ref_date</c>，格式 yyyy-MM-dd）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>浏览次数（官方 <c>view_cnt</c>）。</summary>
    [JsonPropertyName("view_cnt")]
    public int ViewCount { get; set; }

    /// <summary>浏览人数（官方 <c>view_user</c>）。</summary>
    [JsonPropertyName("view_user")]
    public int ViewUserCount { get; set; }

    /// <summary>领取次数（官方 <c>receive_cnt</c>）。</summary>
    [JsonPropertyName("receive_cnt")]
    public int ReceiveCount { get; set; }

    /// <summary>领取人数（官方 <c>receive_user</c>）。</summary>
    [JsonPropertyName("receive_user")]
    public int ReceiveUserCount { get; set; }

    /// <summary>核销次数（官方 <c>verify_cnt</c>）。</summary>
    [JsonPropertyName("verify_cnt")]
    public int ConsumeCount { get; set; }

    /// <summary>核销人数（官方 <c>verify_user</c>）。</summary>
    [JsonPropertyName("verify_user")]
    public int ConsumeUserCount { get; set; }

    /// <summary>转赠次数（官方 <c>given_cnt</c>）。</summary>
    [JsonPropertyName("given_cnt")]
    public int TransferCount { get; set; }

    /// <summary>转赠人数（官方 <c>given_user</c>）。</summary>
    [JsonPropertyName("given_user")]
    public int TransferUserCount { get; set; }

    /// <summary>过期次数（官方 <c>expire_cnt</c>）。</summary>
    [JsonPropertyName("expire_cnt")]
    public int ExpireCount { get; set; }

    /// <summary>过期人数（官方 <c>expire_user</c>）。</summary>
    [JsonPropertyName("expire_user")]
    public int ExpireUserCount { get; set; }
}

/// <summary>
/// 卡券券级统计单日数据项（官方 <c>getcardcardinfo</c> 的 <c>list</c> 项；在帐号级字段上追加券标识）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardCardData : MpCardBizUinData
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置卡券类型（官方 <c>card_type</c>）。</summary>
    [JsonPropertyName("card_type")]
    public int CardType { get; set; }
}

/// <summary>
/// 会员卡统计单日数据项（官方 <c>getcardmembercardinfo</c> 的 <c>list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardMemberCardData
{
    /// <summary>获取或设置统计日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>浏览次数（官方 <c>view_cnt</c>）。</summary>
    [JsonPropertyName("view_cnt")]
    public int ViewCount { get; set; }

    /// <summary>浏览人数（官方 <c>view_user</c>）。</summary>
    [JsonPropertyName("view_user")]
    public int ViewUserCount { get; set; }

    /// <summary>领取次数（官方 <c>receive_cnt</c>）。</summary>
    [JsonPropertyName("receive_cnt")]
    public int ReceiveCount { get; set; }

    /// <summary>领取人数（官方 <c>receive_user</c>）。</summary>
    [JsonPropertyName("receive_user")]
    public int ReceiveUserCount { get; set; }

    /// <summary>核销次数（官方 <c>verify_cnt</c>）。</summary>
    [JsonPropertyName("verify_cnt")]
    public int ConsumeCount { get; set; }

    /// <summary>核销人数（官方 <c>verify_user</c>）。</summary>
    [JsonPropertyName("verify_user")]
    public int ConsumeUserCount { get; set; }

    /// <summary>激活次数（官方 <c>active_cnt</c>）。</summary>
    [JsonPropertyName("active_cnt")]
    public int ActiveCount { get; set; }

    /// <summary>激活人数（官方 <c>active_user</c>）。</summary>
    [JsonPropertyName("active_user")]
    public int ActiveUserCount { get; set; }

    /// <summary>历史领取会员卡总人数（官方 <c>total_user</c>）。</summary>
    [JsonPropertyName("total_user")]
    public int TotalUserCount { get; set; }

    /// <summary>历史有效会员卡总人数（官方 <c>total_receive_user</c>）。</summary>
    [JsonPropertyName("total_receive_user")]
    public int TotalReceiveUserCount { get; set; }
}

/// <summary>
/// 会员卡明细统计单日数据项（官方 <c>getcardmembercarddetail</c> 的 <c>list</c> 项；在会员卡统计字段上追加券与商户标识）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardMemberCardDetailData : MpCardMemberCardData
{
    /// <summary>获取或设置会员卡 ID（官方 <c>cardid</c>；官方拼写无下划线，照抄）。</summary>
    [JsonPropertyName("cardid")]
    public string? CardId { get; set; }

    /// <summary>获取或设置商户类别（官方 <c>merchanttype</c>，可选）。</summary>
    [JsonPropertyName("merchanttype")]
    public int? SubMerchantType { get; set; }

    /// <summary>获取或设置子商户 ID（官方 <c>submerchantid</c>，可选）。</summary>
    [JsonPropertyName("submerchantid")]
    public string? SubMerchantId { get; set; }

    /// <summary>新增会员人数（官方 <c>new_user</c>）。</summary>
    [JsonPropertyName("new_user")]
    public int NewUserCount { get; set; }

    /// <summary>会员卡原来实收金额（官方 <c>payOriginalFee</c>；官方驼峰拼写，照抄）。</summary>
    [JsonPropertyName("payOriginalFee")]
    public int PayOriginalFee { get; set; }

    /// <summary>会员卡实收金额（官方 <c>fee</c>）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }
}

/// <summary>
/// 「卡券帐号级统计」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardBizUinInfoResponse : MpResponse
{
    /// <summary>获取或设置按日统计列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpCardBizUinData>? DataList { get; set; }
}

/// <summary>
/// 「卡券券级统计」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardCardInfoResponse : MpResponse
{
    /// <summary>获取或设置按券统计列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpCardCardData>? DataList { get; set; }
}

/// <summary>
/// 「会员卡统计」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardMemberCardInfoResponse : MpResponse
{
    /// <summary>获取或设置按日统计列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpCardMemberCardData>? DataList { get; set; }
}

/// <summary>
/// 「会员卡明细统计」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardMemberCardDetailResponse : MpResponse
{
    /// <summary>获取或设置按券明细列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<MpCardMemberCardDetailData>? DataList { get; set; }
}
