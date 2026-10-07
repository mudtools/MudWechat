// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.ProviderAuthentication;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 服务商登录授权域（Provider Login 族）契约守卫：路由表、令牌绑定、
/// <b>errcode 缺省成功语义</b>与 DTO 上下文登记锁定
/// （获取登录用户信息 <c>/cgi-bin/service/get_login_info</c>，官方文档 91154）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：<see cref="IWechatWorkProviderAuthenticationService"/>（独立服务商令牌族：
/// 单接口直接注册，无父/子接口分层——provider_access_token 凭据来源唯一、不声明归属域键）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：① <b>errcode 缺省成功语义</b>——官方 91154「因历史原因，
/// 调用失败时才返回 errcode，无 errcode 字段视为成功」，响应体直接继承
/// <see cref="WechatWorkResponse"/>（errcode 为 int、缺省 0）即正确表达该契约，
/// 不得改为可空 errcode / 自定义 IsSuccess / 自建判定器；② <c>auth_code</c> 单次使用、
/// 5 分钟内有效、最长 512 字节；③ 返回 <c>name</c> 自 2020-06-30 起不再返回真实姓名（返回 userid）。
/// </para>
/// </remarks>
public class WechatProviderLoginContractGuards
{
    /// <summary>
    /// 契约守卫 PL1：端点路由与 errcode 缺省成功语义锁定——
    /// 路由恒为 <c>/cgi-bin/service/get_login_info</c>（官方即 POST）；
    /// 响应体必须直接继承 <see cref="WechatWorkResponse"/>（int errcode 缺省 0），
    /// 不得声明自有 errcode 成员或改写成功判定（缺字段 = 成功）。
    /// </summary>
    [Fact]
    public void ProviderLoginEndpoint_ShouldMatchOfficialRoute_AndKeepDefaultSuccessSemantics()
    {
        var target = typeof(IWechatWorkProviderLoginService).GetMethod(
            nameof(IWechatWorkProviderLoginService.GetLoginInfoAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        target.Should().NotBeNull("get_login_info 必须存在（官方仅此 1 个 HTTP 端点；SSO 扫码链接为 URL 拼装，不设 SDK 端点）");

        var post = target!.GetCustomAttribute<PostAttribute>();
        post.Should().NotBeNull("get_login_info 必须声明 [Post] 路由");
        post!.RequestUri.Should().Be("/cgi-bin/service/get_login_info",
            "get_login_info 路由必须与官方契约一致");

        // errcode 缺省成功语义：响应体直接继承统一响应基底（errcode 为 int、缺省 0），
        // 报文缺失 errcode 字段时反序列化保持 0 ⇒ IsSuccess 按成功处理。
        var responseType = target.ReturnType.GetGenericArguments()[0];
        responseType.Should().Be(typeof(GetLoginInfoResponse), "GetLoginInfoAsync 必须返回 GetLoginInfoResponse");
        responseType.BaseType.Should().Be(typeof(WechatWorkResponse),
            "响应体必须直接继承 WechatWorkResponse（int errcode 缺省 0 ⇒ 缺字段 = 成功）");
        responseType.GetProperty(nameof(WechatWorkResponse.ErrorCode))!
            .DeclaringType.Should().Be(typeof(WechatWorkResponse),
            "响应体不得声明/覆写自有 errcode 成员（官方契约：无 errcode 字段视为成功）");
        responseType.GetProperty(nameof(WechatWorkResponse.IsSuccess))!
            .DeclaringType.Should().Be(typeof(WechatWorkResponse),
            "响应体不得覆写成功判定（缺省成功语义由统一基底承载，守卫锁定该判定器路径）");

        // 请求体：auth_code 单次使用、5 分钟内有效。
        var bodyParam = target.GetParameters()
            .SingleOrDefault(p => p.GetCustomAttribute<BodyAttribute>() != null);
        bodyParam.Should().NotBeNull("get_login_info 以 auth_code 请求体换登录用户信息");
        bodyParam!.ParameterType.Should().Be(typeof(GetLoginInfoRequest),
            "请求体必须为 GetLoginInfoRequest");
        bodyParam.ParameterType.GetProperty(nameof(GetLoginInfoRequest.AuthCode))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("auth_code",
            "官方字段名必须照抄原文（auth_code）");
    }

    /// <summary>
    /// 契约守卫 PL2：令牌绑定——独立服务商令牌族，消费 provider_access_token 并以 Query 注入；
    /// 非应用类型子接口（provider_access_token 凭据来源唯一），不得声明归属域键。
    /// </summary>
    [Fact]
    public void ProviderLoginTokenBinding_ShouldBeProviderAccessTokenInjectedViaQuery()
    {
        var token = typeof(IWechatWorkProviderLoginService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("IWechatWorkProviderLoginService 必须声明 [Token]");
        token!.TokenType.Should().Be(WechatTokenTypes.ProviderAccessToken,
            "get_login_info 以服务商主体级 provider_access_token 鉴权");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query,
            "官方契约强制 Query 注入（MUD005 已知接受风险）");
        token.Name.Should().Be("provider_access_token",
            "Query 注入参数名必须为官方契约的 provider_access_token");
        token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
            "独立服务商令牌族凭据来源唯一，不得叠加自建归属域键");
        token.TokenManagerKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
            "独立服务商令牌族凭据来源唯一，不得叠加企业归属域键");

        // 单接口直接注册形态：无父/子接口分层（provider_access_token 无归属域歧义）。
        typeof(IWechatWorkProviderLoginService).GetCustomAttribute<HttpClientApiAttribute>()!
            .RegistryGroupName.Should().Be("Authentication",
            "必须挂 Authentication 注册组（与授权流接口共用 AddAuthenticationWebApiHttpClient()）");
    }

    /// <summary>
    /// 契约守卫 PL3：登录域 DTO 必须登记进既有 <c>ProviderAuthenticationJsonContext</c>
    /// （复用既有 Authentication 域上下文，不新建 JsonContext）。
    /// </summary>
    [Fact]
    public void ProviderLoginDataModels_ShouldBeRegisteredInExistingJsonContext()
    {
        var context = ProviderAuthenticationJsonContext.Default;

        foreach (var type in new[] { typeof(GetLoginInfoRequest), typeof(GetLoginInfoResponse) })
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是服务商登录域契约面类型，必须登记进 ProviderAuthenticationJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("ProviderAuthentication",
                $"{type.Name} 的 SerializerClassName 必须为既有域段 ProviderAuthentication（复用既有上下文）");
        }
    }
}
