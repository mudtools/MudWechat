// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;

namespace Mud.Wechat.Work.Abstractions.Enums;

/// <summary>
/// 企业微信应用模式<b>集合</b>（<see cref="WechatAppType"/> 的位标志形态，v2.2 方案 ADR-15）。
/// </summary>
/// <remarks>
/// <para>
/// 官方开放面的真实粒度是<b>事件键</b>而非事件族：<see cref="WechatAppType"/> 只能表达「单一模式」，
/// 不足以声明「某事件对哪几种模式开放」。本集合即为契约表提供该维度
/// （<c>SupportedAppTypes = WechatAppTypeSet.All</c> 表示三类应用均开放）。
/// </para>
/// <para>
/// 与既有族级闸的关系：<c>WechatAppCallbackOptions.IsEventFamilyAllowed</c> 继续作为<b>族级默认</b>，
/// 事件键级声明在其之上叠加精度（详见方案 §3.9.3）。
/// </para>
/// </remarks>
[Flags]
public enum WechatAppTypeSet
{
    /// <summary>无（不匹配任何应用类型）。</summary>
    None = 0,

    /// <summary>企业自建应用（<see cref="WechatAppType.Internal"/>）。</summary>
    Internal = 1,

    /// <summary>第三方应用（<see cref="WechatAppType.ThirdParty"/>）。</summary>
    ThirdParty = 2,

    /// <summary>服务商代开发（<see cref="WechatAppType.Provider"/>）。</summary>
    Provider = 4,

    /// <summary>三类应用全部开放（通讯录变更族、异步任务族的官方开放面）。</summary>
    All = Internal | ThirdParty | Provider,
}
