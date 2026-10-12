// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>获取插件用户 <c>openpid</c> 应答（<c>GET /wxa/getpluginopenpid</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>user-info/basic-info/api_getpluginopenpid.html</c>。</para>
/// <para>
/// <b>前置约束（官方原文）</b>：须先经小程序端 <c>wx.pluginLogin</c> 取得插件用户标志凭证
/// <c>code</c> 再传至开发者服务器；<c>plugin_appid</c> 为插件 AppID。
/// 返回的 <c>openpid</c> 是<b>插件维度</b>的用户唯一标识（同用户在不同插件下 openpid 不同）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaGetPluginOpenPidResponse : WxaResponse
{
    /// <summary>插件用户唯一标识（<c>openpid</c>）。</summary>
    [JsonPropertyName("openpid")]
    public string? OpenPid { get; set; }
}
