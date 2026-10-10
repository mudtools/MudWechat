// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;

namespace Mud.Wechat.Channels.Callback.Tests;

/// <summary>
/// 小店回调包脚手架守卫（P0/P3 期占位：P3 起按设计文档 v1 §3.5 / §5.1 逐项补全回调契约守卫）。
/// </summary>
/// <remarks>
/// <para>
/// 本类在 P0 期只锁定<b>回调包的依赖隔离</b>与<b>程序集可加载</b>两项硬约束（对齐
/// <c>ChannelsScaffoldContractGuards.CallbackPackage_ShouldBeDedicatedAndNotBridgeExistingChannels</c>
/// 的运行时程序集侧复核）；P3 期随 <c>ChannelsCallbackContractGuards</c> 全量落地后被替换/扩充。
/// </para>
/// <para>
/// 至少 1 个有效测试是 verify-build.ps1 步骤 3 的门禁（total &gt; 0 才算执行），
/// 故本占位守卫保证回调测试工程在 P0/P1 期即可通过门禁（P3 前不出现「无测试」盲区）。
/// </para>
/// </remarks>
public class ChannelsCallbackScaffoldGuards
{
    /// <summary>
    /// 回调程序集可加载且名称正确（防 slnx 漏配 / 程序集改名后测试静默空跑）。
    /// </summary>
    /// <remarks>
    /// <b>为何显式 <c>Assembly.Load</c></b>：P0 期 <c>Mud.Wechat.Channels.Callback</c> 是<b>空包</b>
    /// （P3 才落处理器 / 中间件类型）。空库被测试引用但<b>无任何类型被触碰</b>时，运行时不加载该程序集，
    /// <c>AppDomain.CurrentDomain.GetAssemblies()</c> 找不到 ⇒ 测试假红。故本测试<b>显式按名加载</b>，
    /// 不依赖隐式装载。
    /// </remarks>
    [Fact]
    public void CallbackAssembly_ShouldLoad_WithExpectedName()
    {
        var asm = LoadCallbackAssembly();
        asm.GetName().Name.Should().Be("Mud.Wechat.Channels.Callback");
    }

    /// <summary>
    /// 回调程序集不得引用既有产品线回调程序集（防「顺手复用公众号/企微回调通道」绕过
    /// <c>ChannelsScaffoldContractGuards</c> 的 csproj 文本断言）。
    /// </summary>
    [Fact]
    public void CallbackAssembly_ShouldNotReferenceExistingCallbackAssemblies()
    {
        var asm = LoadCallbackAssembly();

        asm.GetReferencedAssemblies()
            .Select(static a => a.Name)
            .Should().NotContain(
                new[]
                {
                    "Mud.Wechat.OfficialAccount.Callback",
                    "Mud.Wechat.Work.Callback",
                    "Mud.Wechat.Pay.Callback",
                },
                "小店回调运行时程序集不得引用任何既有线的回调程序集（设计方案 v1 §3.1 依赖硬边界）");
    }

    /// <summary>按名显式加载小店回调程序集（空包时运行时不隐式装载，须显式 Load）。</summary>
    private static Assembly LoadCallbackAssembly()
    {
        var asm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(static a => a.GetName().Name == "Mud.Wechat.Channels.Callback");
        if (asm is not null)
        {
            return asm;
        }

        asm = Assembly.Load(new AssemblyName("Mud.Wechat.Channels.Callback"));
        return asm;
    }
}
