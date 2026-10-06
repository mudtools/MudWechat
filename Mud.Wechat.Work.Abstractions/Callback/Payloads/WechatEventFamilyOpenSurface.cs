// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 官方「事件族 → 开放面」默认矩阵（v2.2 ADR-15 的注册期校验基线）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要它</b>：事件键级开放面声明（<see cref="WechatPayloadContract.OpenSurfaces"/>）
/// 由<b>宿主或本 SDK 自行书写</b>。若没有基线比对，宿主可以把「仅自建」的 <c>change_chain</c>
/// 误声明为 <see cref="WechatAppTypeSet.All"/> ⇒ 事件键闸失效 ⇒ <b>开放面被绕过</b>
/// （这正是族级闸 <c>Unknown → return true</c> 缺口的同构风险，只是换到了声明面）。
/// </para>
/// <para>
/// <b>基线来源</b>：与 <c>WechatAppCallbackOptions.IsEventFamilyAllowed</c>（族级闸）的官方矩阵一致，
/// 仅为<b>静态只读基线</b>，不含任何租户/应用配置 ⇒ 可在类型初始化期安全使用。
/// </para>
/// <para>
/// <b>组合对形态</b>：一个族的官方开放面是「（模式集合, 通道）」的<b>组合对集合</b> ——
/// 客户联系/获客族为自建·代开发×应用通道 + 第三方×套件通道两组；其余族为单组。
/// 校验语义：声明的每个组合对必须被某个官方组合对<b>覆盖</b>（模式集合为官方对应通道组合的子集且通道一致）。
/// </para>
/// <para>
/// <b>为何 <c>Unknown</c> 族无基线</b>：官方未文档化的事件键（含宿主私有事件）没有可比对的官方结论，
/// 此时声明完全由宿主负责 —— 这是有意的边界（方案 §3.9.3）：**校验只防「声明宽于官方」，不防「官方未文档化」**。
/// </para>
/// </remarks>
internal static class WechatEventFamilyOpenSurface
{
    /// <summary>取事件族的官方开放面默认值（组合对集合）。</summary>
    /// <param name="family">事件族。</param>
    /// <param name="defaultSurfaces">该族官方默认的「（模式集合, 通道）」组合对集合。</param>
    /// <returns>是否有官方基线（<see cref="WechatCallbackEventFamily.Unknown"/> 无基线）。</returns>
    internal static bool TryGetDefault(
        WechatCallbackEventFamily family,
        out WechatOpenSurface[] defaultSurfaces)
    {
        switch (family)
        {
            // 通讯录变更族 / 异步任务族：三类应用 + 应用数据通道（官方 90967~90973 / 95797）。
            case WechatCallbackEventFamily.ContactChange:
            case WechatCallbackEventFamily.BatchJob:
                defaultSurfaces = new[]
                {
                    new WechatOpenSurface(WechatAppTypeSet.All, WechatCallbackChannel.App),
                };
                return true;

            // 上下游变更族：仅企业自建 + 应用数据通道（官方 95796）。
            case WechatCallbackEventFamily.ChainChange:
                defaultSurfaces = new[]
                {
                    new WechatOpenSurface(WechatAppTypeSet.Internal, WechatCallbackChannel.App),
                };
                return true;

            // 安全事件族（域名IP变更等）：仅企业自建 + 应用数据通道
            // （官方 100080：配置到「我的企业-设置-域名IP-可调用API的应用」；第三方/代开发暂不支持）。
            case WechatCallbackEventFamily.SecurityChange:
                defaultSurfaces = new[]
                {
                    new WechatOpenSurface(WechatAppTypeSet.Internal, WechatCallbackChannel.App),
                };
                return true;

            // 微信客服族：三类应用 + 应用数据通道
            // （官方 94670/94699/96426、97712/97302/97713：三模式报文同构，自建配置到「微信客服-可调用接口的应用」，
            // 第三方/代开发需「微信客服→管理账号、分配会话和收发消息」权限）。
            case WechatCallbackEventFamily.KfEvent:
                defaultSurfaces = new[]
                {
                    new WechatOpenSurface(WechatAppTypeSet.All, WechatCallbackChannel.App),
                };
                return true;

            // 授权族：第三方 / 代开发 + 套件指令通道（官方 99487/100964/90628）。
            // 注：授权族**不登记载荷契约**（走信封，ADR-8）；此处保留仅为矩阵完整性。
            case WechatCallbackEventFamily.Authorization:
                defaultSurfaces = new[]
                {
                    new WechatOpenSurface(
                        WechatAppTypeSet.ThirdParty | WechatAppTypeSet.Provider, WechatCallbackChannel.Suite),
                };
                return true;

            // 客户联系变更族 / 获客助手族：自建·代开发 × 应用数据通道（Event 信封，92130/96361/97299/98958）
            // + 第三方 × 套件指令通道（指令回调 URL，InfoType 信封，92277/97402/99485）。
            case WechatCallbackEventFamily.ExternalContactChange:
            case WechatCallbackEventFamily.CustomerAcquisition:
                defaultSurfaces = new[]
                {
                    new WechatOpenSurface(
                        WechatAppTypeSet.Internal | WechatAppTypeSet.Provider, WechatCallbackChannel.App),
                    new WechatOpenSurface(WechatAppTypeSet.ThirdParty, WechatCallbackChannel.Suite),
                };
                return true;

            // 协议外 / 官方未文档化：无官方基线，不做比对。
            default:
                defaultSurfaces = System.Array.Empty<WechatOpenSurface>();
                return false;
        }
    }

    /// <summary>
    /// 校验显式声明<b>不得宽于</b>官方族默认（fail-fast，组合根期抛出）。
    /// </summary>
    /// <param name="eventTypeKey">事件键（用于异常文案）。</param>
    /// <param name="family">族前置条件；<see langword="null"/> 表示未声明族 ⇒ 无基线，跳过校验。</param>
    /// <param name="declaredSurfaces">声明的开放面组合对集合（每对都必须被官方基线覆盖）。</param>
    /// <exception cref="ArgumentException">声明宽于官方族默认。</exception>
    internal static void ValidateNotWiderThanFamilyDefault(
        string eventTypeKey,
        WechatCallbackEventFamily? family,
        WechatOpenSurface[] declaredSurfaces)
    {
        if (family == null)
            return;

        if (!TryGetDefault(family.Value, out var defaultSurfaces))
            return;

        foreach (var surface in declaredSurfaces)
        {
            // ① 应用模式不得放宽（多出的位即越权）；② 通道必须与官方某组合对一致。
            // 找不到「通道一致」的官方组合对 ⇒ 通道声明本身与官方接入方式矛盾。
            WechatOpenSurface? matched = null;
            foreach (var candidate in defaultSurfaces)
            {
                if (candidate.RequiredChannel == surface.RequiredChannel)
                {
                    matched = candidate;
                    break;
                }
            }

            if (matched == null)
            {
                throw new ArgumentException(
                    "事件键 " + eventTypeKey + " 声明的通道 " + surface.RequiredChannel +
                    " 与事件族 " + family.Value + " 的官方默认通道（" + Describe(defaultSurfaces) +
                    "）不一致。", nameof(declaredSurfaces));
            }

            var widened = surface.SupportedAppTypes & ~matched.Value.SupportedAppTypes;
            if (widened != WechatAppTypeSet.None)
            {
                throw new ArgumentException(
                    "事件键 " + eventTypeKey + " 在通道 " + surface.RequiredChannel + " 上声明的开放面 " +
                    surface.SupportedAppTypes + " 宽于事件族 " + family.Value + " 的官方默认 " +
                    matched.Value.SupportedAppTypes + "（多出 " + widened + "）；官方开放面是**硬约束**，" +
                    "宿主不得放宽。若官方确有变更，请先更新官方契约表与其守卫。",
                    nameof(declaredSurfaces));
            }
        }
    }

    /// <summary>组合对集合的异常文案描述。</summary>
    private static string Describe(WechatOpenSurface[] surfaces)
    {
        var parts = new string[surfaces.Length];
        for (var i = 0; i < surfaces.Length; i++)
        {
            parts[i] = surfaces[i].ToString();
        }

        return string.Join(" / ", parts);
    }
}
