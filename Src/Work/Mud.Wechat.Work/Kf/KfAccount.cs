// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 客服账号信息（<c>/cgi-bin/kf/account/list</c> 的 <c>account_list</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfAccount
{
    /// <summary>
    /// 获取或设置客服账号 ID。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置客服名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置客服头像 URL。
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 获取或设置当前应用是否有该客服账号的管理权限（编辑信息、分配会话、收发消息）。
    /// <para>
    /// 微信客服组件应用调用「获取客服账号列表」时不返回此字段。
    /// </para>
    /// </summary>
    [JsonPropertyName("manage_privilege")]
    public bool? ManagePrivilege { get; set; }
}
