// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.ShortLink;

namespace Mud.Wechat.OfficialAccount.Tests.ShortLink;

/// <summary>
/// 长信息与短链域 DTO 与官方报文的双向映射（官方页示例夹具）。
/// </summary>
public class MpShortLinkSerializationTests
{
    /// <summary>SL-S1：<c>shorten/gen</c> 请求体序列化（long_data / expire_seconds）。</summary>
    [Fact]
    public void GenRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpShortenGenRequest { LongData = "LONG_DATA", ExpireSeconds = 86400 };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, ShortLinkJsonContext.Default.MpShortenGenRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "long_data", "expire_seconds" });
        document.RootElement.GetProperty("long_data").GetString().Should().Be("LONG_DATA");
        document.RootElement.GetProperty("expire_seconds").GetInt32().Should().Be(86400);
    }

    /// <summary>SL-S2：<c>shorten/gen</c> 未指定 expire_seconds 时不得输出该键（官方默认值 2592000 由服务端取）。</summary>
    [Fact]
    public void GenRequest_ShouldOmitExpireSecondsWhenUnset()
    {
        var request = new MpShortenGenRequest { LongData = "LONG_DATA" };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, ShortLinkJsonContext.Default.MpShortenGenRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "long_data" },
                "官方 expire_seconds 默认 2592000 ⇒ 不传即由服务端取默认值");
    }

    /// <summary>SL-S3：<c>shorten/gen</c> 响应解析（short_key 15 字节 base62）。</summary>
    [Fact]
    public void GenResponse_ShouldParseOfficialSample()
    {
        const string json = """{"short_key":"iTqRJFSEqk9RvPk"}""";

        var response = JsonSerializer.Deserialize(json, ShortLinkJsonContext.Default.MpShortenGenResponse);

        response!.ShortKey.Should().Be("iTqRJFSEqk9RvPk");
        response.IsSuccess.Should().BeTrue("成功响应不带 errcode ⇒ 缺省 0");
    }

    /// <summary>SL-S4：<c>shorten/fetch</c> 请求体序列化与响应解析。</summary>
    [Fact]
    public void FetchRequestAndResponse_ShouldMatchOfficialShape()
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpShortenFetchRequest { ShortKey = "iTqRJFSEqk9RvPk" },
            ShortLinkJsonContext.Default.MpShortenFetchRequest));
        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "short_key" });

        const string json = """{"long_data":"LONG_DATA","create_time":1611047541,"expire_seconds":85999}""";
        var response = JsonSerializer.Deserialize(json, ShortLinkJsonContext.Default.MpShortenFetchResponse);

        response!.LongData.Should().Be("LONG_DATA");
        response.CreateTime.Should().Be(1611047541);
        response.ExpireSeconds.Should().Be(85999, "官方原文为「剩余的过期秒数」");
    }

    /// <summary>SL-S5：短链专属错误码形态解析（9410010 / 9410011 / 9410012）。</summary>
    [Fact]
    public void ShortLinkErrors_ShouldParseIntoSharedMpResponse()
    {
        foreach (var (code, name) in new[]
                 {
                     (9410010, nameof(MpErrorCodes.ShortenLongDataTooLong)),
                     (9410011, nameof(MpErrorCodes.ShortenExpireOutOfRange)),
                     (9410012, nameof(MpErrorCodes.ShortenKeyNotExists)),
                 })
        {
            var json = "{\"errcode\":" + code + ",\"errmsg\":\"error\"}";
            var response = JsonSerializer.Deserialize(json, CommonJsonContext.Default.MpResponse);

            response!.ErrorCode.Should().Be(code, $"{name} 取值锁定");
            response.IsSuccess.Should().BeFalse();
        }
    }
}
