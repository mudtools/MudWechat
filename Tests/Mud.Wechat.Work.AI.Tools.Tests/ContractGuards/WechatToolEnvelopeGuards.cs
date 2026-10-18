using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.ContractGuards;

/// <summary>
/// 分页信封守卫（WT-P1~P3，FIX-02）：<b>一种分页词</b>（<c>items</c>/<c>has_more</c>/<c>page_token</c>），
/// 且只能由 <c>ToolResultJsons.PageEnvelope</c> 产出。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须收敛成一种词</b>：改造前同一件事有 7 种写法（<c>next_cursor</c> / <c>next_offset</c> /
/// <c>next_key</c>+<c>ending</c> / <c>is_last</c> / <c>next_cursor_present</c> / 本地推导 <c>has_more</c> /
/// 干脆没有分页）。模型对每个域都要重新猜「哪个键是翻页把手」，而猜错的代价是**静默重复拉第一页**
/// —— 比失败更难自查。
/// </para>
/// <para>
/// <b>为什么禁用本地推导的 has_more</b>：`records.Count &gt;= limit` 这类推导在「恰好满页且已到底」时
/// 假报有下一页；官方给了结束标记（<c>ending=1</c> / <c>is_last</c>）或游标时，必须以官方为准。
/// </para>
/// <para>
/// <b>为什么 page_token 只由工厂写</b>：空 token 必须**省略**而不是写成 <c>""</c>（平台对空 token 的处理是
/// 从头再来 ⇒ 重复拉第一页）。把这条规则留在 20 个站点各自实现，必然有一处写成空串。
/// </para>
/// </remarks>
public class WechatToolEnvelopeGuards
{
    /// <summary>已废弃的分页键（出现即红）：它们是「同一事实的 7 种方言」。</summary>
    private static readonly string[] LegacyKeys =
        ["next_cursor", "next_offset", "next_key", "is_last", "ending", "next_cursor_present"];

    [Fact]
    public void Executors_ShouldNotEmitLegacyPaginationKeys()
    {
        var offenders = Directory
            .GetFiles(ExecutorsDirectory(), "*.cs", SearchOption.TopDirectoryOnly)
            .Select(file => (Name: Path.GetFileName(file), Source: File.ReadAllText(file)))
            .SelectMany(f => LegacyKeys
                .Where(key => f.Source.Contains($"\"{key}\"", StringComparison.Ordinal))
                .Select(key => $"{f.Name}:{key}"))
            .ToArray();

        offenders.Should().BeEmpty(
            "分页只有一种表达（items/has_more/page_token）；逐域方言会让模型对每个域重新猜翻页键");
    }

    [Fact]
    public void Executors_ShouldNotHandRollPageToken()
    {
        // page_token 只能由 PageEnvelope 写入（空值必须省略，而不是写成空串）。
        var offenders = Directory
            .GetFiles(ExecutorsDirectory(), "*.cs", SearchOption.TopDirectoryOnly)
            .Where(file => File.ReadAllText(file).Contains("[\"page_token\"]", StringComparison.Ordinal))
            .Select(Path.GetFileName)
            .ToArray();

        offenders.Should().BeEmpty("page_token 的「空则不写」规则由 ToolResultJsons.PageEnvelope 单点持有");
    }

    [Fact]
    public void Executors_ShouldBuildPagedResultsThroughFactory()
    {
        var executors = Directory
            .GetFiles(ExecutorsDirectory(), "*.cs", SearchOption.TopDirectoryOnly)
            .Select(file => (Name: Path.GetFileName(file), Source: File.ReadAllText(file)))
            .ToArray();

        var pagedCallers = executors
            .Where(f => f.Source.Contains("PageEnvelope(", StringComparison.Ordinal))
            .Select(f => f.Name)
            .ToArray();

        // 防假绿：一处分页都没走工厂时，本守卫等于没守（历史上有 13 个分页点）。
        pagedCallers.Should().HaveCountGreaterThanOrEqualTo(
            12,
            "分页工具必须走 PageEnvelope；数量骤降说明有人绕过了工厂");

        var hasMoreWriters = executors
            .Where(f => Regex.IsMatch(f.Source, "\\[\"has_more\"\\]"))
            .Select(f => f.Name)
            .ToArray();

        hasMoreWriters.Should().BeEmpty("has_more 由 PageEnvelope 产出；手写 has_more 一定伴随本地推导");
    }

    private static string ExecutorsDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "Src", "AI", "Mud.Wechat.Work.AI.Tools", "Executors");
            if (Directory.Exists(candidate))
            {
                Directory.GetFiles(candidate, "*.cs", SearchOption.TopDirectoryOnly)
                    .Length.Should().BeGreaterThan(10, "扫不到执行器时守卫会假绿");
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException($"未能从 {AppContext.BaseDirectory} 定位执行器目录。");
    }
}
