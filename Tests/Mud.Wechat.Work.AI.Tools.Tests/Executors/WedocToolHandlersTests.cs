using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Wedoc;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 微文档域执行器：字段投影、docid/formid 互斥、spaceid⇒fatherid 约束、errcode 回填、
/// 以及「写工具成功文本不编造业务事实」。
/// </summary>
public class WedocToolHandlersTests
{
    private readonly Mock<IWechatWorkWedocService> _wedoc = new(MockBehavior.Loose);

    [Fact]
    public async Task GetBaseInfo_ShouldProjectDocumentFields()
    {
        _wedoc
            .Setup(w => w.GetDocumentBaseInfoAsync(It.IsAny<GetWedocDocumentBaseInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetWedocDocumentBaseInfoResponse
            {
                ErrorCode = 0,
                DocBaseInfo = new WedocDocBaseInfo
                {
                    Docid = "doc-1",
                    DocName = "季度复盘",
                    DocType = 3,
                    CreateTime = 1700000000,
                    ModifyTime = 1700003600,
                },
            });

        var result = await Handler().GetBaseInfoAsync(Args(("docid", "doc-1")), CancellationToken.None);

        result.Text.Should().Contain("\"docid\":\"doc-1\"");
        result.Text.Should().Contain("\"doc_name\":\"季度复盘\"");
        result.Text.Should().Contain("\"doc_type\":3");
        _wedoc.Verify(w => w.GetDocumentBaseInfoAsync(
            It.Is<GetWedocDocumentBaseInfoRequest>(r => r.Docid == "doc-1"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldRejectSpaceWithoutFather()
    {
        var result = await Handler().CreateDocumentAsync(
            Args(("doc_name", "周报"), ("doc_type", 3), ("spaceid", "space-1")),
            CancellationToken.None);

        result.Text.Should().Contain("fatherid");
        _wedoc.Verify(
            w => w.CreateDocumentAsync(It.IsAny<CreateWedocDocumentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_ShouldProjectDocidAndUrl()
    {
        _wedoc
            .Setup(w => w.CreateDocumentAsync(It.IsAny<CreateWedocDocumentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateWedocDocumentResponse { ErrorCode = 0, Docid = "doc-9", Url = "https://doc.weixin.qq.com/doc-9" });

        var result = await Handler().CreateDocumentAsync(
            Args(("doc_name", "周报"), ("doc_type", 3), ("admin_users", new[] { "zhangsan" })),
            CancellationToken.None);

        result.Text.Should().Contain("\"docid\":\"doc-9\"");
        result.Text.Should().Contain("doc.weixin.qq.com");
        _wedoc.Verify(w => w.CreateDocumentAsync(
            It.Is<CreateWedocDocumentRequest>(r =>
                r.DocName == "周报"
                && r.DocType == 3
                && r.AdminUsers != null
                && r.AdminUsers.Contains("zhangsan")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("doc-1", "form-1", "只能填其中一个")]
    [InlineData(null, null, "必须提供其中之一")]
    public async Task Rename_ShouldRejectInvalidTargetCombination(string? docid, string? formid, string expected)
    {
        var result = await Handler().RenameDocumentAsync(
            Args(("new_name", "新名字"), ("docid", docid), ("formid", formid)),
            CancellationToken.None);

        result.Text.Should().Contain(expected);
        _wedoc.Verify(
            w => w.RenameDocumentAsync(It.IsAny<RenameWedocDocumentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Rename_ShouldPassSingleTargetAndReturnOkWithoutInventingFacts()
    {
        _wedoc
            .Setup(w => w.RenameDocumentAsync(It.IsAny<RenameWedocDocumentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WechatWorkResponse { ErrorCode = 0 });

        var result = await Handler().RenameDocumentAsync(
            Args(("new_name", "新名字"), ("docid", "doc-1")),
            CancellationToken.None);

        result.Text.Should().Contain("\"ok\":true");
        result.Text.Should().Contain("\"docid\":\"doc-1\"");
        result.Text.Should().NotContain("doc_name", "上游无业务负载时不得编造「已改名」的具体事实");
        _wedoc.Verify(w => w.RenameDocumentAsync(
            It.Is<RenameWedocDocumentRequest>(r => r.Docid == "doc-1" && r.Formid == null && r.NewName == "新名字"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenErrcodeNonZero_ShouldReturnReadableError()
    {
        _wedoc
            .Setup(w => w.DeleteDocumentAsync(It.IsAny<DeleteWedocDocumentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WechatWorkResponse { ErrorCode = 40003, ErrorMessage = "invalid docid" });

        var result = await Handler().DeleteDocumentAsync(Args(("formid", "form-1")), CancellationToken.None);

        result.Text.Should().Contain("40003");
        result.Text.Should().Contain("invalid docid");
    }

    [Fact]
    public async Task Share_ShouldReturnShareUrlAndTarget()
    {
        _wedoc
            .Setup(w => w.ShareDocumentAsync(It.IsAny<ShareWedocDocumentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShareWedocDocumentResponse { ErrorCode = 0, ShareUrl = "https://doc.weixin.qq.com/s/abc" });

        var result = await Handler().ShareDocumentAsync(Args(("docid", "doc-1")), CancellationToken.None);

        result.Text.Should().Contain("share_url");
        result.Text.Should().Contain("doc.weixin.qq.com/s/abc");
        result.Text.Should().Contain("\"docid\":\"doc-1\"");
    }

    private WedocToolHandlers Handler() => new(_wedoc.Object);

    private static Dictionary<string, object?> Args(params (string Key, object? Value)[] pairs)
    {
        var args = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var (key, value) in pairs)
        {
            args[key] = value;
        }

        return args;
    }
}
