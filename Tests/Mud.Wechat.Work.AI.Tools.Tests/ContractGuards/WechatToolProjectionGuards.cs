using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.ContractGuards;

/// <summary>
/// 投影守卫（WT-G6）：执行器<b>出站 JSON 的键名</b>不得出现个人信息字段。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么盯「键名」而不是「值」</b>：执行器的投影是白名单——写错一行 <c>["mobile"] = user.Mobile</c>，
/// 编译通过、评审未必看得出、模型上下文与日志却永久留下手机号。键名是这条边界的唯一可静态检查点。
/// </para>
/// <para>
/// <b>为什么允许出现在入参</b>：<c>contacts.user.resolve</c> 的 <c>mobile</c>/<c>email</c> 是<b>用户自己给出的</b>
/// 查询条件，不是从企微拉回的数据；守卫只扫 JSON 对象字面量的键（<c>["mobile"] =</c>），不扫参数名。
/// </para>
/// <para>
/// <b>命中后怎么办</b>：不是删键了事——先问「这个字段对回答是否必要」。必要则改投影形态
/// （掩码 / 布尔化，如 <c>has_mobile</c>），并在该执行器的 XML 注释里写明理由。
/// </para>
/// </remarks>
public class WechatToolProjectionGuards
{
    /// <summary>禁止出现在出站 JSON 键名里的个人/设备标识（小写比较）。</summary>
    private static readonly string[] ForbiddenKeys =
    [
        "mobile",
        "email",
        "unionid",
        "id_card",
        "identity_card",
        "bank_card",
        "latitude",
        "longitude",
        "lat",
        "lng",
        "deviceid",
        "device_id",
        "avatar",
        "avatars",
        "remark_mobiles",
        "telephone",
    ];

    [Fact]
    public void Executors_ShouldNotProjectPersonalFieldsInJsonKeys()
    {
        var executors = Path.Combine(RepoRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Tools", "Executors");
        Directory.Exists(executors).Should().BeTrue($"执行器目录必须可定位（当前解析到 {executors}）");

        var pattern = new Regex(
            $"\\[\"(?<key>{string.Join('|', ForbiddenKeys)})\"\\]\\s*=",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        var files = Directory.GetFiles(executors, "*.cs", SearchOption.TopDirectoryOnly);
        files.Should().HaveCountGreaterThan(10, "扫不到执行器文件时守卫会假绿（等于没守）");

        var violations = new List<string>();
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var match = pattern.Match(lines[i]);
                if (match.Success)
                {
                    violations.Add($"{Path.GetFileName(file)}:{i + 1} 键 \"{match.Groups["key"].Value}\"");
                }
            }
        }

        violations.Should().BeEmpty(
            "个人信息不得进入模型上下文（日志与上下文留存期远长于一次请求）；"
            + "确有必要时改为掩码 / 布尔化（如 has_mobile）并写明理由。命中："
            + string.Join(" | ", violations));
    }

    /// <summary>从测试输出目录向上定位仓库根（以 <c>Mud.Wechat.slnx</c> 为锚）。</summary>
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException($"未能从 {AppContext.BaseDirectory} 向上定位仓库根（缺少 Mud.Wechat.slnx）。");
    }
}
