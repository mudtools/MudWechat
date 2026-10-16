// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandCardMember;

/// <summary>用户动态信息创建请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336987"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserFeedCreateRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商家请求单号】 商家创建用户动态信息凭据号。商家自定义，注意保持唯一性，仅供参考的格式：品牌ID+时间戳+流水号。字符仅允许包含英文半角的数字、字母、连接线-和下划线_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【会员动态内容】 与 下单同步用户实时动态 文档中的 `cell` 参数保持一致（官方字段路径 <c>cell</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("cell")]
    public string? Cell { get; set; }

}

/// <summary>用户动态信息创建响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336987"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberUserFeedCreateResponse : WechatPayResponse
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code（官方字段路径 <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_card_code")]
    public string? UserCardCode { get; set; }

    /// <summary>【商家请求单号】 商家创建用户动态信息凭据号。商家自定义，注意保持唯一性，仅供参考的格式：品牌ID+时间戳+流水号。字符仅允许包含英文半角的数字、字母、连接线-和下划线_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【会员动态内容】 与 下单同步用户实时动态 文档中的 `cell` 参数保持一致（官方字段路径 <c>cell</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("cell")]
    public string? Cell { get; set; }

}
