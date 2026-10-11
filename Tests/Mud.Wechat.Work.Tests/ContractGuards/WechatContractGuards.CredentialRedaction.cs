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
/// 类名与测试 FQN 不变 —— 本文件承载 QueryCredentialParams_ShouldBeRedactionRegisteredOrExplicitlyExempted / ReadComponentSensitiveVocabulary / EnumerateCredentialQueryParamNames。
/// </summary>
public partial class WechatContractGuards
{
    /// <summary>
    /// 契约守卫 G7（P0-4）：Query 承载凭据的参数名必须已被组件脱敏词表覆盖，或在豁免清单中显式登记；
    /// 且豁免清单**自动过期**（组件词表一旦覆盖该键，豁免必须移除）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为何不是「未覆盖即失败」</b>：组件（<c>Mud.HttpUtils</c>，独立仓库、NuGet 单一版本锁定）的词表补齐
    /// 属跨仓交付（C-01，已在组件 2.0.10 源码落地，并随 <b>3.0.0</b> 被本仓库消费），
    /// 跨仓期间无法在同一提交内使其转绿；若写成硬失败，则与「门禁必须全绿」的硬约束冲突。
    /// 故本守卫的职责是<b>可审计 + 自过期</b>：任何新增的 Query 凭据参数都必须做出「已覆盖 / 豁免（附追踪号）」决策。
    /// </para>
    /// <para>
    /// <b>自过期机制</b>：豁免项若已被组件词表覆盖，本守卫立即失败并要求清理 —— 升级
    /// <c>Mud.HttpUtils</c> 到含 C-01 的版本后，豁免清单**不能**被静默遗留（否则未来真实缺口会被掩盖）。
    /// </para>
    /// <para>
    /// <c>SensitiveUrlRedactor</c> 为组件 internal 类型，SDK 无法编译期引用 → 反射读取；
    /// 测试工程单 TFM net8.0 且不参与 AOT strict 冒烟（verify-build 步骤 2 排除 Tests），反射可接受。
    /// </para>
    /// </remarks>
    [Fact]
    public void QueryCredentialParams_ShouldBeRedactionRegisteredOrExplicitlyExempted()
    {
        // 豁免清单：每条 MUST 带追踪号与理由。
        // 【已清空】Mud.HttpUtils 3.0.0（含 C-01 词表补齐）已消费 ⇒ corpsecret / suite_access_token /
        // provider_access_token 均被组件词表覆盖，按本守卫的「自过期」机制清空（并由下方 stale 断言防止回归）。
        var exemptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var vocabulary = ReadComponentSensitiveVocabulary();
        vocabulary.Should().NotBeEmpty("未能读取组件脱敏词表（组件版本或字段名变更，请同步本守卫）");

        var stale = exemptions.Keys.Where(vocabulary.Contains).OrderBy(p => p, StringComparer.Ordinal).ToList();
        stale.Should().BeEmpty(
            "以下豁免项已被组件脱敏词表覆盖（组件 ≥ 2.0.10 含 C-01）⇒ 豁免已过期，请从本守卫的豁免清单中删除：" +
            string.Join(", ", stale));

        var uncovered = EnumerateCredentialQueryParamNames()
            .Where(p => !vocabulary.Contains(p) && !exemptions.ContainsKey(p))
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToList();

        uncovered.Should().BeEmpty(
            "以下 Query 参数承载凭据、既未被组件脱敏词表覆盖、也未登记豁免：会随 ApiException.RequestUri / " +
            "遥测 URL 明文外泄。请二选一：补齐组件词表（C-01）或在本守卫豁免清单登记（须附追踪号）：" +
            string.Join(", ", uncovered));
    }

    /// <summary>反射读取组件脱敏词表（单一事实源：<c>SensitiveUrlRedactor.SensitiveFieldNames</c>）。</summary>
    private static List<string> ReadComponentSensitiveVocabulary()
    {
        var abstractions = typeof(Mud.HttpUtils.ApiException).Assembly;
        var redactor = abstractions.GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（G7 依赖其词表）");

        var field = redactor!.GetField("SensitiveFieldNames",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        field.Should().NotBeNull();

        var value = field!.GetValue(null) as System.Collections.IEnumerable;
        value.Should().NotBeNull();

        return value!.Cast<string>().ToList();
    }

    /// <summary>
    /// 枚举「Query 承载凭据」的参数名：扫描 SDK 全部接口，收集
    /// ① <c>[Query("x")]</c> 参数名；② <c>[Token(..., InjectionMode = Query, Name = "x")]</c> 的 Name；
    /// 再以「名称含 token / secret」过滤为凭据面。
    /// </summary>
    private static List<string> EnumerateCredentialQueryParamNames()
    {
        var assemblies = new[]
        {
            typeof(WechatWorkServiceCollectionExtensions).Assembly,
            typeof(Mud.Wechat.Work.Abstractions.WechatTokenTypes).Assembly,
        };

        var names = new List<string>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsInterface)
                {
                    continue;
                }

                foreach (var method in type.GetMethods())
                {
                    foreach (var parameter in method.GetParameters())
                    {
                        if (parameter.GetCustomAttribute<Mud.HttpUtils.Attributes.QueryAttribute>() is { } query
                            && !string.IsNullOrEmpty(query.Name))
                        {
                            names.Add(query.Name!);
                        }
                    }
                }

                if (type.GetCustomAttribute<Mud.HttpUtils.Attributes.TokenAttribute>() is { } token
                    && token.InjectionMode == Mud.HttpUtils.TokenInjectionMode.Query
                    && !string.IsNullOrEmpty(token.Name))
                {
                    names.Add(token.Name!);
                }
            }
        }

        return names
            .Where(n => n.IndexOf("token", StringComparison.OrdinalIgnoreCase) >= 0
                        || n.IndexOf("secret", StringComparison.OrdinalIgnoreCase) >= 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
    }
}
