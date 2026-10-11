// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.OfficialAccount.DataModels.Semantic;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 语义理解（智能对话旧接口）域契约守卫（B2a 落地）。
/// </summary>
/// <remarks>
/// <b>details 原样承载裁决</b>：官方 <c>details</c> 结构随服务类型有二十余种形态且不受控演进，
/// 逐形态建模即开放多态 DTO（与本仓「不做运行时多态」红线冲突）——本守卫锁定
/// <see cref="MpSemanticResult.Details"/> 为 <see cref="System.Text.Json.JsonElement"/>（原样 JSON），
/// 防止后来者「顺手」拆成强类型族。
/// </remarks>
public class MpSemanticContractGuards
{
    private const string RegistryGroupName = "Semantic";

    /// <summary>契约守卫 SE1：单端点路由与官方契约一致（全 POST /semantic/*）。</summary>
    [Fact]
    public void SemanticEndpoints_ShouldMatchOfficialRoutes()
    {
        var target = typeof(IMpSemanticService)
            .GetMethod(nameof(IMpSemanticService.SearchAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        target.Should().NotBeNull("语义理解域恰 1 个端点");

        var attr = target!.GetCustomAttribute<PostAttribute>();
        attr.Should().NotBeNull("SearchAsync 必须声明 POST 路由");
        attr!.RequestUri.Should().Be("/semantic/semproxy/search", "路由必须与官方契约一致");

        typeof(IMpSemanticService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
            .Should().HaveCount(1, "本域恰 1 端点（官方已停维，无扩展面）");
    }

    /// <summary>契约守卫 SE2：DTO 登记 + details 原样承载裁决锁定。</summary>
    [Fact]
    public void SemanticDataModels_ShouldBeRegisteredInJsonContext_AndKeepRawDetails()
    {
        var context = SemanticJsonContext.Default;

        var domainTypes = typeof(MpSemanticSearchRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Semantic"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 3;
        domainTypes.Should().HaveCount(expectedCount,
            "语义域契约面类型数漂移须先核对官方文档再同批调整本守卫（请求 1 + 响应 1 + 语义结果 1）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于语义域命名空间，必须登记进 SemanticJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // details 原样承载（JsonElement）：拆强类型族即引入开放多态 DTO，违反 AOT 红线（裁决见类型 remarks）。
        typeof(MpSemanticResult).GetProperty(nameof(MpSemanticResult.Details))!.PropertyType
            .Should().Be(typeof(System.Text.Json.JsonElement?),
                "details 官方有二十余种形态且随类型演进，SDK 以原始 JSON 透出、由调用方按服务类型解析");
    }
}
