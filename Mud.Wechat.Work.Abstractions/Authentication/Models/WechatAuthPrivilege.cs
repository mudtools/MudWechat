// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.Models;

/// <summary>应用权限信息（领域模型；对应官方 <c>privilege</c>）。</summary>
public sealed class WechatAuthPrivilege
{
    /// <summary>权限等级：1 通讯录基本信息只读；2 通讯录全部信息只读；3 通讯录全部信息读写；4 单个基本信息只读；5 通讯录全部信息只写。</summary>
    public int Level { get; set; }

    /// <summary>应用可见范围（部门）。</summary>
    public IList<int> AllowParty { get; set; } = new List<int>();

    /// <summary>应用可见范围（成员）。</summary>
    public IList<string> AllowUser { get; set; } = new List<string>();

    /// <summary>应用可见范围（标签）。</summary>
    public IList<int> AllowTag { get; set; } = new List<int>();

    /// <summary>额外通讯录（部门）。</summary>
    public IList<int> ExtraParty { get; set; } = new List<int>();

    /// <summary>额外通讯录（成员）。</summary>
    public IList<string> ExtraUser { get; set; } = new List<string>();

    /// <summary>额外通讯录（标签）。</summary>
    public IList<int> ExtraTag { get; set; } = new List<int>();
}
