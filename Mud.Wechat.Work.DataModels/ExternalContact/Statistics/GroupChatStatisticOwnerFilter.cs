// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Statistics;

/// <summary>
/// 群聊数据统计的群主过滤（<c>owner_filter</c>）。
/// <para>不填表示取应用可见范围内全部群主的数据（可见范围人数超过 1000 人会报错 81017）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Statistics")]
public class GroupChatStatisticOwnerFilter
{
    /// <summary>
    /// 获取或设置群主（成员）的 userid 列表（最多 100 个）。
    /// </summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UseridList { get; set; }
}
