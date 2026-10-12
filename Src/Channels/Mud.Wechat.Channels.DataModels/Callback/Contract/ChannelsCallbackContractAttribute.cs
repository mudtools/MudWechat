// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 微信小店回调载荷的事件键声明（契约登记<b>单一来源</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 载荷类标本特性后，契约登记生成器（中立工具 <c>Mud.Wechat.Callback.Generator</c> 的小店档位）按
/// 登记宿主签名推断档位，编译期发射 <c>ChannelsPayloadContracts.RegisterAll</c> 方法体 ——
/// 「特性声明」与「注册表内容」不会漂移（手写登记表是禁止形态）。
/// </para>
/// <para>
/// <b>同键多声明</b>：<c>AllowMultiple = true</c>，同一载荷服务于多个事件键时逐键声明；同键重复声明由生成器去重。
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ChannelsCallbackContractAttribute : Attribute
{
    /// <summary>本载荷覆盖的事件类型键集合（官方解密明文顶层 <c>Event</c> 取值，建议引用常量类）。</summary>
    public string[] EventTypes { get; set; } = Array.Empty<string>();
}
