// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contracts.Users;

/// <summary>
/// 成员扩展属性（<c>extattr</c>，详见官方「成员扩展属性」文档）。
/// </summary>
/// <remarks>新增/更新仅通讯录同步助手或第三方通讯录应用可进行；属性须先在企业 WEB 管理端添加，否则赋值被忽略。</remarks>
public class UserExtAttr
{
    /// <summary>
    /// 获取或设置属性列表。
    /// </summary>
    [JsonPropertyName("attrs")]
    public List<UserExtAttrItem>? Attrs { get; set; }
}
