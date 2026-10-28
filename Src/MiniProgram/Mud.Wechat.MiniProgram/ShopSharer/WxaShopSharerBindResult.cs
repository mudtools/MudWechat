// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 邀请分享员逐人结果（<c>result_list[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b><c>reason_code</c> 官方枚举</b>：<c>0</c> 邀请成功；<c>100</c> 系统繁忙可重试；<c>200</c> 小程序绑定
/// 分享员达上限（10000 个）；<c>201</c> 分享员 openid 错误；<c>202</c> 已绑定为分享员；<c>203</c> 用户拒绝再次邀请；
/// <c>204</c> 邀请过于频繁（一天最多三次、10 分钟内最多一次）。</para>
/// <para><b>敏感面</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerBindResult
{
    /// <summary>邀请的分享员 openid（<c>openid</c>，string）。禁落日志。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>邀请结果（<c>result_code</c>，number）：<c>0</c> 成功、<c>1</c> 失败（可重试）、<c>2</c> 拒绝（不可重试）。</summary>
    [JsonPropertyName("result_code")]
    public int? ResultCode { get; set; }

    /// <summary>邀请分享员错误码（<c>reason_code</c>，number），取值见类型 remarks 的官方枚举。</summary>
    [JsonPropertyName("reason_code")]
    public int? ReasonCode { get; set; }
}
