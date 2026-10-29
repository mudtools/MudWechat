// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Basic;

/// <summary>查询 / 重置指定 API 调用次数的请求体（<c>openapi/quota/get</c> 与 <c>openapi/quota/clear</c> 请求体字段集一致 ⇒ 共用）。</summary>
/// <remarks>
/// 官方契约：<c>cgi_path</c> 为 api 的请求地址——官方原文「不要前缀 https://api.weixin.qq.com，
/// 也不要漏了 /，否则都会 76003 的报错」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsCgiPathRequest
{
    /// <summary>获取或设置 api 的请求地址（官方 <c>cgi_path</c>，必填；以 / 开头、不带 https://api.weixin.qq.com 前缀）。</summary>
    [JsonPropertyName("cgi_path")]
    public string CgiPath { get; set; } = string.Empty;
}
