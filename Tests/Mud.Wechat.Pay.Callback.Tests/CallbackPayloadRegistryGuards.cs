// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.Wechat.Pay.DataModels.Callback;

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 回调载荷**登记守卫**：<c>Mud.Wechat.Pay.DataModels.Callback</c> 下的每个载荷类型都必须在
/// <b>手写</b>的 <see cref="WechatPayCallbackJsonContext"/> 里登记。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么这条必须被守住</b>：本包的手写上下文（为覆盖 net6.0 而手写，见其类注释）与
/// DataModels 生成的 <c>CallbackJsonContext</c> 是<b>两份</b>登记表。新增一个载荷类型时，
/// 若只登记了生成那份而漏了手写这份，<b>编译期毫无提示</b>、普通 TFM 下的单测也可能照过，
/// 而 Native AOT 下 <c>JsonSerializer.Deserialize</c> 会因缺少元数据<b>抛异常或静默给 null</b> ——
/// 属于「上线才炸、且难定位」的一类缺陷。故以守卫把两份登记表的一致性固化。
/// </para>
/// <para>
/// 本守卫<b>只查 Callback 命名空间下的类型</b>：跨域复用的子类型（如支付分域的
/// <c>PayScore*</c>）由各自域的守卫负责，不在此重复。
/// </para>
/// </remarks>
public class CallbackPayloadRegistryGuards
{
    /// <summary>回调命名空间下的载荷类型必须全部登记进手写上下文。</summary>
    [Fact]
    public void EveryCallbackPayloadType_ShouldBeRegisteredInHandwrittenContext()
    {
        var payloadTypes = typeof(WechatPayNotification).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == "Mud.Wechat.Pay.DataModels.Callback")
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 防「发现机制失效导致白名单真空」的静默空跑（AGENTS §6）。
        payloadTypes.Should().HaveCount(14,
            "回调载荷类型数：通知信封 2 + 交易族 3 + 退款族 2 + 分账族 2 + 支付分族 5；" +
            "数量变化须同批更新本守卫，并确认新类型已登记进手写上下文");

        foreach (var type in payloadTypes)
        {
            WechatPayCallbackJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(
                    $"{type.Name} 必须登记进**手写**的 WechatPayCallbackJsonContext —— " +
                    "漏登记时普通 TFM 可能正常，而 AOT 下解析会静默失效");
        }
    }

    /// <summary>
    /// 新增的支付分授权载荷确实在登记表中（锚定上一条枚举口径未失效）。
    /// </summary>
    /// <remarks>
    /// <b>为何不在此断言生成的那份上下文</b>：生成的 <c>CallbackJsonContext</c> 是 <c>internal</c>
    /// （测试工程不可见），而且它<b>不可能漂移</b> —— 脚手架按 <c>SerializerClassName</c> 分组自动收录
    /// 全部带该特性的类型。所以真正的漂移风险<b>只在手写这份</b>，但前提是<b>分组名写对</b>，
    /// 故本用例改为断言分组名（公开可查），把生成那份的正确性交给「属性正确」这一等价条件。
    /// </remarks>
    [Fact]
    public void PayScoreAuthorizationPayload_ShouldDeclareCallbackRegistryGroup()
    {
        WechatPayCallbackJsonContext.Default
            .GetTypeInfo(typeof(WechatPayPayScoreAuthorizationResource))
            .Should().NotBeNull("本包手写上下文（覆盖 net6.0）须能解析该载荷");

        typeof(WechatPayPayScoreAuthorizationResource)
            .GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
            .Should().Be("Callback",
                "分组名写错则脚手架不会把它收进生成的 CallbackJsonContext（组件序列化管线 AOT 下解析失效）");
    }
}
