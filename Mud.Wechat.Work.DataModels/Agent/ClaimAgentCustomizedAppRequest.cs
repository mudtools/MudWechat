// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 自建应用迁移成代开发应用请求体（<c>/cgi-bin/agent/claim_customized_app</c>；
/// 待迁移自建应用的 access_token 为官方 URL 参数、经 Query 注入自动携带，不在请求体中）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：suite_access_token 为<b>包体参数</b>——代开发应用模板接口调用凭证
/// （获取方式与第三方应用凭证相同，依赖 suite_ticket 推送机制），
/// 不经由令牌作用域机制表达，由调用方显式传入。
/// </para>
/// <para>安全约束：suite_access_token 为套件级敏感凭证，不得记录日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class ClaimAgentCustomizedAppRequest
{
    /// <summary>获取或设置代开发应用模板接口调用凭证（官方必填的包体参数，获取方式与第三方应用凭证相同）。</summary>
    [JsonPropertyName("suite_access_token")]
    public string SuiteAccessToken { get; set; } = string.Empty;
}
