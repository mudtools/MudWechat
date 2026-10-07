// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;

namespace Mud.Wechat.OfficialAccount.Tests.QrcodeJump;

/// <summary>
/// 扫二维码打开小程序域 DTO 与官方报文的双向映射（官方页示例夹具）。
/// </summary>
public class MpQrcodeJumpSerializationTests
{
    /// <summary>QJ-S1：<c>qrcodejumpget</c> 服务号场景响应解析（含本月剩余发布配额）。</summary>
    [Fact]
    public void GetResponse_ShouldParseServiceAccountShape()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok",
            "rule_list":[{"prefix":"http://weixin.qq.com/q/kZgfwMTm72Wxxxx","path":"pages/index/index",
            "state":2}],
            "qrcodejump_open":1,"list_size":1,"qrcodejump_pub_quota":99,"total_count":1}
            """;

        var response = JsonSerializer.Deserialize(json, QrcodeJumpJsonContext.Default.MpQrcodeJumpGetResponse);

        response!.IsSuccess.Should().BeTrue();
        response.QrcodeJumpOpen.Should().Be(1);
        response.ListSize.Should().Be(1);
        response.QrcodeJumpPublishQuota.Should().Be(99, "本月还可发布的次数可用于发布前置探量");
        response.RuleList.Should().HaveCount(1);
        response.RuleList![0].State.Should().Be(2, "2 = 已发布");
        response.RuleList[0].OpenVersion.Should().BeNull("服务号场景不返回测试范围");
        response.RuleList[0].DebugUrls.Should().BeNull("服务号场景不返回测试链接");
    }

    /// <summary>QJ-S2：<c>qrcodejumpget</c> 普通二维码场景响应解析（返回 open_version / debug_url）。</summary>
    [Fact]
    public void GetResponse_ShouldParseNormalQrcodeShape()
    {
        const string json = """
            {"rule_list":[{"prefix":"https://weixin.qq.com/q/02P5KzM_xxxxx","path":"pages/index/index",
            "state":1,"open_version":3,"debug_url":["https://weixin.qq.com/q/02P5KzM_xxxxx"]}],
            "list_size":1,"qrcodejump_pub_quota":100,"total_count":1}
            """;

        var response = JsonSerializer.Deserialize(json, QrcodeJumpJsonContext.Default.MpQrcodeJumpGetResponse);

        var rule = response!.RuleList![0];
        rule.OpenVersion.Should().Be(3, "3 = 正式版");
        rule.DebugUrls.Should().HaveCount(1, "官方原文：测试链接最多 5 个");
        rule.State.Should().Be(1, "1 = 未发布");
    }

    /// <summary>QJ-S3：<c>qrcodejumpadd</c> 服务号场景请求体序列化（字段名须与官方完全一致）。</summary>
    [Fact]
    public void AddRequest_ShouldSerializeServiceAccountShape()
    {
        var request = new MpQrcodeJumpAddRequest
        {
            Prefix = "http://weixin.qq.com/q/kZgfwMTm72Wxxxx",
            AppId = "wxxxxxx",
            Path = "pages/index/index",
            IsEdit = MpQrcodeJumpEditFlags.Add,
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, QrcodeJumpJsonContext.Default.MpQrcodeJumpAddRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "prefix", "appid", "path", "is_edit" },
                "服务号场景四字段；普通二维码场景专属字段未设置时不得输出（官方示例同为四字段）");
        document.RootElement.GetProperty("is_edit").GetInt32().Should().Be(0);
    }

    /// <summary>QJ-S4：<c>qrcodejumpadd</c> 普通二维码场景请求体序列化（含 debug_url / permit_sub_rule）。</summary>
    [Fact]
    public void AddRequest_ShouldSerializeNormalQrcodeShape()
    {
        var request = new MpQrcodeJumpAddRequest
        {
            Prefix = "https://weixin.qq.com/q/02P5KzM_xxxxx",
            Path = "pages/index/index",
            IsEdit = MpQrcodeJumpEditFlags.Edit,
            OpenVersion = MpQrcodeJumpOpenVersions.Trial,
            DebugUrls = new List<string> { "https://weixin.qq.com/q/02P5KzM_xxxxx" },
            PermitSubRule = MpQrcodeJumpSubRuleModes.NotOccupied,
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, QrcodeJumpJsonContext.Default.MpQrcodeJumpAddRequest));

        var root = document.RootElement;
        root.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "prefix", "path", "is_edit", "open_version", "debug_url", "permit_sub_rule" },
                "普通二维码场景六字段（无 appid）");
        root.GetProperty("permit_sub_rule").GetInt32().Should().Be(1);
        root.GetProperty("debug_url").EnumerateArray().Should().HaveCount(1);
    }

    /// <summary>QJ-S5：<c>qrcodejumppublish</c> / <c>qrcodejumpdelete</c> 请求体序列化。</summary>
    [Fact]
    public void PublishAndDeleteRequests_ShouldSerializeOfficialShape()
    {
        using var publish = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpQrcodeJumpPublishRequest { Prefix = "https://weixin.qq.com/qrcodejump" },
            QrcodeJumpJsonContext.Default.MpQrcodeJumpPublishRequest));
        publish.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "prefix" });

        using var delete = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpQrcodeJumpDeleteRequest { Prefix = "https://weixin.qq.com/qrcodejump", AppId = "wxxxxxx" },
            QrcodeJumpJsonContext.Default.MpQrcodeJumpDeleteRequest));
        delete.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "prefix", "appid" }, "appid 为服务号场景字段（官方 delete 页说明）");
    }

    /// <summary>QJ-S6：发布配额耗尽错误形态解析。</summary>
    [Fact]
    public void PublishQuotaError_ShouldParseIntoSharedMpResponse()
    {
        const string json = """{"errcode":886000,"errmsg":"beyond publish count this month"}""";

        var response = JsonSerializer.Deserialize(json, CommonJsonContext.Default.MpResponse);

        response!.ErrorCode.Should().Be(MpErrorCodes.QrcodeJumpPublishQuotaExceeded);
        response.IsSuccess.Should().BeFalse();
    }
}
