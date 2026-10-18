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
/// 已绑定分享员条目（<c>sharers[]</c> 元素，<c>get_sharer_list</c> 应答）。
/// </summary>
/// <remarks>
/// <para><b>与 <see cref="WxaShopSharerSearchResponse"/> 不共用类型</b>：单查侧多 <c>bind_status</c>、
/// 列表侧无该字段（官方逐页参数表差异），且本条带包装层级 ⇒ 两套独立建模。</para>
/// <para><b>敏感面</b>：<c>openid</c> 与 <c>nickname</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerInfo
{
    /// <summary>分享员 openid（<c>openid</c>，string）。禁落日志。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>分享员邀请时间（<c>invited_time</c>，number，unix 秒）。表标 number、示例为字符串 ⇒ 以表为准。</summary>
    [JsonPropertyName("invited_time")]
    public long? InvitedTime { get; set; }

    /// <summary>分享员绑定时间（<c>bind_time</c>，number，unix 秒）。表标 number、示例为字符串 ⇒ 以表为准。</summary>
    [JsonPropertyName("bind_time")]
    public long? BindTime { get; set; }

    /// <summary>分享员昵称（<c>nickname</c>，string）。禁落日志。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}
