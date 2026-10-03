// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 事件键级开放面声明单元：「允许的应用模式集合 × 要求的回调通道」的<b>组合对</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何是组合对而非两个独立维度</b>：部分官方事件族的开放面矩阵无法用「单一通道 + 单一模式集合」表达 ——
/// 如客户联系/获客族（官方 92130/92277/96361/97299/98958）：企业自建与服务商代开发经<b>应用数据通道</b>、
/// 第三方应用经<b>套件指令通道</b>（指令回调 URL）。一个事件键需要同时声明两组
/// （模式集合, 通道）组合，任一组命中即许可分发（见 <c>WechatPayloadContract.IsOpenFor</c>）。
/// </para>
/// </remarks>
public readonly struct WechatOpenSurface
{
    /// <summary>创建开放面声明单元。</summary>
    /// <param name="supportedAppTypes">允许的应用模式集合（不得为 <see cref="WechatAppTypeSet.None"/>）。</param>
    /// <param name="requiredChannel">要求的回调通道。</param>
    public WechatOpenSurface(WechatAppTypeSet supportedAppTypes, WechatCallbackChannel requiredChannel)
    {
        SupportedAppTypes = supportedAppTypes;
        RequiredChannel = requiredChannel;
    }

    /// <summary>本组合对允许的应用模式集合。</summary>
    public WechatAppTypeSet SupportedAppTypes { get; }

    /// <summary>本组合对要求的回调通道。</summary>
    public WechatCallbackChannel RequiredChannel { get; }

    /// <inheritdoc />
    public override string ToString() => SupportedAppTypes + "@" + RequiredChannel;
}
