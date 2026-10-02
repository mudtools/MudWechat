// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup;

/// <summary>
/// 获取下级/下游企业的 access_token 响应体（<c>/cgi-bin/corpgroup/corp/gettoken</c>）。
/// </summary>
/// <remarks>
/// 返回的 <see cref="AccessToken"/> 为下级/下游企业调用凭证（最长 512 字节），
/// 由调用方自行管理生命周期（SDK 令牌基座不自动缓存该凭证）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CorpGroup")]
public class GetCorpGroupTokenResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置下级/下游企业调用凭证（最长 512 字节；敏感，不得记录到日志）。
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// 获取或设置凭证的有效时间（秒）。
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
