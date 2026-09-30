// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.Models;

/// <summary>授权的应用信息（领域模型；对应官方 <c>auth_info.agent</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Authentication")]
public sealed class WechatAuthAgent
{
    /// <summary>授权方应用 id。</summary>
    public int AgentId { get; set; }

    /// <summary>授权方应用名称。</summary>
    public string? Name { get; set; }

    /// <summary>授权方应用方形头像。</summary>
    public string? SquareLogoUrl { get; set; }

    /// <summary>授权方应用圆形头像。</summary>
    public string? RoundLogoUrl { get; set; }

    /// <summary>旧多应用套件中的对应应用 id（新开发者忽略）。</summary>
    public int AppId { get; set; }

    /// <summary>授权模式：0 管理员授权；1 成员授权。</summary>
    public int AuthMode { get; set; }

    /// <summary>是否为代开发自建应用。</summary>
    public bool IsCustomizedApp { get; set; }

    /// <summary>是否由第三方应用接口唤起授权（仅特定链路返回）。</summary>
    public bool AuthFromThirdApp { get; set; }

    /// <summary>应用对应的权限。</summary>
    public WechatAuthPrivilege? Privilege { get; set; }

    /// <summary>共享了应用的企业信息（企业互联 / 上下游共享安装时返回）。</summary>
    public WechatAuthSharedFrom? SharedFrom { get; set; }
}
