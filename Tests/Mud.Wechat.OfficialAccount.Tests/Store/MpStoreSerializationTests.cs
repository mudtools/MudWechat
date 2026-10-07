// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Store;

namespace Mud.Wechat.OfficialAccount.Tests.Store;

/// <summary>
/// 微信门店·门店小程序域 DTO 与官方报文的双向映射（官方页示例夹具）。
/// </summary>
public class MpStoreSerializationTests
{
    /// <summary>ST-S1：类目树解析（含示例独有的 name / children 与三层 level）。</summary>
    [Fact]
    public void MerchantCategoryResponse_ShouldParseCategoryTree()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","data":{"all_category_info":{"categories":[
            {"id":0,"name":"root","level":0,"children":[
              {"id":1,"name":"美食","level":1,"sensitive_type":0,"children":[
                {"id":2,"name":"中餐厅","level":2,"sensitive_type":1}]}]}]}}}
            """;

        var response = JsonSerializer.Deserialize(json, StoreJsonContext.Default.MpMerchantCategoryResponse);

        response!.IsSuccess.Should().BeTrue();
        var roots = response.Data!.AllCategoryInfo!.Categories!;
        roots.Should().HaveCount(1);
        roots[0].Level.Should().Be(0, "官方示例根节点 level = 0（字段说明称「一级或二级」矛盾，照录）");
        roots[0].Name.Should().Be("root");
        var second = roots[0].Children![0].Children![0];
        second.SensitiveType.Should().Be(
            MpStoreCategorySensitiveTypes.QualificationRequired,
            "sensitive_type = 1 ⇒ 创建该类目需添加证件");
    }

    /// <summary>ST-S2：主体审核结果解析（<c>reason</c> 为字符串，标量冲突取示例口径）。</summary>
    [Fact]
    public void MerchantAuditInfoResponse_ShouldParseStringReason()
    {
        const string successJson = """{"errcode":0,"errmsg":"ok","data":{"audit_id":123456,"status":1,"reason":""}}""";
        var success = JsonSerializer.Deserialize(successJson, StoreJsonContext.Default.MpMerchantAuditInfoResponse);

        success!.Data!.Status.Should().Be(MpMerchantAuditStatuses.Approved);
        success.Data.Reason.Should().BeEmpty("官方示例 reason 为字符串空串 ⇒ 按示例口径建模为 string");

        const string rejectedJson = """{"data":{"audit_id":123456,"status":3,"reason":"名称重复"}}""";
        var rejected = JsonSerializer.Deserialize(rejectedJson, StoreJsonContext.Default.MpMerchantAuditInfoResponse);

        rejected!.Data!.Status.Should().Be(MpMerchantAuditStatuses.Rejected);
        rejected.Data.Reason.Should().Be("名称重复");
    }

    /// <summary>ST-S3：省市区二维数组解析（<c>location.lat/lng</c> 为数字，标量冲突取示例口径）。</summary>
    [Fact]
    public void DistrictResponse_ShouldParseTwoDimensionalArray()
    {
        const string json = """
            {"status":0,"message":"ok","data_version":"20200101","result":[[
            {"id":"1","name":"北京市","fullname":"北京市","pinyin":["bei","jing"],
             "location":{"lat":39.90469,"lng":116.40717},"cidx":[2,3]}]]}
            """;

        var response = JsonSerializer.Deserialize(json, StoreJsonContext.Default.MpDistrictResponse);

        response!.Status.Should().Be(0);
        response.IsSuccess.Should().BeTrue("成功响应不带 errcode ⇒ 缺省 0（超集承载）");
        response.DataVersion.Should().Be("20200101");
        response.Result.Should().HaveCount(1, "外层为省 / 市 / 区三级（二维数组）");
        var district = response.Result![0][0];
        district.Id.Should().Be("1");
        district.FullName.Should().Be("北京市");
        district.Pinyin.Should().BeEquivalentTo(new[] { "bei", "jing" });
        district.Location!.Latitude.Should().Be(39.90469);
        district.Location.Longitude.Should().Be(116.40717);
        district.ChildIndexes.Should().BeEquivalentTo(new[] { 2, 3 });
    }

    /// <summary>ST-S4：地图点位搜索响应解析（<c>sosomap_poi_uid</c> 即 add_store 的 map_poi_id 来源）。</summary>
    [Fact]
    public void MapPoiSearchResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","data":{"item":[
            {"branch_name":"门店A","address":"某地址","longitude":113.26627,"latitude":23.13171,
             "telephone":"020-89190388","category":"美食:中餐厅","sosomap_poi_uid":"5938314494307741153",
             "data_supply":1,"pic_urls":["https://x/1.jpg"],"card_id_list":["p123"]}]}}
            """;

        var response = JsonSerializer.Deserialize(json, StoreJsonContext.Default.MpMapPoiSearchResponse);

        response!.Data!.Items.Should().HaveCount(1);
        var item = response.Data.Items![0];
        item.SosoMapPoiUid.Should().Be("5938314494307741153");
        item.Longitude.Should().Be(113.26627, "本页字段表即标 number");
        item.PictureUrls.Should().HaveCount(1);
        item.CardIdList.Should().HaveCount(1);
    }

    /// <summary>ST-S5：新增门店请求体序列化（官方字段名与可空字段省略形态）。</summary>
    [Fact]
    public void AddStoreRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpAddStoreRequest
        {
            MapPoiId = "5938314494307741153",
            PictureList = """{"list":["https://x/1.jpg"]}""",
            ContractPhone = "020-89190388",
            Hour = "11:11-12:12",
            Credential = "1234567890",
            QualificationList = new List<string> { "mediaid1" },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, StoreJsonContext.Default.MpAddStoreRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[]
            {
                "map_poi_id", "pic_list", "contract_phone", "hour", "credential", "qualification_list",
            }, "company_name / card_id / poi_id 未设置时不得输出（官方标非必填）");
        document.RootElement.GetProperty("qualification_list").EnumerateArray().Should().HaveCount(1);
    }

    /// <summary>ST-S6：门店图片 JSON 字符串（官方 <c>pic_list</c> 形态）由 <see cref="MpStorePictureList"/> 生成。</summary>
    [Fact]
    public void StorePictureList_ShouldSerializeToOfficialShape()
    {
        var pictures = new MpStorePictureList { List = new List<string> { "https://x/1.jpg" } };

        var json = JsonSerializer.Serialize(pictures, StoreJsonContext.Default.MpStorePictureList);

        json.Should().Be("""{"list":["https://x/1.jpg"]}""", "官方示例结构为 {\"list\":[图片url]}");
    }

    /// <summary>ST-S7：新增门店响应解析（审核单 id）。</summary>
    [Fact]
    public void AddStoreResponse_ShouldParseAuditId()
    {
        const string json = """{"errcode":0,"errmsg":"ok","data":{"audit_id":123}}""";

        var response = JsonSerializer.Deserialize(json, StoreJsonContext.Default.MpAddStoreResponse);

        response!.Data!.AuditId.Should().Be(123);
    }

    /// <summary>ST-S8：门店详情解析（官方字段表仅列 errcode/errmsg，示例返回完整 business.base_info ⇒ 并集建模）。</summary>
    [Fact]
    public void StoreInfoResponse_ShouldParseBusinessBaseInfo()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","business":{"base_info":{
            "business_name":"龙涤小区熟食店","address":"舍利街道","telephone":"12345678",
            "city":"哈尔滨市","province":"黑龙江省","longitude":126.94355011,"latitude":45.556098938,
            "photo_list":[{"photo_url":"https://x/1.jpg"}],"open_time":"11:00-12:00",
            "poi_id":"472671857","status":2,"district":"value",
            "qualification_num":"91750100ME2XCR6A70","qualification_name":"龙涤小区熟食店"}}}
            """;

        var response = JsonSerializer.Deserialize(json, StoreJsonContext.Default.MpStoreInfoResponse);

        var info = response!.Business!.BaseInfo!;
        info.BusinessName.Should().Be("龙涤小区熟食店");
        info.PoiId.Should().Be("472671857");
        info.Status.Should().Be(MpStoreStatuses.Auditing);
        info.Longitude.Should().Be(126.94355011, "字段表标 string、示例为数字 ⇒ 取示例");
        info.PhotoList.Should().HaveCount(1);
        info.PhotoList![0].PhotoUrl.Should().Be("https://x/1.jpg");
        info.QualificationNumber.Should().Be("91750100ME2XCR6A70");
    }

    /// <summary>ST-S9：门店列表解析（<c>business_list[].base_info</c> 嵌套 + 示例独有 categories / qualification_list）。</summary>
    [Fact]
    public void StoreListResponse_ShouldParseNestedBaseInfo()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","total_count":1,"business_list":[{"base_info":{
            "poi_id":"1","business_name":"门店A","longitude":114.958480835,"latitude":23.1317,
            "province":"广东省","city":"广州市","district":"天河区","address":"某地址",
            "telephone":"010-6666666-111; 010-6666666","photo_list":[{"photo_url":"https://x/1.jpg"}],
            "qualification_name":"门店A","qualification_num":"n1","open_time":"11:00-12:00","status":1},
            "categories":[],"qualification_list":[]}]}
            """;

        var response = JsonSerializer.Deserialize(json, StoreJsonContext.Default.MpStoreListResponse);

        response!.TotalCount.Should().Be(1);
        response.BusinessList.Should().HaveCount(1);
        var item = response.BusinessList![0];
        item.BaseInfo!.BusinessName.Should().Be("门店A", "字段表把子字段列为平级，示例包裹在 base_info 内 ⇒ 取示例");
        item.BaseInfo.Status.Should().Be(MpStoreStatuses.Approved);
        item.BaseInfo.Longitude.Should().Be(114.958480835);
        item.Categories.Should().BeEmpty("示例独有字段（字段表未收录）");
        item.QualificationList.Should().BeEmpty();
    }

    /// <summary>ST-S10：更新门店请求 / 响应（<c>has_audit_id</c> + <c>audit_id</c>）。</summary>
    [Fact]
    public void UpdateStoreRequestAndResponse_ShouldMatchOfficialShape()
    {
        var request = new MpUpdateStoreRequest
        {
            PoiId = "472671857",
            PictureList = """{"list":["https://x/1.jpg"]}""",
            ContractPhone = "020-89190388",
            Hour = "11:11-12:12",
            CardId = "p123",
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, StoreJsonContext.Default.MpUpdateStoreRequest));
        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "poi_id", "pic_list", "contract_phone", "hour", "card_id" },
                "官方 update_store 请求参数表仅此五字段");

        const string json = """{"errcode":0,"errmsg":"ok","data":{"has_audit_id":1,"audit_id":456}}""";
        var response = JsonSerializer.Deserialize(json, StoreJsonContext.Default.MpUpdateStoreResponse);

        response!.Data!.HasAuditId.Should().Be(1, "1 = 需要审核");
        response.Data.AuditId.Should().Be(456);
    }

    /// <summary>ST-S11：地图建店请求 / 响应（<c>introduct</c> 为官方字段原文，不得规范化）。</summary>
    [Fact]
    public void CreateMapPoiRequestAndResponse_ShouldMatchOfficialShape()
    {
        var request = new MpCreateMapPoiRequest
        {
            Name = "门店A",
            Longitude = "113.26627",
            Latitude = "23.13171",
            Province = "广东省",
            City = "广州市",
            District = "天河区",
            Address = "某地址",
            Category = "美食:中餐厅",
            Telephone = "020-89190388",
            Photo = "https://x/1.jpg",
            License = "https://x/2.jpg",
            Introduct = "介绍",
            DistrictId = "440106",
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, StoreJsonContext.Default.MpCreateMapPoiRequest));
        var root = document.RootElement;
        root.EnumerateObject().Select(p => p.Name).Should().Contain("introduct",
            "官方字段原文为 introduct（疑 introduction 拼写）——不得「规范化」为 introduction");
        root.GetProperty("longitude").GetString().Should().Be("113.26627", "本页字段表标 string 且无示例可对照");
        root.EnumerateObject().Select(p => p.Name).Should().Contain("districtid");

        const string json = """{"errcode":0,"errmsg":"ok","data":{"base_id":123,"rich_id":456}}""";
        var response = JsonSerializer.Deserialize(json, StoreJsonContext.Default.MpCreateMapPoiResponse);
        response!.Data!.BaseId.Should().Be(123);
        response.Data.RichId.Should().Be(456);
    }

    /// <summary>ST-S12：主体申请 / 修改的「仅 errcode/errmsg」错误形态复用 <see cref="MpResponse"/>。</summary>
    [Fact]
    public void MerchantRequestErrors_ShouldParseIntoSharedMpResponse()
    {
        const string json = """{"errcode":43104,"errmsg":"this appid does not have permission"}""";

        var response = JsonSerializer.Deserialize(json, CommonJsonContext.Default.MpResponse);

        response!.ErrorCode.Should().Be(MpErrorCodes.StoreAppIdPermissionDenied);
        response.IsSuccess.Should().BeFalse();
    }
}
