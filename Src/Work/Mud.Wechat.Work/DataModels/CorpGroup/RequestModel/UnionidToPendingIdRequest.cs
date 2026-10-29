// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup;

/// <summary>
/// 上下游关联客户信息（未添加客户，单个转换）请求体（<c>/cgi-bin/corpgroup/unionid_to_pending_id</c>）。
/// </summary>
/// <remarks>
/// pending_id 主要用于关联微信 unionid 与外部联系人 external_userid（可理解为临时外部联系人 ID），
/// 有效期 90 天、共享应用内唯一。调用频率：10 万次/小时、48 万次/天、750 万次/月（按上游企业维度）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CorpGroup")]
public class UnionidToPendingIdRequest
{
    /// <summary>
    /// 获取或设置微信客户的 unionid。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string UnionId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置微信客户的 openid。
    /// </summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;
}
