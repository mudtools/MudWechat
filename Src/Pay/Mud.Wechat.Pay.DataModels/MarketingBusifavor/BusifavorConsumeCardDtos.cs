// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingBusifavor;

/// <summary>发放消费卡请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017164666"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorConsumeCardSendRequest
{
    /// <summary>消费卡card_id 归属appid，需与api调用方商户号有M-A绑定关系，需和创建消费卡信息中填入的归属appid一致。入参中的用户openid也需用此appid生成。 示例值：wxc0b84a53ed8e8d29（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string[1,18]</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>待发卡用户的openid，需为消费卡归属appid生成的openid。 示例值：obLatjhnqgy2syxrXVM3MJirbkdI（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string[1,28]</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>商户此次发放凭据号。推荐使用大小写字母和数字，不同添加请求发放凭据号不同，商户侧需保证同一发券请求的out_request_no和send_time的唯一性。 示例值：oTYhjfdsahnssddj_0136（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>单次请求发卡时间，消费卡在商户系统的实际发放时间，为东八区标准时间（UTC+8）。商户需保证同一次请求的out_request_no和send_time唯一。 由于系统限制，暂不支持传入早于当前时间24小时以上的时间进行发券请求。 示例值：2019-12-31T13:29:35.120+08:00（官方字段路径 <c>send_time</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</summary>
    [JsonPropertyName("send_time")]
    public string? SendTime { get; set; }

}

/// <summary>发放消费卡响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4017164666"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorConsumeCardSendResponse : WechatPayResponse
{
    /// <summary>发卡成功后，每个用户在每个card_id下会分配一个唯一code。 示例值：198555211279（官方字段路径 <c>card_code</c>，<b>必填</b>，类型 <c>string[1,15]</c>）。</summary>
    [JsonPropertyName("card_code")]
    public string? CardCode { get; set; }

}
