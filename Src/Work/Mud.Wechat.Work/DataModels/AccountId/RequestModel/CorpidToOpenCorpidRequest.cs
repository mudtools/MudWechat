// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// corpid 转换请求体（<c>/cgi-bin/service/corpid_to_opencorpid</c>）：
/// 将企业主体的 corpid 转换为服务商的密文 corpid（open_corpid）。
/// </summary>
/// <remarks>
/// 官方限制：仅限第三方服务商，转换已获授权企业（或待迁移的自建应用企业）的 corpid；
/// corpid 密文大小写敏感，不能再转为纯小写或纯大写。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class CorpidToOpenCorpidRequest
{
    /// <summary>
    /// 获取或设置待获取的企业 ID（corpid，官方必填）。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }
}
