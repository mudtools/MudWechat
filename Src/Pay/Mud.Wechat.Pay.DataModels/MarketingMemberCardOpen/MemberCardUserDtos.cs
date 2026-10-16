// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingMemberCardOpen;

/// <summary>查询用户在品牌下所有会员卡响应·【会员卡列表】 符合条件的会员卡列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549255"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardBrandUserCardsResponseDataItem
{
    /// <summary>【会员卡模板ID】 商户创建微信会员卡模板成功后系统返回的会员卡模板ID（官方字段路径 <c>data.card_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【会员卡code】 会员在一个card_id下的唯一标志，用户领取会员卡后获得的code（官方字段路径 <c>data.code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

}

/// <summary>查询用户在品牌下所有会员卡响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549255"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardBrandUserCardsResponse : WechatPayResponse
{
    /// <summary>【会员卡列表】 符合条件的会员卡列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MemberCardBrandUserCardsResponseDataItem>? Data { get; set; }

    /// <summary>【总数量】 总数量（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页页码】 分页页码（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

}
