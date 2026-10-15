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

/// <summary>会员卡预授权令牌发起请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336986"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberPreAuthTokenRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方字段路径 <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

}

/// <summary>会员卡预授权令牌发起响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336986"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandCardMember")]
public class BrandMemberPreAuthTokenResponse : WechatPayResponse
{
    /// <summary>【预授权 token】 用于H5 和小程序场景拉起会员开卡组件投放会员卡（官方字段路径 <c>token</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>【token 过期时间】 token过期时间（生成后一小时有效），需遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE。yyyy-MM-DD 表示年月日；T 字符用于分隔日期和时间部分；HH:mm:ss 表示具体的时分秒；TIMEZONE 表示时区（例如，+08:00 对应东八区时间，即北京时间）。示例：2015-05-20T13:29:35+08:00 表示北京时间2015年5月20日13点29分35秒（官方字段路径 <c>expire_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

}
