// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Basic;

/// <summary>
/// 网络通信检测请求体（<c>POST /cgi-bin/callback/check</c>）。
/// </summary>
/// <remarks>
/// 两个字段官方均为<b>必填</b>；取值常量见 <c>MpCallbackCheckActions</c> / <c>MpCallbackCheckOperators</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class MpCallbackCheckRequest
{
    /// <summary>检测动作：<c>dns</c>（域名解析）/ <c>ping</c>（ping 检测）/ <c>all</c>（全部）。</summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    /// <summary>检测运营商：<c>CHINANET</c>（电信）/ <c>UNICOM</c>（联通）/ <c>CAP</c>（腾讯）/ <c>DEFAULT</c>（自动）。</summary>
    [JsonPropertyName("check_operator")]
    public string CheckOperator { get; set; } = string.Empty;
}
