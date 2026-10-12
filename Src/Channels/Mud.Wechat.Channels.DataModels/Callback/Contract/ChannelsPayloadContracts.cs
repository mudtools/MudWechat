// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 载荷契约登记宿主（生成器的发射目标；方法体由生成器补齐）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何宿主落 DataModels</b>：载荷 DTO 与 <c>[ChannelsCallbackContract]</c> 声明都在本程序集，
/// 生成器「只挂载荷工程」（对齐企微线先例）；跨程序集发射 partial 实现会产出 CS0759
/// （实现与定义必须同程序集）。
/// </para>
/// <para>
/// 生成器扫描本类型并按其 <c>RegisterAll</c> 签名中的注册表接口类型
/// （<c>IChannelsPayloadContractRegistry</c>）推断小店档位；生成器若未找到任何
/// <c>[ChannelsCallbackContract]</c> 声明将发出编译期诊断（MUDCB106），不会静默产出空方法体。
/// </para>
/// </remarks>
public static partial class ChannelsPayloadContracts
{
    /// <summary>登记全部小店回调载荷契约（调用点：组合根期 <c>AddWechatChannelsCallback</c>）。</summary>
    /// <param name="registry">契约注册表。</param>
    public static partial void RegisterAll(IChannelsPayloadContractRegistry registry);
}
