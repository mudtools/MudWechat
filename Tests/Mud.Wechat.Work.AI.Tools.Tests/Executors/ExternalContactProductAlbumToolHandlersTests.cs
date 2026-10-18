using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 商品图册域执行器：商品事实的投影形状，以及「media_id（宿主素材）不外发」这条边界。
/// </summary>
public class ExternalContactProductAlbumToolHandlersTests
{
    private readonly Mock<IWechatWorkExternalContactProductAlbumService> _productAlbum = new(MockBehavior.Loose);

    [Fact]
    public async Task List_ShouldProjectProductFactsWithoutMediaId()
    {
        _productAlbum
            .Setup(s => s.GetProductAlbumListAsync(It.IsAny<GetProductAlbumListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProductAlbumListResponse
            {
                ErrorCode = 0,
                ProductList =
                [
                    new ProductAlbumInfo
                    {
                        ProductId = "p-1",
                        ProductSn = "SKU-001",
                        Description = "企业定制礼盒",
                        Price = 19900,
                        CreateTime = 1700000000,
                        Attachments = [new ProductAlbumAttachment { Type = "image", Image = new ProductAlbumImage { MediaId = "media-1" } }],
                    },
                ],
                NextCursor = "c2",
            });

        var result = await Handler().ListAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"product_id\":\"p-1\"");
        result.Text.Should().Contain("\"product_sn\":\"SKU-001\"");
        result.Text.Should().Contain("\"price\":19900", "官方价格单位为分，执行器原样投影");
        result.Text.Should().Contain("\"image_count\":1");
        result.Text.Should().Contain("\"next_cursor\":\"c2\"");
        result.Text.Should().NotContain("media-1", "商品图片 media_id 是宿主素材资产，不外发");
    }

    [Fact]
    public async Task Get_WithoutProductPayload_ShouldReturnNotFoundAsSuccess()
    {
        _productAlbum
            .Setup(s => s.GetProductAlbumAsync(It.IsAny<GetProductAlbumRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetProductAlbumResponse { ErrorCode = 0, Product = null });

        var result = await Handler().GetAsync(Args(("product_id", "p-1")), CancellationToken.None);

        result.Error.Should().BeNull();
        result.Text.Should().Contain("\"found\":false");
    }

    [Fact]
    public async Task List_LimitOverOfficialMax_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await Handler().ListAsync(Args(("limit", 500)), CancellationToken.None);

        result.Text.Should().Contain("1~100");
        _productAlbum.Verify(
            s => s.GetProductAlbumListAsync(It.IsAny<GetProductAlbumListRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private ExternalContactProductAlbumToolHandlers Handler() => new(_productAlbum.Object);

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
