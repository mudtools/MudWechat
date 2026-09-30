// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpTokenAuthentication;

/// <summary>
/// 获取授权企业的 access_token 请求体（get_corp_token）。
/// </summary>
public class GetCorpTokenRequest
{
    /// <summary>
    /// 获取或设置授权方（企业）的 CorpId。
    /// </summary>
    [JsonPropertyName("auth_corpid")]
    public string AuthCorpId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置永久授权码（通过 get_permanent_code 接口获取）。
    /// </summary>
    [JsonPropertyName("permanent_code")]
    public string PermanentCode { get; set; } = string.Empty;
}
