// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Work.Abstractions.Authentication.Models;

namespace Mud.Wechat.Work.Abstractions.Tests.Authentication;

/// <summary>
/// P0-5：授权领域模型 AOT 上下文——覆盖完整性（消除 AOT006）+ 授权聚合往返可用性。
/// </summary>
public class AuthenticationJsonContextTests
{
    /// <summary>被 <c>[HttpJsonSerializable]</c> 标注、必须被上下文覆盖的全部类型（脚手架口径）。</summary>
    private static readonly Type[] RequiredTypes =
    {
        typeof(WechatAuthAgent),
        typeof(WechatAuthCorpInfo),
        typeof(WechatAuthPrivilege),
        typeof(WechatAuthSharedFrom),
        typeof(WechatAuthUserInfo),
        typeof(WechatCorpAuthorization),
        typeof(WechatCorpExName),
        typeof(WechatDealerCorpInfo),
    };

    [Fact]
    public void Context_ShouldCoverAllHttpJsonSerializableTypes()
    {
        foreach (var type in RequiredTypes)
        {
            AuthenticationJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(
                    $"{type.Name} 标注了 [HttpJsonSerializable]，必须被 AuthenticationJsonContext 覆盖（否则 AOT006 转红）");
        }
    }

    [Fact]
    public void Context_ShouldRoundtripCorpAuthorization()
    {
        var authorization = new WechatCorpAuthorization
        {
            AppKey = "suite-app",
            AuthCorpId = "ww-auth-corp",
            PermanentCode = "perm-1",
            IsCustomizedApp = true,
            AuthMode = 1,
            State = "state-1",
            UpdatedAt = 1735689600000,
            CorpInfo = new WechatAuthCorpInfo { CorpId = "ww-auth-corp", CorpName = "授权企业" },
            Agents = new List<WechatAuthAgent>
            {
                new()
                {
                    AgentId = 1001,
                    Name = "代开发应用",
                    Privilege = new WechatAuthPrivilege { Level = 3 },
                    SharedFrom = new WechatAuthSharedFrom { CorpId = "ww-dealer" },
                },
            },
            AuthUser = new WechatAuthUserInfo { UserId = "admin" },
            DealerCorp = new WechatDealerCorpInfo { CorpId = "ww-dealer", CorpName = "代理商" },
        };

        var json = JsonSerializer.Serialize(authorization, AuthenticationJsonContext.Default.WechatCorpAuthorization);
        var roundtrip = JsonSerializer.Deserialize(json, AuthenticationJsonContext.Default.WechatCorpAuthorization);

        roundtrip.Should().NotBeNull();
        roundtrip!.AppKey.Should().Be("suite-app");
        roundtrip.AuthCorpId.Should().Be("ww-auth-corp");
        roundtrip.PermanentCode.Should().Be("perm-1");
        roundtrip.IsCustomizedApp.Should().BeTrue();
        roundtrip.AuthMode.Should().Be(1);
        roundtrip.State.Should().Be("state-1");
        roundtrip.UpdatedAt.Should().Be(1735689600000);
        roundtrip.CorpInfo!.CorpName.Should().Be("授权企业");
        roundtrip.Agents.Should().HaveCount(1);
        roundtrip.Agents[0].AgentId.Should().Be(1001);
        roundtrip.Agents[0].Privilege!.Level.Should().Be(3);
        roundtrip.Agents[0].SharedFrom!.CorpId.Should().Be("ww-dealer");
        roundtrip.AuthUser!.UserId.Should().Be("admin");
        roundtrip.DealerCorp!.CorpName.Should().Be("代理商");
        roundtrip.AgentId.Should().Be(1001, "派生属性（Agents 首条 AgentId）随嵌套集合还原");

        json.Should().Contain("\"appKey\"", "序列化口径为 camelCase（与脚手架默认一致）");
    }

    [Fact]
    public void Context_ShouldOmitNullMembers_WhenSerializing()
    {
        var authorization = new WechatCorpAuthorization { AppKey = "a", AuthCorpId = "c", PermanentCode = "p" };

        var json = JsonSerializer.Serialize(authorization, AuthenticationJsonContext.Default.WechatCorpAuthorization);

        json.Should().NotContain("corpInfo", "DefaultIgnoreCondition = WhenWritingNull");
        json.Should().NotContain("authUser");
    }

    [Fact]
    public void CombinedResolver_ShouldResolveBothContexts()
    {
        // 与主包 WechatJsonResolverExtensions 的合并方式一致。
        var resolver = JsonTypeInfoResolver.Combine(
            Mud.Wechat.Work.DataModels.CommonJsonContext.Default,
            AuthenticationJsonContext.Default);

        resolver.GetTypeInfo(typeof(WechatCorpAuthorization), AuthenticationJsonContext.Default.Options)
            .Should().NotBeNull("领域模型上下文与传输 DTO 上下文必须可共存于同一解析链");

        resolver.GetTypeInfo(
                typeof(Mud.Wechat.Work.DataModels.WechatWorkResponse),
                Mud.Wechat.Work.DataModels.CommonJsonContext.Default.Options)
            .Should().NotBeNull("合并不得影响既有 DTO 解析");
    }
}
#endif
