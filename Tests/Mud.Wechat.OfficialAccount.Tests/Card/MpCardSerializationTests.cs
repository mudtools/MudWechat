// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Card;

namespace Mud.Wechat.OfficialAccount.Tests.Card;

/// <summary>
/// 卡券域 DTO 与官方报文的双向映射（报文形态取自对齐基准 SKIT 的请求/响应结构）。
/// </summary>
/// <remarks>
/// 与 <c>MpCardContractGuards</c> 的分工：守卫锁「字段名与形态裁决」，本测试锁「源生成上下文实际写出的报文」
/// —— 两者互为交叉验证（字段名断言改错、或 DTO 与上下文脱节，必有一侧打红）。
/// </remarks>
public class MpCardSerializationTests
{
    /// <summary>CD-S1：建卡请求为 <c>card</c> 包装 + <c>card_type</c> 判别，未选分支不输出（分支互斥）。</summary>
    [Fact]
    public void CreateRequest_ShouldSerializeWrappedCardWithSingleBranch()
    {
        var request = new MpCardCreateRequest
        {
            Card = new MpCardCreateBody
            {
                CardType = "CASH",
                Cash = new MpCardCreateCash
                {
                    LeastCost = 1000,
                    ReduceCost = 500,
                    BaseInfo = new MpCardCreateBaseInfo
                    {
                        LogoUrl = "http://mmbiz.qpic.cn/LOGO/",
                        CodeType = "CODE_TYPE_TEXT",
                        BrandName = "品牌名",
                        Title = "十元代金券",
                        Color = "Color020",
                        Notice = "使用时请出示给店员",
                        Description = "使用说明",
                        Sku = new MpCardCreateSku { Quantity = 1000000 },
                        DateInfo = new MpCardDateInfo { Type = "FIXED_TIME", BeginTimestamp = 1430507643, EndTimestamp = 1745886843 },
                    },
                },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CardJsonContext.Default.MpCardCreateRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "card" }, "建卡请求体顶层只有 card");
        var card = document.RootElement.GetProperty("card");
        card.GetProperty("card_type").GetString().Should().Be("CASH");
        card.GetProperty("cash").GetProperty("least_cost").GetInt32().Should().Be(1000);
        card.GetProperty("cash").GetProperty("reduce_cost").GetInt32().Should().Be(500);
        card.GetProperty("cash").GetProperty("base_info").GetProperty("brand_name").GetString().Should().Be("品牌名");
        card.GetProperty("cash").GetProperty("base_info").GetProperty("sku").GetProperty("quantity")
            .GetInt32().Should().Be(1000000, "建卡方向的 sku 只有 quantity（剩余量属查询方向）");

        // 分支互斥：只传 cash 时，其余 10 支不得出现在报文里（WhenWritingNull 生效）。
        card.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "card_type", "cash" });
    }

    /// <summary>CD-S2：修改请求为 <c>card_id</c> 与分支<b>平级</b>，且写不出 card_type / advanced_info。</summary>
    [Fact]
    public void UpdateRequest_ShouldSerializeFlatShapeWithoutAdvancedInfo()
    {
        var request = new MpCardUpdateRequest
        {
            CardId = "card_id",
            Cash = new MpCardUpdateCash { LeastCost = 2000, BaseInfo = new MpCardUpdateBaseInfo { Title = "五元代金券" } },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CardJsonContext.Default.MpCardUpdateRequest));

        var names = document.RootElement.EnumerateObject().Select(p => p.Name).ToList();
        names.Should().Contain(new[] { "card_id", "cash" });
        names.Should().NotContain("card_type", "修改方向无分支判别键");
        names.Should().NotContain("card", "修改方向无 card 包装");
        document.RootElement.GetProperty("cash").EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "base_info", "least_cost" }, "修改分支无 advanced_info（高级信息建卡后不可改）");
    }

    /// <summary>CD-S3：自助核销开关写出官方键名 <c>need_verify_cod</c>（缺尾字母 e，照录不修正）。</summary>
    [Fact]
    public void SelfConsumeCellRequest_ShouldWriteVerifiedCodeKeyAsOfficialSpelling()
    {
        var request = new MpCardSelfConsumeCellSetRequest
        {
            CardId = "card_id",
            IsOpen = true,
            NeedVerifyCode = true,
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CardJsonContext.Default.MpCardSelfConsumeCellSetRequest));

        document.RootElement.GetProperty("need_verify_cod").GetBoolean().Should().BeTrue();
        document.RootElement.TryGetProperty("need_verify_code", out _).Should().BeFalse(
            "不得擅自补全字母 e（该拼写待官方逐页核验，补 e 即改变契约名）");
        document.RootElement.TryGetProperty("need_remark_amount", out _).Should().BeFalse("未传即不输出");
    }

    /// <summary>CD-S4：库存请求为增量语义（键名 <c>*_stock_value</c>，非 quantity）。</summary>
    [Fact]
    public void ModifyStockRequest_ShouldWriteIncrementKeys()
    {
        var request = new MpCardModifyStockRequest { CardId = "card_id", IncreaseStockValue = 100 };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CardJsonContext.Default.MpCardModifyStockRequest));

        document.RootElement.EnumerateObject().Select(p => p.Name)
            .Should().BeEquivalentTo(new[] { "card_id", "increase_stock_value" });
        document.RootElement.GetProperty("increase_stock_value").GetInt32().Should().Be(100);
    }

    /// <summary>CD-S5：查询应答解析（会员卡分支 + base_info 的官方生成字段）。</summary>
    [Fact]
    public void GetResponse_ShouldParseMemberCardBranchWithGeneratedFields()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","card":{"card_type":"MEMBER_CARD","member_card":{
            "base_info":{"id":"card_id","status":"CARD_STATUS_REVIEW","title":"会员卡","sku":{"quantity":80,"total_quantity":100},
            "create_time":1442468956,"update_time":1442468957,"need_push_on_view":true},
            "bind_old_card_url":"http://.qq.com"}}}
            """;

        var response = JsonSerializer.Deserialize(json, CardJsonContext.Default.MpCardGetResponse);

        response!.IsSuccess.Should().BeTrue();
        var member = response.Card!.MemberCard!;
        member.BaseInfo!.Id.Should().Be("card_id");
        member.BaseInfo.Status.Should().Be("CARD_STATUS_REVIEW", "状态为字符串，SDK 不建枚举、不做状态机校验");
        member.BaseInfo.Sku!.TotalQuantity.Should().Be(100, "查询方向的 sku 额外回传总量");
        member.BaseInfo.CreateTime.Should().Be(1442468956);
        member.BaseInfo.NeedPushOnView.Should().BeTrue();
        member.BindOldCardUrl.Should().Be("http://.qq.com", "bind_old_card_url 属查询方向独有");
    }

    /// <summary>CD-S6：券码应答解析（顶层会员视角与 card 子对象券码视角的 bonus/balance 两层并存）。</summary>
    [Fact]
    public void CodeGetResponse_ShouldParseTwoLayerBonusAndBalance()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","card":{"card_id":"card_id","code":"12345678901234","begin_time":1442468956,
            "end_time":1745886843,"bonus":100,"balance":2000},
            "user_card_status":"USER_CARD_STATUS_NORMAL","openid":"oABC123","nickname":"昵称",
            "membership_number":"0001","bonus":300,"balance":5000,"order_id":"12345","can_consume":true,
            "user_info":{"common_field_list":[{"name":"FIELD0_1","value":"v1"}],"custom_field_list":[]}}
            """;

        var response = JsonSerializer.Deserialize(json, CardJsonContext.Default.MpCardCodeGetResponse);

        response!.CardStatus.Should().Be("USER_CARD_STATUS_NORMAL", "官方键名为 user_card_status");
        response.MemberBonus.Should().Be(300, "顶层 bonus 属会员视角");
        response.MemberBalance.Should().Be(5000);
        response.Card!.Bonus.Should().Be(100, "card.bonus 属券码视角，与顶层不同源");
        response.Card.Balance.Should().Be(2000);
        response.Card.BeginTime.Should().Be(1442468956);
        response.UserInfo!.CommonFieldList.Should().HaveCount(1);
        response.UserInfo.CommonFieldList![0].Name.Should().Be("FIELD0_1");
        response.CanConsume.Should().BeTrue();
    }

    /// <summary>CD-S7：礼品卡分支（官方键名 <c>general_card</c>，<c>gift</c> 是兑换券而非礼品卡）。</summary>
    [Fact]
    public void GiftCardBranch_ShouldSerializeUnderGeneralCardKey()
    {
        var request = new MpCardCreateRequest
        {
            Card = new MpCardCreateBody
            {
                CardType = "GENERAL_CARD",
                GeneralCard = new MpCardCreateGeneralCard
                {
                    SubCardType = "GIFT_CARD_TYPE_PHYSICAL",
                    Prerogative = "权益说明",
                    SupplyBonus = true,
                    InitBalance = 10000,
                    BaseInfo = new MpCardGiftCardBaseInfo
                    {
                        MaxGiveFriendTimes = 5,
                        GiftCardInfo = new MpCardGiftCardPriceInfo { Price = 10000 },
                    },
                },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, CardJsonContext.Default.MpCardCreateRequest));

        var card = document.RootElement.GetProperty("card");
        card.TryGetProperty("gift", out _).Should().BeFalse("gift 分支未传 ⇒ 不得出现（gift 指兑换券，与礼品卡不是一回事）");
        var general = card.GetProperty("general_card");
        general.GetProperty("sub_card_type").GetString().Should().Be("GIFT_CARD_TYPE_PHYSICAL");
        general.GetProperty("supply_bonus").GetBoolean().Should().BeTrue();
        general.GetProperty("init_balance").GetInt32().Should().Be(10000);
        general.GetProperty("base_info").GetProperty("giftcard_info").GetProperty("price").GetInt32().Should().Be(10000);
        general.GetProperty("base_info").GetProperty("max_give_friend_times").GetInt32().Should().Be(5);
    }
}
