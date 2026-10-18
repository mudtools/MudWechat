// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Demo.SelfTest;

/// <summary>自测目标通道（各线回调报文的信封与验签形态不同）。</summary>
public enum SelfTestChannel
{
    /// <summary>企业微信 XML 回调（加密 XML + 四参 <c>msg_signature</c>，receiveid = CorpId / SuiteId）。</summary>
    Work,

    /// <summary>公众号 XML 回调（GET 三参 <c>signature</c> + echostr 原文回显，POST 四参 <c>msg_signature</c>，receiveid = AppId）。</summary>
    Mp,

    /// <summary>开放平台「授权事件接收 URL」（加密 XML + 四参 <c>msg_signature</c>，receiveid = 第三方平台 AppId，处置以响应体 <c>success</c>/<c>fail</c> 表达）。</summary>
    Component,

    /// <summary>微信小店 JSON 回调（加密 <b>JSON</b> 信封 <c>{"ToUserName","Encrypt"}</c>、官方强制安全模式，receiveid = 小店 AppID，成功应答恒为明文 <c>success</c>）。</summary>
    Channels,

    /// <summary>
    /// 企业微信<b>智能机器人</b> JSON 回调（信封 <c>{"encrypt"}</c>、明文 <c>receiveid</c> 恒为空串、
    /// 应答为<b>加密外壳</b> <c>{encrypt, msgsignature, timestamp, nonce}</c> 而非明文 <c>success</c>）。
    /// </summary>
    Bot,
}

/// <summary>一次自测运行的目标参数。</summary>
public sealed record SelfTestOptions
{
    public required SelfTestChannel Channel { get; init; }

    public required Uri BaseUrl { get; init; }

    /// <summary>回调路由（不含查询串），如 <c>/wechat/default</c>。</summary>
    public required string Route { get; init; }

    /// <summary>是否允许向非本机主机推送构造报文。</summary>
    public bool AllowRemote { get; init; }

    /// <summary>
    /// <c>/healthz</c> 身份片段（仅 bot / channels 两通道使用）。
    /// 留空 = 用本通道专属 Demo 的默认标识；指向<b>多线共存宿主</b>时须显式给出该宿主的标识，
    /// 否则这条断言会把「端口撞车打到别家 Demo」误报成「共存宿主不是本线 Demo」。
    /// </summary>
    public string? HealthFragment { get; init; }
}

/// <summary>单条断言结果。</summary>
/// <param name="Name">断言名称。</param>
/// <param name="Passed">是否通过。</param>
/// <param name="Detail">失败详情（含实际状态码 / 期望值）。</param>
public sealed record SelfTestAssertion(string Name, bool Passed, string Detail = "");

/// <summary>命令行解析：<c>--channel</c> / <c>--base-url</c> / <c>--route</c> / <c>--allow-remote</c>。</summary>
public static class SelfTestArgs
{
    /// <summary>解析参数；非法输入打印原因并返回 <see langword="null"/>（调用方给退出码 2）。</summary>
    public static SelfTestOptions? Parse(string[] args)
    {
        var channel = SelfTestChannel.Work;
        Uri? baseUrl = null;
        string? route = null;
        var allowRemote = false;
        string? healthFragmentValue = null;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "--channel":
                    if (!TryReadValue(args, ref i, out var channelValue))
                    {
                        return null;
                    }

                    if (!Enum.TryParse<SelfTestChannel>(channelValue, ignoreCase: true, out channel) ||
                        !string.Equals(channel.ToString(), channelValue, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Error.WriteLine($"[selftest] 未知通道 {channelValue}；当前支持：work、mp、component、channels、bot。");
                        return null;
                    }

                    break;
                case "--base-url":
                    if (!TryReadValue(args, ref i, out var urlValue) ||
                        !Uri.TryCreate(urlValue, UriKind.Absolute, out var parsedUrl) ||
                        (parsedUrl.Scheme != Uri.UriSchemeHttp && parsedUrl.Scheme != Uri.UriSchemeHttps))
                    {
                        Console.Error.WriteLine("[selftest] --base-url 需要一个 http/https 绝对地址。");
                        return null;
                    }

                    baseUrl = parsedUrl;
                    break;
                case "--route":
                    if (!TryReadValue(args, ref i, out route))
                    {
                        return null;
                    }

                    break;
                case "--allow-remote":
                    allowRemote = true;
                    break;
                case "--health-fragment":
                    if (!TryReadValue(args, ref i, out var healthFragment))
                    {
                        return null;
                    }

                    healthFragmentValue = healthFragment;
                    break;
                default:
                    Console.Error.WriteLine($"[selftest] 未知参数 {arg}；支持 --channel/--base-url/--route/--allow-remote/--health-fragment。");
                    return null;
            }
        }

        return new SelfTestOptions
        {
            Channel = channel,
            BaseUrl = baseUrl ?? DefaultBaseUrl(channel),
            Route = route ?? DefaultRoute(channel),
            AllowRemote = allowRemote,
            HealthFragment = healthFragmentValue,
        };
    }

    private static bool TryReadValue(string[] args, ref int index, out string value)
    {
        if (index + 1 >= args.Length)
        {
            Console.Error.WriteLine($"[selftest] 参数 {args[index]} 缺少取值。");
            value = string.Empty;
            return false;
        }

        index++;
        value = args[index];
        return true;
    }

    /// <summary>各通道对应 Demo 的默认监听地址（取各自 launchSettings 的 http 档，跑探针时可被 --base-url 覆盖）。</summary>
    private static Uri DefaultBaseUrl(SelfTestChannel channel) => channel switch
    {
        SelfTestChannel.Work => new Uri("http://localhost:56534", UriKind.Absolute),
        SelfTestChannel.Mp => new Uri("http://localhost:56554", UriKind.Absolute),
        SelfTestChannel.Component => new Uri("http://localhost:56560", UriKind.Absolute),
        SelfTestChannel.Channels => new Uri("http://localhost:56562", UriKind.Absolute),
        SelfTestChannel.Bot => new Uri("http://localhost:56564", UriKind.Absolute),
        _ => throw new NotSupportedException($"通道 {channel} 缺少默认地址配置。"),
    };

    private static string DefaultRoute(SelfTestChannel channel) => channel switch
    {
        SelfTestChannel.Work => "/wechat/default",
        SelfTestChannel.Mp => "/mp/default",
        SelfTestChannel.Component => "/wechat/component",
        SelfTestChannel.Channels => "/channels/default",
        SelfTestChannel.Bot => "/wechat/default",
        _ => throw new NotSupportedException($"通道 {channel} 缺少默认路由配置。"),
    };
}
