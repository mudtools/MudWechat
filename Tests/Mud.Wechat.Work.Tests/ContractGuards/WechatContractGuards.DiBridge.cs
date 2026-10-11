// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// WechatContractGuards 的 partial 分文件（F2 守卫拆分，审查报告 v1）：断言方法逐字搬移、
/// 类名与测试 FQN 不变 —— 本文件承载 AppContextHolder_ShouldBeSameInstanceAsSwitcher_InRegistrationSource。
/// </summary>
public partial class WechatContractGuards
{
    /// <summary>
    /// 契约守卫 G8-A（P0-1）：<c>IAppContextHolder</c> 必须与 <c>IWechatAppContextSwitcher</c> 同实例，
    /// 否则声明式（<c>[Token]</c>）客户端读到的环境上下文恒为 null（多套件静默回退默认应用令牌）。
    /// </summary>
    /// <remarks>
    /// 组件 3.0.4+ 默认注册委托式适配器（<c>DelegatingAppContextHolder</c>，internal）后，注册位置不再影响
    /// 语义（行为断言见 <c>WechatServiceCollectionExtensionsTests</c> 的正向/逆序用例）；本守卫改为锁定
    /// <b>桥接注册形态</b>：holder 桥接必须为「无条件注册（最后注册者胜）」——TryAdd 形态在「组件先注册」
    /// 场景下会静默失效并回退适配器内部 switcher（两个 AsyncLocal），故形态本身是契约。
    /// </remarks>
    [Fact]
    public void AppContextHolder_ShouldBeSameInstanceAsSwitcher_InRegistrationSource()
    {
        var extensionsPath = SourcePath(
            "Mud.Wechat.Work.Abstractions", "Extensions", "WechatWorkMultiAppExtensions.cs");
        File.Exists(extensionsPath).Should().BeTrue();

        var source = File.ReadAllText(extensionsPath);

        source.Should().Contain("TryAddSingleton<IWechatAppContextSwitcher", "必须注册 IWechatAppContextSwitcher");
        source.Should().Contain("services.AddMudHttpClient(", "per-app 命名客户端在本扩展内注册");

        source.Should().Contain("AddSingleton<IAppContextHolder>(sp => sp.GetRequiredService<IWechatAppContextSwitcher>())",
            "G8-A：IAppContextHolder 必须委托到同一个切换器实例，且为无条件注册（最后注册者胜）——" +
            "TryAdd 形态在组件先注册时静默失效（适配器回退内部 switcher，P0-1 复现）");
        source.Should().NotContain("TryAddSingleton<IAppContextHolder>",
            "G8-A：holder 桥接不得为 TryAdd（组件 3.0.4+ 委托式适配器在场时 TryAdd 会静默失效）");
    }
}
