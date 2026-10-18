// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Demo.SelfTest;
using Mud.Wechat.Demo.Shared;

// Demo 回调离线自测工具：用 SDK 自身的密码学内核构造合法报文，向正在运行的回调 Demo 推送，
// 从而在无企业微信服务器的前提下验证「验签 → 时效 → 解密 → receiveid → 抗重放 → 分发」整条链路。
//
// 用法：
//   dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel work --base-url http://localhost:56534
//   dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel mp   --base-url http://localhost:56554
//   dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel component
//   dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel channels
//   dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel bot
//
// 未给 --base-url / --route 时按通道取对应 Demo 的默认值（见 SelfTestArgs）。
//
// 退出码：0 = 全部断言通过；1 = 存在失败断言；2 = 参数或前置条件错误。

var options = SelfTestArgs.Parse(args);
if (options == null)
{
    return 2;
}

if (!options.AllowRemote && !IsLocalHost(options.BaseUrl.Host))
{
    Console.Error.WriteLine(
        $"[selftest] 目标主机 {options.BaseUrl.Host} 不是本机地址。" +
        "自测工具默认只允许 localhost / 127.0.0.1，确需推送远端请加 --allow-remote（并自行承担把构造报文发给外部系统的后果）。");
    return 2;
}

Console.WriteLine($"[selftest] 通道={options.Channel} 目标={options.BaseUrl} 路由={options.Route}");
Console.WriteLine("[selftest] 提示：同一通道请只跑一次 —— 企微/公众号/小店通道有抗重放指纹闸，重复推同一报文会被 403；开放平台通道无指纹闸，但票据与授权码各自的一次性语义仍由 Demo 台账承担。");

IReadOnlyList<SelfTestAssertion> assertions = options.Channel switch
{
    SelfTestChannel.Work => await WorkCallbackProbe.RunAsync(options).ConfigureAwait(false),
    SelfTestChannel.Mp => await MpCallbackProbe.RunAsync(options).ConfigureAwait(false),
    SelfTestChannel.Component => await ComponentPushProbe.RunAsync(options).ConfigureAwait(false),
    SelfTestChannel.Channels => await ChannelsCallbackProbe.RunAsync(options).ConfigureAwait(false),
    SelfTestChannel.Bot => await BotCallbackProbe.RunAsync(options).ConfigureAwait(false),
    _ => throw new NotSupportedException($"通道 {options.Channel} 的自测尚未实现。"),
};

var failed = 0;
foreach (var assertion in assertions)
{
    if (assertion.Passed)
    {
        Console.WriteLine($"  PASS  {assertion.Name}");
        continue;
    }

    failed++;
    Console.WriteLine($"  FAIL  {assertion.Name} —— {assertion.Detail}");
}

Console.WriteLine(failed == 0
    ? $"[selftest] 全部通过（{assertions.Count} 项断言）。"
    : $"[selftest] {failed}/{assertions.Count} 项断言失败。");

return failed == 0 ? 0 : 1;

static bool IsLocalHost(string host)
    => string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
       || string.Equals(host, "127.0.0.1", StringComparison.Ordinal)
       || string.Equals(host, "::1", StringComparison.Ordinal);
