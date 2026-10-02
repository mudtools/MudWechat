// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;

/// <summary>
/// 获取客户群列表请求体（<c>/cgi-bin/externalcontact/groupchat/list</c>）。
/// <para>不指定 <see cref="OwnerFilter"/> 时将拉取应用可见范围内全部群主数据，
/// 可见范围人数超过 1000 人时报错 81017，此时必须指定群主过滤；
/// 当群主为离职成员时必须指定群主过滤才可拉取对应数据。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupChat")]
public class GetGroupChatListRequest
{
    /// <summary>
    /// 获取或设置客户群跟进状态过滤：0 - 所有列表（即不过滤）；1 - 离职待继承；
    /// 2 - 离职继承中；3 - 离职继承完成。默认为 0。
    /// </summary>
    [JsonPropertyName("status_filter")]
    public int? StatusFilter { get; set; }

    /// <summary>
    /// 获取或设置群主过滤条件（不填表示获取应用可见范围内全部群主的数据）。
    /// </summary>
    [JsonPropertyName("owner_filter")]
    public GroupChatOwnerFilter? OwnerFilter { get; set; }

    /// <summary>
    /// 获取或设置用于分页查询的游标，由上一次调用返回，首次调用不填。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页预期请求的数据量（官方必填；取值范围 1 ~ 1000）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
