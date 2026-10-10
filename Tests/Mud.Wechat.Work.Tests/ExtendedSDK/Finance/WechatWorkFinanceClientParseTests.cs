// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Work.DataModels.Finance;
using Mud.Wechat.Work.ExtendedSDK.Finance;

namespace Mud.Wechat.Work.Tests.ExtendedSDK.Finance;

/// <summary>
/// 会话存档解析内核（<see cref="WechatWorkFinanceClient.Parse{T}"/>）的行为测试：
/// 设备控制符脏数据清洗「只在解析失败路径发生一次」，且清洗针对<b>原始控制字节</b>形态。
/// </summary>
/// <remarks>
/// <para>
/// <b>背景（2026-10-10 修复）</b>：历史实现剥的是「<c>\u0011</c>」6 字符<b>转义文本</b> —— 那是合法
/// JSON 转义，解析根本不会失败，清洗分支永不生效（与本地 SKIT 源码同源缺陷）。真实脏数据是
/// <b>原始 U+0011~U+0014 字节</b>直接出现在字符串字面量内（JSON 规范禁止），正是它们触发
/// <see cref="JsonException"/>。本文件三个用例把「脏数据可被 rescue、合法数据不被破坏、清洗后仍失败有明确诊断」
/// 全部钉死，防止回归回 6 字符文本形态（守卫 FIN-B4b 只锁源码文本，锁不住行为）。
/// </para>
/// </remarks>
public class WechatWorkFinanceClientParseTests
{
    /// <summary>原始控制字节（C# 源码 <c>"\u0011"</c> 是 1 个字符，不是 6 字符文本）可被清洗 rescue。</summary>
    [Theory]
    [InlineData("\u0011")]
    [InlineData("\u0012")]
    [InlineData("\u0013")]
    [InlineData("\u0014")]
    public void Parse_ShouldStripRawDeviceControlChars_WhenJsonIsIllegal(string controlChar)
    {
        // JSON 文本里 content 的值内嵌原始控制字节（1 个字符，非 6 字符转义文本）—— 非法 JSON 形态。
        var dirty = "{\"msgid\":\"m-1\",\"msgtype\":\"text\",\"text\":{\"content\":\"a" + controlChar + "b\"}}";

        var message = ParseMessage(dirty);

        message!.Text!.Content.Should().Be("ab",
            "官方脏数据（原始设备控制字节）应被剥离而不是让整条记录解析失败");
        message.MessageId.Should().Be("m-1", "清洗只剥控制字符，消息头不受影响");
    }

    /// <summary>合法的「\u0011」转义文本是合法 JSON：不触发清洗、解析成功、内容按规范解码保留。</summary>
    [Fact]
    public void Parse_ShouldKeepLegalEscapedText_WithoutRescue()
    {
        // JSON 文本层面是 6 字符转义序列（\ + u0011），由 C# 的 \\ 还原。
        var legal = """{"msgid":"m-2","msgtype":"text","text":{"content":"a\\u0011b"}}""";

        var message = ParseMessage(legal);

        message!.Text!.Content.Should().Be("a\\u0011b",
            "6 字符转义文本按 JSON 规范解码为字面文本，走「先原样解析」主路径、清洗分支不得介入");
    }

    /// <summary>清洗后仍失败 ⇒ 落统一异常，诊断文本指明「已清洗仍失败」，且不回显报文。</summary>
    [Fact]
    public void Parse_ShouldThrowWithDiagnosis_WhenCleaningStillFails()
    {
        // 双重损坏：原始控制字节之外还有结构级坏点（缺闭合引号），清洗救不了。
        var broken = "{\"msgid\":\"m-3\",\"msgtype\":\"text\",\"text\":{\"content\":\u0011broken";

        var act = () => ParseMessage(broken);

        var ex = act.Should().Throw<InvalidOperationException>();
        ex.Which.Message.Should().Contain("已剥设备控制符后仍失败");
        ex.Which.Message.Should().NotContain("broken", "异常消息不得回显报文内容（明文即敏感）");
        ex.Which.InnerException.Should().BeOfType<JsonException>("首轮 JsonException 必须保留为内部异常");
    }

    /// <summary>不含控制字节的坏 JSON ⇒ 诊断文本指明「未检出设备控制符脏数据」（走到清洗分支但无命中）。</summary>
    [Fact]
    public void Parse_ShouldReportNoControlCharsFound_WhenDirtyJsonHasNone()
    {
        const string broken = """{"msgid":"m-4","msgtype":"text","text":{"content":"broken""";

        var act = () => ParseMessage(broken);

        act.Should().Throw<InvalidOperationException>().Which.Message.Should()
            .Contain("未检出设备控制符脏数据");
    }

    private static FinanceChatMessage? ParseMessage(string json)
        => WechatWorkFinanceClient.Parse(
            json,
            static json => JsonSerializer.Deserialize(json, FinanceJsonContext.Default.FinanceChatMessage),
            nameof(FinanceJsonContext));
}
