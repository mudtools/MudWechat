// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Sns;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 网页授权（sns）域契约守卫（<b>服务号专属</b>）：免令牌裁决（I4/I5）、路由表、注册形态、
/// DTO 字段与两套字段集分别核验裁决、JSON 上下文登记、错误码常量。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07）：服务号域 /doc/service/api/webdev/access/ 共 4 页
/// （subscription 订阅号域无该目录）。域级约束：适用范围「服务号 仅认证」；频率 5 万/分钟（三页）；
/// refresh_token 30 天；sns/userinfo 仍返回 nickname/headimgurl（与 /cgi-bin/user/info 停供字段集不同）。
/// </remarks>
public class MpSnsContractGuards
{
    private const string SnsRegistryGroupName = "Sns";

    /// <summary>网页授权域官方路由表（4 端点，全 GET、无请求体）。</summary>
    private static readonly (Type Interface, string Method, string Route)[] SnsRoutes =
    {
        (typeof(IMpSnsService), nameof(IMpSnsService.GetAccessTokenAsync), "/sns/oauth2/access_token"),
        (typeof(IMpSnsService), nameof(IMpSnsService.RefreshTokenAsync), "/sns/oauth2/refresh_token"),
        (typeof(IMpSnsService), nameof(IMpSnsService.AuthAsync), "/sns/auth"),
        (typeof(IMpSnsService), nameof(IMpSnsService.GetUserInfoAsync), "/sns/userinfo"),
    };

    /// <summary>契约守卫 SN1（SNS）：4 端点路由与官方契约一致，全部 GET 且无请求体。</summary>
    [Fact]
    public void SnsEndpoints_ShouldMatchOfficialRoutes()
    {
        SnsRoutes.Should().HaveCount(4, "网页授权 = 换凭证 1 + 刷新 1 + 检验 1 + 用户信息 1");

        foreach (var (iface, method, route) in SnsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<GetAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 官方契约为 GET");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");

            // GET 端点无请求体：参数仅剩 Query 标注参数与取消令牌。
            target.GetParameters().Should().OnlyContain(
                p => p.GetCustomAttribute<QueryAttribute>() != null || p.ParameterType == typeof(CancellationToken),
                "sns 四端点官方契约均为 Query 传参、无请求体");
        }
    }

    /// <summary>
    /// 契约守卫 SN2（SNS）：I4/I5 裁决锁定——接口<b>不得</b>带 [Token]（用户级凭证形态，
    /// 应用级令牌注入是错误语义；禁止后续「顺手」补 [Token]）。
    /// </summary>
    [Fact]
    public void SnsInterface_ShouldStayOutOfAppTokenPipeline()
    {
        typeof(IMpSnsService).GetCustomAttribute<TokenAttribute>().Should().BeNull(
            "sns 四端点不消费应用级 access_token（I4/I5 裁决）：换/刷新凭证以 appid+secret 入参，" +
            "检验/取用户信息消费用户级凭证——应用级令牌注入是错误语义且会把「取令牌失败」变成调用前置失败");

        // Query 令牌白名单同样不得收录本接口（无 [Token] ⇒ 不在 QT1 发现集合内）。
        var api = typeof(IMpSnsService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull();
        api!.RegistryGroupName.Should().Be(SnsRegistryGroupName, "必须挂 Sns 注册组（AddSnsWebApiHttpClient()）");
        api.TokenManage.Should().Be(nameof(IMpAppManager));
    }

    /// <summary>契约守卫 SN3（SNS）：sns/oauth2/access_token 的 secret 走 Query（官方契约；脱敏词表已覆盖）。</summary>
    [Fact]
    public void SnsAccessTokenEndpoint_ShouldTakeSecretViaQuery()
    {
        var target = typeof(IMpSnsService).GetMethod(nameof(IMpSnsService.GetAccessTokenAsync))!;
        target.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name)
            .Should().BeEquivalentTo(new[] { "appid", "secret", "code", "grant_type" },
                "官方 Query 参数逐项一致（secret 为公众号 AppSecret，官方契约 Query 传，词表已覆盖脱敏）");

        // grant_type 为官方固定值常量（调用方经常量类传入）。
        MpSnsGrantTypes.AuthorizationCode.Should().Be("authorization_code");
        MpSnsGrantTypes.RefreshToken.Should().Be("refresh_token");

        var refresh = typeof(IMpSnsService).GetMethod(nameof(IMpSnsService.RefreshTokenAsync))!;
        refresh.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name)
            .Should().BeEquivalentTo(new[] { "appid", "refresh_token", "grant_type" });
    }

    /// <summary>契约守卫 SN4（SNS）：sns 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void SnsDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = SnsJsonContext.Default;

        var domainTypes = typeof(MpSnsAccessTokenResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Sns"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 4;
        domainTypes.Should().HaveCount(expectedCount,
            "sns 域契约面类型数漂移须先核对官方文档再同批调整本守卫（换凭证/刷新/检验/用户信息响应各 1）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 sns 域命名空间，必须登记进 SnsJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(SnsRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Sns");
        }
    }

    /// <summary>契约守卫 SN5（SNS）：官方字段名与「两套字段集分别核验」裁决锁定。</summary>
    [Fact]
    public void SnsDataModels_ShouldLockOfficialFields()
    {
        // sns/oauth2/access_token 响应：无 scope（逐页核验；与 refresh_token 响应表不同，勿互相「补全」）。
        AssertJsonProperty<MpSnsAccessTokenResponse>("access_token", "网页授权凭证（用户级，与基础 access_token 不同）");
        AssertJsonProperty<MpSnsAccessTokenResponse>("expires_in", "凭证有效期（秒）");
        AssertJsonProperty<MpSnsAccessTokenResponse>("refresh_token", "刷新凭证（30 天）");
        AssertJsonProperty<MpSnsAccessTokenResponse>("openid", "用户唯一标识（未关注也会产生）");
        AssertJsonProperty<MpSnsAccessTokenResponse>("unionid", "仅 snsapi_userinfo 作用域返回");
        AssertJsonProperty<MpSnsAccessTokenResponse>("is_snapshotuser", "快照页虚拟账号标记（仅该形态返回）");
        JsonNamesOf<MpSnsAccessTokenResponse>().Should().NotContain("scope",
            "2026-10-07 核验：sns/oauth2/access_token 响应表无 scope 字段（不得从 refresh_token 页「补全」）");

        // refresh_token 响应：有 scope（照本页原文建模）。
        AssertJsonProperty<MpSnsRefreshTokenResponse>("scope", "用户授权作用域（逗号分隔）");

        AssertJsonProperty<MpSnsUserInfoResponse>("nickname", "本接口在有效授权下仍返回昵称");
        AssertJsonProperty<MpSnsUserInfoResponse>("headimgurl", "本接口在有效授权下仍返回头像");
        AssertJsonProperty<MpSnsUserInfoResponse>("sex", "1 男 / 2 女 / 0 未知");
        AssertJsonProperty<MpSnsUserInfoResponse>("privilege", "用户特权信息（json 数组）");

        // 与 /cgi-bin/user/info 的停供字段集是两回事：sns/userinfo 的这些字段不得按「停供」建模掉。
        typeof(MpSnsUserInfoResponse).GetProperty(nameof(MpSnsUserInfoResponse.Nickname)).Should().NotBeNull();
        typeof(MpSnsUserInfoResponse).GetProperty(nameof(MpSnsUserInfoResponse.HeadImgUrl)).Should().NotBeNull();
    }

    /// <summary>契约守卫 SN6：模块枚举 / 注册入口同批扩展。</summary>
    [Fact]
    public void SnsModule_ShouldBeRegistered()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("Sns");
        typeof(MpServiceBuilder).GetMethod("AddSnsApi").Should().NotBeNull();
    }

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
