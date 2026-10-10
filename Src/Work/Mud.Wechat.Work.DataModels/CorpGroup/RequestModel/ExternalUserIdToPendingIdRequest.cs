// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.CorpGroup;

/// <summary>
/// 上下游关联客户信息（未添加客户，批量转换）请求体（<c>/cgi-bin/corpgroup/batch/external_userid_to_pending_id</c>）。
/// </summary>
/// <remarks>
/// 上游应用须已调用过 unionid 转 pending_id 接口；该客户的跟进人或其所在客户群群主必须在应用的可见范围之内。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CorpGroup")]
public class ExternalUserIdToPendingIdRequest
{
    /// <summary>
    /// 获取或设置上游或下游企业外部联系人 ID 列表（最多同时查询 100 个）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public List<string>? ExternalUserIds { get; set; }

    /// <summary>
    /// 获取或设置群 id（传入时只检查群主是否在可见范围，同时忽略该群以外的 external_userid；
    /// 不传则只检查客户跟进人是否在可见范围内。不填时不出网）。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }
}
