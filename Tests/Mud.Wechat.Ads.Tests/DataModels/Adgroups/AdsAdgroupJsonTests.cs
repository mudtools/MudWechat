// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.Tests.DataModels.Adgroups;

/// <summary>
/// 营销单元域 DTO 的<b>源生成上下文</b>序列化测试（守卫是形状权威，本类是运行期证据）。
/// </summary>
/// <remarks>
/// <para>
/// 与 <c>AdsAdvertiserJsonTests</c> 同一取向：一律走 <see cref="AdgroupsJsonContext"/> 的类型信息而非
/// 反射重载 —— 反射版在 Native AOT 下没有元数据，用它测试等于「JIT 下绿、AOT 下炸」（守卫 ADS-B6）。
/// 本类因此同时是「生成物是否真的覆盖到闭合应答类型」的运行期证据：生成器漏类型时
/// <c>AdgroupsJsonContext.Default.AdsAdgroupGetResponse</c> 直接编译不过或抛。
/// </para>
/// <para>
/// <b>报文样例的来源与一处官方缺陷</b>：2026-10-10 逐页核验时，<c>adgroups/get</c> 页的应答示例恰为
/// <c>{"code":0,"message":"","message_cn":""}</c> —— 整个 <c>data</c> 缺席，而应答字段表有近 80 个顶层字段。
/// 故本类的样例是<b>按字段表与其 DOM 层级构造</b>（不是照抄示例），层级归属由守卫
/// <c>AdgroupsFieldLevels_ShouldMatchVerifiedDocumentHierarchy</c> 锁定，本类负责证明「按该层级写的报文真能绑上」。
/// </para>
/// </remarks>
public class AdsAdgroupJsonTests
{
    /// <summary>
    /// <b>三处「平面读法会读错层级」的字段的绑定证据</b>：
    /// <c>targeting_translation</c> 与 <c>targeting</c> 平级、<c>poi_list</c> 与
    /// <c>marketing_asset_outer_spec</c> 平级、<c>prospect_retargeting</c> 在
    /// <c>industry_value_explore</c> <b>之内</b>（守卫已锁层级，这里锁「按该层级能绑上」）。
    /// </summary>
    [Fact]
    public void AdgroupGetResponse_ShouldBindTopLevelFields_AtTheirVerifiedLevel()
    {
        const string json = """
        {
          "code": 0,
          "message": "",
          "message_cn": "",
          "data": {
            "list": [
              {
                "adgroup_id": 3000000001,
                "adgroup_name": "示例营销单元",
                "marketing_goal": "MARKETING_GOAL_PRODUCT_PROMOTION",
                "marketing_asset_outer_spec": {
                  "marketing_target_type": "MARKETING_TARGET_TYPE_COMMODITY",
                  "marketing_asset_outer_id": "outer-9001"
                },
                "poi_list": ["poi-1", "poi-2"],
                "targeting": {
                  "gender": ["MALE"],
                  "age": [ { "min": 18, "max": 45 } ]
                },
                "targeting_translation": "男性，18-45 岁",
                "scene_spec": {
                  "wechat_scene": { "pay_scene": [22] },
                  "wechat_position": [1001]
                },
                "industry_value_explore": {
                  "iaa_smart_hosting": true,
                  "high_volume_exploration": false,
                  "prospect_retargeting": { "enabled": true, "bid_coefficient": 1.2 }
                },
                "configured_status": "AD_STATUS_NORMAL",
                "daily_budget": 300000,
                "created_time": 1730000000
              }
            ]
          }
        }
        """;

        var response = JsonSerializer.Deserialize(json, AdgroupsJsonContext.Default.AdsAdgroupGetResponse);

        response!.IsSuccess.Should().BeTrue();
        var info = response.Data!.List!.Single();

        info.AdgroupId.Should().Be(3000000001L);
        info.PoiList.Should().Equal(new[] { "poi-1", "poi-2" },
            "poi_list 是 list[] 元素的顶层字段（平面读法会把它挂进 marketing_asset_outer_spec）");
        info.MarketingAssetOuterSpec!.MarketingAssetOuterId.Should().Be("outer-9001");
        info.Targeting!.Age.Should().ContainSingle();
        info.Targeting.Age![0].Max.Should().Be(45);
        info.TargetingTranslation.Should().Be("男性，18-45 岁",
            "与 targeting 平级、由官方生成的只读文案（挂进 targeting 内即静默绑不上）");

        // wechat_scene 是 struct，而 wechat_position 与它平级（平面读法会把后者误挂进 wechat_scene）。
        info.SceneSpec!.WechatScene!.PayScene.Should().Equal(new long[] { 22L });
        info.SceneSpec.WechatPosition.Should().Equal(new long[] { 1001L });

        var explore = info.IndustryValueExplore!;
        explore.IaaSmartHosting.Should().BeTrue();
        explore.HighVolumeExploration.Should().BeFalse();
        explore.ProspectRetargeting!.Enabled.Should().BeTrue();
        explore.ProspectRetargeting.BidCoefficient.Should().Be(1.2,
            "prospect_retargeting 确在 industry_value_explore 之内（平面读法会把它提到顶层）");

        info.ConfiguredStatus.Should().Be("AD_STATUS_NORMAL");
        info.DailyBudget.Should().Be(300000);
        info.CreatedTime.Should().Be(1730000000);
    }

    /// <summary>
    /// 两种翻页模式<b>互斥返回</b>，且游标形态与 <c>advertiser/get</c> 不同：
    /// 本页用 <c>next_cursor</c> / <c>previous_cursor</c> 两支 <c>string</c>，请求侧回填的却是
    /// <c>cursor</c> 参数 —— <b>两侧不同名</b>，收敛成一支公共类型即失真。
    /// </summary>
    [Fact]
    public void AdgroupGetResponse_ShouldBindCursorPageInfo_WithStringCursors()
    {
        const string cursorJson = """
        {
          "code": 0, "message": "", "message_cn": "",
          "data": {
            "list": [ { "adgroup_id": 1, "adgroup_name": "n-1" } ],
            "cursor_page_info": {
              "page_size": 100, "total_number": 356,
              "next_cursor": "MTAwMjM0NTY3OA", "previous_cursor": "MTAwMDEyMzQ1Ng"
            }
          }
        }
        """;

        var response = JsonSerializer.Deserialize(cursorJson, AdgroupsJsonContext.Default.AdsAdgroupGetResponse);

        var cursor = response!.Data!.CursorPageInfo!;
        cursor.PageSize.Should().Be(100);
        cursor.TotalNumber.Should().Be(356);
        cursor.NextCursor.Should().Be("MTAwMjM0NTY3OA",
            "游标是字符串（官方 0–10 字节的编码值），不得按 advertiser/get 的 integer cursor 建模");
        cursor.PreviousCursor.Should().Be("MTAwMDEyMzQ1Ng");
        response.Data.PageInfo.Should().BeNull("游标模式下官方不返回 page_info");

        const string normalJson = """
        {
          "code": 0, "message": "", "message_cn": "",
          "data": {
            "list": [],
            "page_info": { "page": 1, "page_size": 10, "total_number": 0, "total_page": 0 }
          }
        }
        """;

        var normal = JsonSerializer.Deserialize(normalJson, AdgroupsJsonContext.Default.AdsAdgroupGetResponse);
        normal!.Data!.PageInfo!.TotalPage.Should().Be(0);
        normal.Data!.CursorPageInfo.Should().BeNull("普通模式下官方不返回 cursor_page_info");
    }

    /// <summary>
    /// <c>fields</c> 裁剪后未请求的字段在应答里为 <c>null</c> <b>属正常形态</b>（官方字段表标可空），
    /// 而上下文又设了 <c>WhenWritingNull</c> ⇒「显式 null」与「字段缺席」在线上不可区分。
    /// 本用例固定「只请求两支字段」的报文能正常绑定、其余全为 null 且判错不受影响。
    /// </summary>
    [Fact]
    public void AdgroupGetResponse_ShouldLeaveUnrequestedFieldsNull_WhenFieldsAreTrimmed()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{"list":[{"adgroup_id":7,"adgroup_name":"仅两支字段"}]}}
        """;

        var response = JsonSerializer.Deserialize(json, AdgroupsJsonContext.Default.AdsAdgroupGetResponse);

        var info = response!.Data!.List!.Single();
        info.AdgroupId.Should().Be(7);
        info.Targeting.Should().BeNull("未列入 fields 的字段官方不返回，属正常而非缺陷");
        info.DailyBudget.Should().BeNull();
        info.PoiList.Should().BeNull();
        response.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// 创建请求体的键名必须逐支等于官方 snake_case 原文，且<b>未赋值字段不写出</b>
    /// （官方 162 行参数表里绝大多数是条件必填，本地按 <c>*</c> 拦截会误伤）。
    /// </summary>
    [Fact]
    public void AdgroupAddRequest_ShouldSerializeOfficialRequiredSubset()
    {
        var request = new AdsAdgroupAddRequest
        {
            AccountId = 12345678,
            AdgroupName = "示例营销单元",
            MarketingGoal = "MARKETING_GOAL_PRODUCT_PROMOTION",
            MarketingCarrierType = "MARKETING_CARRIER_TYPE_APP_ANDROID",
            BeginDate = "2026-10-11",
            EndDate = "2026-10-31",
            TimeSeries = new string('1', 336),
            DailyBudget = 300000,
            Targeting = new AdsAdgroupTargeting
            {
                Gender = new List<string> { "MALE" },
                Age = new List<AdsAgeRange> { new() { Min = 18, Max = 45 } },
            },
        };

        var json = JsonSerializer.Serialize(request, AdgroupsJsonContext.Default.AdsAdgroupAddRequest);

        json.Should().Contain("\"account_id\":12345678");
        json.Should().Contain("\"marketing_goal\":\"MARKETING_GOAL_PRODUCT_PROMOTION\"");
        json.Should().Contain("\"marketing_carrier_type\":\"MARKETING_CARRIER_TYPE_APP_ANDROID\"");
        json.Should().Contain("\"begin_date\":\"2026-10-11\"");
        json.Should().Contain("\"end_date\":\"2026-10-31\"");
        json.Should().Contain("\"daily_budget\":300000");
        json.Should().Contain("\"targeting\":{\"gender\":[\"MALE\"],\"age\":[{\"min\":18,\"max\":45}]}");

        json.Should().NotContain("adgroup_id", "add 由官方分配 id，请求体不得带该字段");
        json.Should().NotContain("configuredStatus");
        json.Should().NotContain("beginDate", "camelCase 支出现即说明显式 [JsonPropertyName] 被删");

        // 中文值走的是默认编码器的 \uXXXX 转义形态（JSON 语义等价、官方按解码值处理），
        // 故断言取「往返还原」而非「逐字符等于原文」—— 后者会把测试绑在转义形态上。
        var roundTripped = JsonSerializer.Deserialize(json, AdgroupsJsonContext.Default.AdsAdgroupAddRequest);
        roundTripped!.AdgroupName.Should().Be("示例营销单元",
            "非 ASCII 值必须能原样往返（转义只是线形态，不是数据变更）");
        json.Should().Contain(@"\u793A", "默认编码器把非 ASCII 转义为 \\uXXXX —— 记为已知线形态，勿误判为字段丢失");
    }

    /// <summary>
    /// 批量族报文数组名取官方<b>请求参数表</b>的 <c>update_datetime_spec</c>，
    /// 不跟官方使用说明处的笔误 <c>update_date_spec</c>；元素内三字段全为选填（条件必填由官方判）。
    /// </summary>
    [Fact]
    public void AdgroupUpdateDatetimeRequest_ShouldUseRequestTableArrayName()
    {
        var request = new AdsAdgroupUpdateDatetimeRequest
        {
            AccountId = 12345678,
            UpdateDatetimeSpec = new List<AdsAdgroupUpdateDatetimeSpec>
            {
                new() { AdgroupId = 11, EndDate = "2026-11-30" },
                new() { AdgroupId = 22, TimeSeries = new string('1', 336) },
            },
        };

        var json = JsonSerializer.Serialize(
            request, AdgroupsJsonContext.Default.AdsAdgroupUpdateDatetimeRequest);

        json.Should().Be(
            "{\"account_id\":12345678,\"update_datetime_spec\":[" +
            "{\"adgroup_id\":11,\"end_date\":\"2026-11-30\"}," +
            "{\"adgroup_id\":22,\"time_series\":\"" + new string('1', 336) + "\"}]}");

        json.Should().NotContain("update_date_spec", "官方使用说明处误写，报文权威是请求参数表");
    }

    /// <summary>
    /// 四支批量端点<b>共用</b>一支载荷，判错必须两层都做：外层 <c>code == 0</c> 只表示受理，
    /// 逐条成败看 <c>data.list[i].code</c>，<c>fail_id_list</c> 是第三个汇总面。
    /// </summary>
    [Fact]
    public void AdgroupBatchResponse_ShouldExposePerItemCodeAndFailIdList()
    {
        const string json = """
        {
          "code": 0, "message": "", "message_cn": "",
          "data": {
            "list": [
              { "code": 0, "message": "", "message_cn": "", "adgroup_id": 11 },
              { "code": 12345, "message": "bid amount out of range", "message_cn": "出价超出区间", "adgroup_id": 22 }
            ],
            "fail_id_list": [22]
          }
        }
        """;

        var response = JsonSerializer.Deserialize(json,
            AdgroupsJsonContext.Default.AdsAdgroupUpdateBidAmountResponse);

        response!.IsSuccess.Should().BeTrue("外层只表达「请求被受理」，半数失败时外层仍是 0");

        var items = response.Data!.List!;
        items.Should().HaveCount(2, "官方保证返回顺序与请求数组一致 ⇒ 可按索引配对");
        items[0].Code.Should().Be(0);
        items[0].AdgroupId.Should().Be(11);
        items[1].Code.Should().Be(12345);
        items[1].MessageCn.Should().Be("出价超出区间");

        response.Data.FailIdList.Should().Equal(new long[] { 22L });

        // 同一份载荷被四支端点复用：逐支都能绑定（守卫锁类型同一性，本句锁运行期可达）。
        var status = JsonSerializer.Deserialize(json,
            AdgroupsJsonContext.Default.AdsAdgroupUpdateConfiguredStatusResponse);
        status!.Data!.FailIdList.Should().Equal(new long[] { 22L });
    }

    /// <summary>删除成功时官方 <c>data</c> 只回被删的 <c>adgroup_id</c>（与 add / update 共用一支载荷）。</summary>
    [Fact]
    public void AdgroupDeleteResponse_ShouldBindAdgroupIdOnly()
    {
        const string json = """
        {"code":0,"message":"","message_cn":"","data":{"adgroup_id":3000000001}}
        """;

        var response = JsonSerializer.Deserialize(json, AdgroupsJsonContext.Default.AdsAdgroupDeleteResponse);

        response!.IsSuccess.Should().BeTrue();
        response.Data!.AdgroupId.Should().Be(3000000001L);
    }

    /// <summary>
    /// 失败信封必须能落到公共判错契约（<c>code</c> → <see cref="Mud.Wechat.Ads.DataModels.Common.AdsResponse.ErrorCode"/>），
    /// 且 <c>message_cn</c> <b>不顶替</b>英文支 —— 官方 message 才是与错误码对齐的权威描述。
    /// </summary>
    [Fact]
    public void AdgroupResponse_ShouldMapEnvelopeToCommonContract_WhenCodeIsNotZero()
    {
        const string json = """
        {"code":11001,"message":"invalid user_token","message_cn":"实名认证令牌无效","data":null}
        """;

        var response = JsonSerializer.Deserialize(json, AdgroupsJsonContext.Default.AdsAdgroupAddResponse);

        response!.IsSuccess.Should().BeFalse();
        response.ErrorCode.Should().Be(11001);
        response.ErrorMessage.Should().Be("invalid user_token");
        response.MessageCn.Should().Be("实名认证令牌无效");
        response.Data.Should().BeNull();
    }
}
