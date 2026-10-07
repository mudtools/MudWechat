// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 公众号回调载荷的事件键声明（契约登记<b>单一来源</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 载荷类标本特性后，契约登记生成器（中立工具 <c>Mud.Wechat.Callback.Generator</c>）按
/// <b>登记宿主签名推断的档位</b>发射 <c>MpPayloadContracts.RegisterAll</c> 方法体，
/// 故「特性声明」与「注册表内容」不会漂移（手写登记表是禁止形态）。
/// </para>
/// <para>
/// <b>为何公众号侧特性比企微侧薄</b>：企微特性还承载「族前置 + 应用模式集合 + 回调通道」开放面
/// （依赖企微枚举，按 AB-G1/X11 不得下沉叶层）；公众号无对应语义，故只保留事件键集合。
/// </para>
/// <para>
/// <b>同键多声明</b>：<c>AllowMultiple = true</c>，同一载荷服务于多个事件键时逐键声明；同键重复声明由生成器去重。
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MpCallbackContractAttribute : Attribute
{
    /// <summary>本载荷覆盖的事件类型键集合（官方 <c>MsgType</c>/<c>Event</c> 取值，建议引用常量类）。</summary>
    public string[] EventTypes { get; set; } = Array.Empty<string>();
}

/// <summary>
/// 载荷契约登记宿主（生成器的发射目标；方法体由生成器补齐）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何是本形态</b>：生成器无法在编译期「凭空知道」要往哪个类型发射方法体，
/// 故由产品线显式声明 <c>partial</c> 宿主 —— 生成器扫描本类型并按<b>其 <c>RegisterAll</c> 签名中的
/// 注册表接口类型</b>推断产品线档位（<c>IMpPayloadContractRegistry</c> → 公众号档位）。
/// </para>
/// <para>
/// <b>防静默空跑</b>：生成器若未找到任何 <c>[MpCallbackContract]</c> 声明，将发出诊断（编译期可见），
/// 不会静默产出空方法体；运行期另有守卫断言 <c>RegisteredKeys</c> 下限。
/// </para>
/// </remarks>
public static partial class MpPayloadContracts
{
    /// <summary>登记全部公众号回调载荷契约（调用点：<c>AddMpCallbackCore</c> 组合根期）。</summary>
    /// <param name="registry">契约注册表。</param>
    public static partial void RegisterAll(IMpPayloadContractRegistry registry);
}
