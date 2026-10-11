// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.DynamicCreatives;

namespace Mud.Wechat.Ads.Tests.DataModels.DynamicCreatives;

/// <summary>
/// 组件化创意域 DTO 的官方键绑定测试（2026-10-11 L3 核验后的样本回放）。
/// </summary>
/// <remarks>
/// <b>锁形态不锁语义</b>：组件 <c>value</c> 是逐组件 union、以开放字典承载（核验留档 §4-D1）——
/// 本文件验证「字典键原名入袋、官方样例零丢字段、双分页形态各自绑定」。
/// 解析器固定为源生成上下文合并（不用反射重载，AOT 口径一致）。
/// </remarks>
public class AdsDynamicCreativeJsonTests
{
    private static IJsonTypeInfoResolver Resolver()
        => System.Text.Json.Serialization.Metadata.JsonTypeInfoResolver.Combine(
            DynamicCreativesJsonContext.Default,
            CommonJsonContext.Default);

    private static T? Deserialize<T>(string json)
        where T : class
        => (T?)System.Text.Json.JsonSerializer.Deserialize(
            json, new System.Text.Json.JsonSerializerOptions { TypeInfoResolver = Resolver() }.GetTypeInfo(typeof(T))!);

    [Fact]
    public void DynamicCreativeGetResponse_ShouldBindOfficialSample_ThroughSourceGeneratedContext()
    {
        const string json = """
{
  "code": 0,
  "message": "",
  "message_cn": "",
  "data": {
    "list": [
      {
        "adgroup_id": 1234567890,
        "dynamic_creative_id": 111,
        "dynamic_creative_name": "DC-1",
        "creative_template_id": 999,
        "delivery_mode": "DELIVERY_MODE_NORMAL",
        "dynamic_creative_type": "DYNAMIC_CREATIVE_TYPE_UNION",
        "creative_components": {
          "title": [ { "component_id": 1, "value": { "content": "标题", "content_alignment": "LEFT" }, "is_deleted": false } ],
          "image": [ { "component_id": 2, "value": { "image_id": "img-1", "image_url": "https://e.qq.com/i.png", "jump_info": { "page_type": "PAGE_TYPE_DEFAULT" } }, "is_deleted": false } ],
          "video": []
        },
        "configured_status": "AD_STATUS_NORMAL",
        "is_deleted": false,
        "created_time": 1700000000,
        "creative_insight": { "duplicate_component_id_list": [7, 8] }
      }
    ],
    "page_info": { "page": 1, "page_size": 10, "total_number": 1, "total_page": 1 },
    "cursor_page_info": { "page_size": 10, "total_number": 1, "next_cursor": "c-next", "previous_cursor": "c-prev" }
  }
}
""";

        var response = Deserialize<AdsDynamicCreativeGetResponse>(json);

        response!.IsSuccess.Should().BeTrue();
        var creative = response.Data!.List!.Single();
        creative.DynamicCreativeId.Should().Be(111);
        creative.AdgroupId.Should().Be(1234567890);
        creative.DeliveryMode.Should().Be("DELIVERY_MODE_NORMAL");

        // 组件键集固定属性 + value 开放字典零丢字段（官方键原名入袋）。
        var title = creative.CreativeComponents!.Title!.Single();
        title.ComponentId.Should().Be(1);
        title.IsDeleted.Should().BeFalse();
        title.Value!.Keys.Should().Equal(new[] { "content", "content_alignment" });
        title.Value["content"].GetString().Should().Be("标题");
        var image = creative.CreativeComponents.Image!.Single();
        image.Value!["jump_info"].GetProperty("page_type").GetString().Should().Be("PAGE_TYPE_DEFAULT");

        // 双分页形态各自绑定（游标为字符串——与 advertiser 的整数游标不同构）。
        response.Data.PageInfo!.TotalNumber.Should().Be(1);
        response.Data.CursorPageInfo!.NextCursor.Should().Be("c-next");
        response.Data.CursorPageInfo.PreviousCursor.Should().Be("c-prev");
    }

    [Fact]
    public void AddRequest_ShouldOmitNullFields_WhenSerializingPatch()
    {
        var request = new AdsDynamicCreativeAddRequest
        {
            AccountId = 111,
            AdgroupId = 222,
            DynamicCreativeName = "DC-2",
            CreativeComponents = new AdsCreativeComponents
            {
                Title = new List<AdsCreativeComponentItem>
                {
                    new()
                    {
                        ComponentId = 1,
                        Value = new Dictionary<string, System.Text.Json.JsonElement>
                        {
                            ["content"] = System.Text.Json.JsonSerializer.Deserialize(
                                "\"标题\"", DynamicCreativesJsonContext.Default.JsonElement),
                        },
                    },
                },
            },
        };

        var json = System.Text.Json.JsonSerializer.Serialize(
            request, DynamicCreativesJsonContext.Default.AdsDynamicCreativeAddRequest);

        json.Should().Contain("\"account_id\":111").And.Contain("\"adgroup_id\":222")
            .And.Contain("\"dynamic_creative_name\":\"DC-2\"");
        json.Should().Contain("\"content\":");
        json.Should().NotContain("delivery_mode", "未赋值的可空字段（WhenWritingNull）不得上送");
        json.Should().NotContain("program_creative_info");
    }

    [Fact]
    public void WriteResponses_ShouldExposeScalarCreativeId()
    {
        var response = Deserialize<AdsDynamicCreativeAddResponse>(
            """{"code":0,"message":"","message_cn":"","data":{"dynamic_creative_id":333}}""");

        response!.IsSuccess.Should().BeTrue();
        response.Data!.DynamicCreativeId.Should().Be(333);
    }
}
