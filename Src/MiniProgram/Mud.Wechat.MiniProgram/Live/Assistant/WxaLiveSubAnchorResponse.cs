// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 获取主播副号应答（<c>GET /wxaapi/broadcast/room/getsubanchor</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getsubanchor.html"/>。
/// </para>
/// <para>
/// <b>只返回单个微信号</b>：官方原文「副号」为<b>唯一</b>一个（<c>addsubanchor</c> 的 <c>500004</c>
/// 「不能设置重复的副号」印证）⇒ 本应答不是数组，未设置副号时官方返回体不含该字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveSubAnchorResponse : WxaResponse
{
    /// <summary>主播副号微信号（<c>username</c>，官方原文说明「主播微信号」）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
