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
/// 查看分享员应答（<c>POST /shop/sharer/search_sharer</c>，字段平铺于应答顶层、无对象包装）。
/// </summary>
/// <remarks>
/// <para><b><c>bind_status</c> 官方枚举</b>：<c>1</c> 已绑定、<c>2</c> 拒绝邀请、<c>3</c> 邀请中、<c>4</c> 邀请过期。
/// 参数表标 number、回包示例为带引号字符串（<c>"1"</c>）⇒ 本仓以表为准建 <c>int?</c> 并留档该冲突。</para>
/// <para><b>敏感面</b>：<c>nickname</c> 属个人敏感信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerSearchResponse : WxaResponse
{
    /// <summary>分享员邀请时间（<c>invited_time</c>，number，unix 秒）。表标 number、示例为字符串 ⇒ 以表为准。</summary>
    [JsonPropertyName("invited_time")]
    public long? InvitedTime { get; set; }

    /// <summary>分享员绑定时间（<c>bind_time</c>，number，unix 秒）。表标 number、示例为字符串 ⇒ 以表为准。</summary>
    [JsonPropertyName("bind_time")]
    public long? BindTime { get; set; }

    /// <summary>分享员昵称（<c>nickname</c>，string）。禁落日志。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>分享员绑定状态（<c>bind_status</c>，number）：<c>1</c> 已绑定 / <c>2</c> 拒绝邀请 / <c>3</c> 邀请中 / <c>4</c> 邀请过期。</summary>
    [JsonPropertyName("bind_status")]
    public int? BindStatus { get; set; }
}
