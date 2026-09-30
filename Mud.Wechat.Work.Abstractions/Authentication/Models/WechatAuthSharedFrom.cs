// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.Models;

/// <summary>共享应用的企业信息（领域模型；对应官方 <c>shared_from</c>）。</summary>
public sealed class WechatAuthSharedFrom
{
    /// <summary>共享了应用的企业 CorpId。</summary>
    public string? CorpId { get; set; }

    /// <summary>共享途径：0 企业互联；1 上下游。</summary>
    public int ShareType { get; set; }
}
