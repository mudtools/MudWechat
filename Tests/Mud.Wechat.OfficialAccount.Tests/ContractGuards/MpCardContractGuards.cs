// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Card;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 公众号「卡券」域契约守卫（<c>/card/*</c> 双接口共 14 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>对齐基准与核验状态（勿弱化）</b>：官方文档站正文为 SPA，本批实施期（2026-10-10）不可达 ⇒
/// 路由与字段面以本地 SKIT 源码 <c>SKIT.FlurlHttpClient.Wechat.Api/Models/Card/</c> 为对齐依据。
/// 因此本守卫<b>只锁「已对齐部分」</b>：路由、分支归属、官方字段名拼写、三向形态差异；
/// 频率上限、字段长度、账号门槛、错误码表、取值枚举一律属「待官方逐页核验」，恢复可达后须逐页补齐断言。
/// </para>
/// <para>
/// <b>守卫锁定的形态裁决</b>（勿「顺手修正」）：建卡 / 查询 / 修改三张 <c>base_info</c> 分建不合并（CD2）；
/// 官方键名照原文不驼峰化（CD3）；建卡 <c>card</c> 包装 vs 修改平级不同构（CD4）；
/// 查询方向无 <c>general_card</c>（CD5）；<c>need_verify_cod</c> 缺尾字母照录（CD6）；
/// 未建模 10 族 39 端点以「零路由」负向断言留档（CD8）。
/// </para>
/// </remarks>
public class MpCardContractGuards
{
    private const string RegistryGroupName = "Card";
    private const string CardNamespace = "Mud.Wechat.OfficialAccount.DataModels.Card";

    /// <summary>官方路由表（14 端点，全 POST，前缀统一 <c>/card/</c>）。</summary>
    private static readonly (Type Interface, string Route, string Method)[] Routes =
    {
        (typeof(IMpCardService), "/card/create", nameof(IMpCardService.CreateCardAsync)),
        (typeof(IMpCardService), "/card/get", nameof(IMpCardService.GetCardAsync)),
        (typeof(IMpCardService), "/card/batchget", nameof(IMpCardService.BatchGetCardsAsync)),
        (typeof(IMpCardService), "/card/update", nameof(IMpCardService.UpdateCardAsync)),
        (typeof(IMpCardService), "/card/modifystock", nameof(IMpCardService.ModifyCardStockAsync)),
        (typeof(IMpCardService), "/card/delete", nameof(IMpCardService.DeleteCardAsync)),
        (typeof(IMpCardService), "/card/qrcode/create", nameof(IMpCardService.CreateCardQrCodeAsync)),
        (typeof(IMpCardService), "/card/landingpage/create", nameof(IMpCardService.CreateCardLandingPageAsync)),
        (typeof(IMpCardService), "/card/paycell/set", nameof(IMpCardService.SetPayCellAsync)),
        (typeof(IMpCardService), "/card/selfconsumecell/set", nameof(IMpCardService.SetSelfConsumeCellAsync)),
        (typeof(IMpCardService), "/card/testwhitelist/set", nameof(IMpCardService.SetTestWhiteListAsync)),
        (typeof(IMpCardCodeService), "/card/code/consume", nameof(IMpCardCodeService.ConsumeCardCodeAsync)),
        (typeof(IMpCardCodeService), "/card/code/get", nameof(IMpCardCodeService.GetCardCodeAsync)),
        (typeof(IMpCardCodeService), "/card/code/decrypt", nameof(IMpCardCodeService.DecryptCardCodeAsync)),
    };

    /// <summary>建卡方向的 11 个券型分支官方键名（<c>card_type</c> 判别，分支互斥）。</summary>
    private static readonly string[] CreateBranchKeys =
    {
        "groupon", "cash", "discount", "gift", "general_coupon", "general_card",
        "member_card", "meeting_ticket", "scenic_ticket", "movie_ticket", "boarding_pass",
    };

    /// <summary>
    /// 契约守卫 CD1：14 端点路由与方法一致（全 POST、同前缀、请求体形态、令牌绑定）。
    /// </summary>
    [Fact]
    public void CardEndpoints_ShouldMatchAlignedOfficialRoutes()
    {
        Routes.Select(r => r.Route).Should().OnlyHaveUniqueItems("同一路由不得重复声明");
        Routes.Should().HaveCount(14);
        Routes.Should().OnlyContain(r => r.Route.StartsWith("/card/", StringComparison.Ordinal),
            "本域 14 端点同前缀 /card/（带参二维码域用 /cgi-bin/qrcode/create，两域路由不重叠）");

        foreach (var (iface, route, method) in Routes)
        {
            var target = FindMethod(iface, method);
            var attr = target.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与对齐基准一致");
            target.GetParameters().Any(p => p.GetCustomAttribute<BodyAttribute>() != null)
                .Should().BeTrue($"{method} 为 POST + JSON 请求体");
        }

        // 14 端点全部为 POST：本域无 GET 端点（协议查询 GET /card/getapplyprotocol 属未建模族）。
        foreach (var iface in new[] { typeof(IMpCardService), typeof(IMpCardCodeService) })
        {
            iface.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
                .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()!)
                .Should().AllBeAssignableTo<PostAttribute>($"{iface.Name} 官方全为 POST");
        }

        // 双接口同注册组（照 openApi 先例）+ 令牌绑定：Query 注入 access_token。
        foreach (var iface in new[] { typeof(IMpCardService), typeof(IMpCardCodeService) })
        {
            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull();
            api!.RegistryGroupName.Should().Be(RegistryGroupName);

            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.Name.Should().Be("access_token");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query);
            token.TokenType.Should().Be(MpTokenTypes.AccessToken);
        }
    }

    /// <summary>
    /// 契约守卫 CD2：<c>base_info</c> 三形态（建卡 / 查询 / 修改）<b>分建不合并</b>。
    /// </summary>
    [Fact]
    public void BaseInfoThreeDirections_ShouldStaySeparated()
    {
        var create = JsonNamesOf(typeof(MpCardCreateBaseInfo));
        var query = JsonNamesOf(typeof(MpCardQueryBaseInfo));
        var update = JsonNamesOf(typeof(MpCardUpdateBaseInfo));

        // 建卡方向独有：子商户归属 + 券码类型族（建卡后不可改）。
        create.Should().Contain("sub_merchant_info", "官方键名即 sub_merchant_info，非 sub_merchant");
        create.Should().Contain("sku");
        create.Should().Contain("use_custom_code");
        create.Should().Contain("get_custom_code_mode");
        create.Should().Contain("bind_openid");

        // 查询方向独有：模板号与状态由官方生成 ⇒ 建卡方向不得携带。
        query.Should().Contain(new[] { "id", "status", "create_time", "update_time" },
            "查询应答携带官方生成的模板号/状态/时间戳");
        create.Should().NotContain("id", "建卡时模板号由官方生成 ⇒ 请求体不得携带");
        create.Should().NotContain("status");
        create.Should().NotContain("create_time");
        create.Should().NotContain("update_time");
        query.Should().NotContain("sub_merchant_info", "查询应答不回传子商户归属");

        // 修改方向：券码与库存形态建卡后不可改 ⇒ 一律不提供（库存走 /card/modifystock）。
        update.Should().NotContain("sku", "库存调整走独立端点，修改方向无 sku");
        update.Should().NotContain("sub_merchant_info");
        update.Should().NotContain("use_custom_code");
        update.Should().NotContain("get_custom_code_mode");
        update.Should().NotContain("bind_openid");

        // 小程序入口六字段：建卡与修改有、查询应答无（方向不可逆）。
        CountAppBrand(create).Should().Be(6, "center/custom/promotion 各 user_name + pass");
        CountAppBrand(update).Should().Be(6);
        CountAppBrand(query).Should().Be(0, "查询应答不回传 *_app_brand_* 组");

        // sku 形态按方向分建：建卡只有总量，查询额外回传剩余量。
        JsonNamesOf(typeof(MpCardCreateSku)).Should().BeEquivalentTo(new[] { "quantity" });
        JsonNamesOf(typeof(MpCardQuerySku)).Should().BeEquivalentTo(new[] { "quantity", "total_quantity" });

        // 三型各自独立（防「为省类型而合并」）。
        typeof(MpCardCreateBaseInfo).Should().NotBeSameAs(typeof(MpCardQueryBaseInfo));
        typeof(MpCardUpdateBaseInfo).Should().NotBeSameAs(typeof(MpCardQueryBaseInfo));

        // 券型专用 base_info 走「继承而非重复声明」：只增字段、不减。
        typeof(MpCardGiftCardBaseInfo).Should().BeDerivedFrom(typeof(MpCardCreateBaseInfo));
        typeof(MpCardMemberCardBaseInfo).Should().BeDerivedFrom(typeof(MpCardCreateBaseInfo));
        typeof(MpCardQueryMemberCardBaseInfo).Should().BeDerivedFrom(typeof(MpCardQueryBaseInfo));
        typeof(MpCardUpdateGiftCardBaseInfo).Should().BeDerivedFrom(typeof(MpCardUpdateBaseInfo));
        typeof(MpCardUpdateMemberCardBaseInfo).Should().BeDerivedFrom(typeof(MpCardUpdateBaseInfo));
        JsonNamesOf(typeof(MpCardGiftCardBaseInfo)).Should().Contain(new[]
        {
            "giftcard_info", "max_give_friend_times", "need_push_on_view",
        }, "礼品卡形态在通用形态之上只增三字段");
    }

    /// <summary>契约守卫 CD3：官方字段名逐类型锁定（<b>照官方原文，不驼峰化、不「修正拼写」</b>）。</summary>
    [Fact]
    public void CardShapes_ShouldLockOfficialFieldNames()
    {
        // 建卡顶层与分支判别。
        AssertJsonProperty<MpCardCreateRequest>("card", "顶层唯一对象");
        AssertJsonProperty<MpCardCreateBody>("card_type", "分支判别键");
        AssertJsonProperty<MpCardCreateResponse>("card_id", "建卡应答唯一业务字段");

        // 券型分支内字段：易错点集中处（照官方键名，勿凭常识改写）。
        AssertJsonProperty<MpCardCreateGroupon>("deal_detail", "团购详情");
        AssertJsonProperty<MpCardCreateCash>("least_cost", "起用金额（分）");
        AssertJsonProperty<MpCardCreateCash>("reduce_cost", "减免金额（分）");
        AssertJsonProperty<MpCardCreateDiscount>("discount", "打折额度");
        AssertJsonProperty<MpCardCreateGift>("gift", "兑换内容（gift 分支即「兑换券」，非礼品卡）");
        AssertJsonProperty<MpCardCreateGeneralCoupon>("default_detail", "官方键名为 default_detail，非 general_coupon_detail");
        AssertJsonProperty<MpCardCreateGeneralCard>("sub_card_type", "礼品卡子类型");
        AssertJsonProperty<MpCardCreateGeneralCard>("auto_activate", "领取后自动激活");
        AssertJsonProperty<MpCardCreateGeneralCard>("init_balance", "初始余额（分）");
        AssertJsonProperty<MpCardCreateMemberCard>("wx_activate_after_submit_url", "跳转型一键激活的跳转外链");
        AssertJsonProperty<MpCardCreateMemberCard>("activate_app_brand_pass", "激活小程序页面路径");
        AssertJsonProperty<MpCardCreateMeetingTicket>("meeting_detail", "会议详情");
        AssertJsonProperty<MpCardCreateScenicTicket>("ticket_class", "票种");
        AssertJsonProperty<MpCardCreateMovieTicket>("detail", "影片详情，官方键名为 detail 而非 movie_detail");
        AssertJsonProperty<MpCardCreateBoardingPass>("air_model", "机型");
        AssertJsonProperty<MpCardCreateBoardingPass>("check_in_url", "在线值机外链");

        // 通用 base_info 与共用子表。
        AssertJsonProperty<MpCardCreateBaseInfo>("location_id_list", "门店 id 列表");
        AssertJsonProperty<MpCardCreateBaseInfo>("get_limit", "每人可领券数上限");
        AssertJsonProperty<MpCardCreateBaseInfo>("can_give_friend", "可否转赠好友");
        AssertJsonProperty<MpCardCreateBaseInfo>("source", "第三方来源名");
        AssertJsonProperty<MpCardAdvancedInfo>("abstract", "封面摘要，官方键名即为 abstract");
        AssertJsonProperty<MpCardAdvancedInfo>("text_image_list", "图文列表");
        AssertJsonProperty<MpCardAdvancedInfo>("time_limit", "使用时段限制");
        AssertJsonProperty<MpCardAbstractInfo>("icon_url_list", "摘要图标列表");
        AssertJsonProperty<MpCardDateInfo>("fixed_begin_term", "固定有效期起算日");
        AssertJsonProperty<MpCardSubMerchantInfo>("merchant_id", "微信支付商户号（只透传，不调用支付接口）");
        AssertJsonProperty<MpCardBonusRule>("least_money_to_use_bonus", "积分抵扣门槛金额");
        AssertJsonProperty<MpCardCustomField>("name_type", "自定义会员信息类目类型");
        AssertJsonProperty<MpCardCustomCell>("tips", "会员服务入口提示语");
        AssertJsonProperty<MpCardUrlCell>("app_brand_pass", "消息卡片的页面路径");

        // 投放面。
        AssertJsonProperty<MpCardQrcodeCardInfo>("is_unique_code", "是否唯一码");
        AssertJsonProperty<MpCardQrcodeCreateResponse>("show_qrcode_url", "可直接展示的图片地址（SDK 不做图片通道）");
        AssertJsonProperty<MpCardLandingPageCreateRequest>("page_title", "落地页标题");
        AssertJsonProperty<MpCardLandingPageCardInfo>("thumb_url", "卡片封面");
        AssertJsonProperty<MpCardTestWhiteListSetRequest>("username", "与 openid 二选一的公众号原始 id");
    }

    /// <summary>
    /// 契约守卫 CD4：建卡方向与修改方向<b>请求形态不同构</b>（防「复用同一请求 DTO」）。
    /// </summary>
    [Fact]
    public void CreateAndUpdateForms_ShouldNotBeIsomorphic()
    {
        // 建卡：顶层只有 card 包装，card_type 在 card 内，11 分支齐备且各带 advanced_info。
        JsonNamesOf(typeof(MpCardCreateRequest)).Should().BeEquivalentTo(new[] { "card" },
            "建卡请求体为 card 包装（官方形态，非扁平）");
        var createBody = JsonNamesOf(typeof(MpCardCreateBody));
        createBody.Should().Contain("card_type");
        createBody.Should().Contain(CreateBranchKeys, "11 个券型分支全部在建卡方向");
        CreateBranchKeys.Should().OnlyContain(k => createBody.Contains(k), "分支键集合不得漂移（新增券型须同批改本断言）");

        // 修改：card_id 与分支同层，无 card 包装、无 card_type。
        var updateTop = JsonNamesOf(typeof(MpCardUpdateRequest));
        updateTop.Should().Contain("card_id", "修改方向顶层为 card_id");
        updateTop.Should().NotContain("card", "修改方向无 card 包装");
        updateTop.Should().NotContain("card_type", "分支由 card_id 隐含，官方修改页无 card_type");
        updateTop.Should().Contain(CreateBranchKeys, "修改方向分支键与建卡同名同集合");

        // advanced_info：建卡 11 支全有、查询 10 支全有、修改一支都无。
        CountJsonName(BranchTypesOf(typeof(MpCardCreateBody)), "advanced_info").Should().Be(11,
            "建卡各分支均可提交高级信息");
        CountJsonName(BranchTypesOf(typeof(MpCardUpdateRequest)), "advanced_info").Should().Be(0,
            "官方修改页不开放 advanced_info（高级信息建卡后不可改）");

        // 建卡应答与修改应答各自独立：修改应答不回传 card_id，只回「是否送审」。
        OwnJsonNamesOf(typeof(MpCardUpdateResponse)).Should().BeEquivalentTo(new[] { "send_check" },
            "修改应答仅 send_check（不回传 card_id）");
        OwnJsonNamesOf(typeof(MpCardCreateResponse)).Should().BeEquivalentTo(new[] { "card_id" });

        // 删除 / 库存 / 查询单张均为「仅 card_id」级请求（防把整个 card 对象塞进这些端点）。
        JsonNamesOf(typeof(MpCardDeleteRequest)).Should().BeEquivalentTo(new[] { "card_id" });
        JsonNamesOf(typeof(MpCardGetRequest)).Should().BeEquivalentTo(new[] { "card_id" });

        // 纯 errcode/errmsg 应答复用 MpResponse（防空壳 DTO）。
        TaskResultType(typeof(IMpCardService), nameof(IMpCardService.ModifyCardStockAsync)).Should().Be(typeof(MpResponse));
        TaskResultType(typeof(IMpCardService), nameof(IMpCardService.DeleteCardAsync)).Should().Be(typeof(MpResponse));
        TaskResultType(typeof(IMpCardService), nameof(IMpCardService.SetPayCellAsync)).Should().Be(typeof(MpResponse));
        TaskResultType(typeof(IMpCardService), nameof(IMpCardService.SetSelfConsumeCellAsync)).Should().Be(typeof(MpResponse));
        TaskResultType(typeof(IMpCardService), nameof(IMpCardService.SetTestWhiteListAsync)).Should().Be(typeof(MpResponse));
    }

    /// <summary>
    /// 契约守卫 CD5：查询方向分支集与会员卡「同一东西的两种写法」并存。
    /// </summary>
    [Fact]
    public void QueryBranchesAndMemberCardDualForms_ShouldKeepAdjudicatedSets()
    {
        // 查询方向 10 支：无 general_card（礼品卡详情由 /card/giftcard/* 族承载，本域未建模）。
        var queryDetail = JsonNamesOf(typeof(MpCardQueryDetail));
        queryDetail.Should().Contain("card_type");
        queryDetail.Should().HaveCount(11, "card_type + 10 个券型分支");
        queryDetail.Should().NotContain("general_card",
            "查询方向无 general_card 分支（礼品卡走 /card/giftcard/* 族，未建模 ⇒ 本域不提供查询形态）");
        queryDetail.Should().NotContain("card", "查询应答的 card 包装在 MpCardGetResponse 上，MpCardQueryDetail 是其内容");

        // 会员卡三向差集（照对齐基准逐支锁定，防「顺手补齐」）。
        var createMember = JsonNamesOf(typeof(MpCardCreateMemberCard));
        var queryMember = JsonNamesOf(typeof(MpCardQueryMemberCard));
        var updateMember = JsonNamesOf(typeof(MpCardUpdateMemberCard));

        queryMember.Should().Contain("bind_old_card_url", "查询方向独有：旧卡绑定入口");
        createMember.Should().NotContain("bind_old_card_url");
        updateMember.Should().NotContain("bind_old_card_url");

        updateMember.Should().Contain(new[] { "modify_msg_operation", "activate_msg_operation" },
            "修改方向独有：两个消息运营入口（建卡与查询方向均无）");
        createMember.Should().NotContain("modify_msg_operation");
        queryMember.Should().NotContain("activate_msg_operation");

        // 高级信息只在建卡与查询方向出现（CD4 已断修改方向整体缺失）。
        createMember.Should().Contain("advanced_info");
        queryMember.Should().Contain("advanced_info");

        // 「同一个东西的两种写法」并存，不得择一删除：积分规则字符串 + 对象。
        foreach (var type in new[] { typeof(MpCardCreateMemberCard), typeof(MpCardUpdateMemberCard), typeof(MpCardQueryMemberCard) })
        {
            JsonNamesOf(type).Should().Contain("bonus_rules", $"{type.Name} 保留积分规则字符串写法");
            JsonNamesOf(type).Should().Contain("bonus_rule", $"{type.Name} 保留积分规则对象写法");
        }

        // 一键激活的直接型与跳转型并存；外链与小程序入口并存。
        createMember.Should().Contain(new[]
        {
            "wx_activate", "wx_activate_after_submit", "wx_activate_after_submit_url",
            "activate_url", "activate_app_brand_user_name", "activate_app_brand_pass",
        }, "一键激活与入口外链的多种官方写法全部保留");
        queryMember.Should().NotContain("wx_activate_after_submit", "查询方向不回传一键激活配置组");

        // 会员卡专用 base_info 只增两字段（pay_info 仅建卡/修改方向）。
        JsonNamesOf(typeof(MpCardMemberCardBaseInfo)).Should().Contain(new[] { "need_push_on_view", "pay_info" });
        JsonNamesOf(typeof(MpCardUpdateMemberCardBaseInfo)).Should().Contain(new[] { "need_push_on_view", "pay_info" });
        JsonNamesOf(typeof(MpCardQueryMemberCardBaseInfo)).Should().Contain("need_push_on_view")
            .And.NotContain("pay_info", "查询方向无 pay_info（支付信息不回传）");
    }

    /// <summary>契约守卫 CD6：库存与卡面组件开关的键名钉死（含高风险拼写照录）。</summary>
    [Fact]
    public void StockAndCellSwitches_ShouldLockKeyNames()
    {
        // 库存：增量语义、键名为 *_stock_value（不是 *_quantity，也不是目标总量）。
        JsonNamesOf(typeof(MpCardModifyStockRequest)).Should().BeEquivalentTo(new[]
        {
            "card_id", "increase_stock_value", "reduce_stock_value",
        }, "库存端点键名钉死为 increase_stock_value / reduce_stock_value");
        typeof(MpCardModifyStockRequest).GetProperty(nameof(MpCardModifyStockRequest.IncreaseStockValue))!
            .PropertyType.Should().Be(typeof(int?), "增量字段可空（未传即不增）");

        // 组件开关：is_open（不是 open/status）。
        JsonNamesOf(typeof(MpCardPayCellSetRequest)).Should().BeEquivalentTo(new[] { "card_id", "is_open" });
        JsonNamesOf(typeof(MpCardSelfConsumeCellSetRequest)).Should().BeEquivalentTo(new[]
        {
            "card_id", "is_open", "need_verify_cod", "need_remark_amount",
        }, "自助核销开关四字段（need_verify_cod 为对齐基准的键名）");

        // 高风险留档：官方（对齐基准）键名缺尾字母 e，SDK 照录不修正。
        // 若后续逐页核验证实官方实为 need_verify_code，须同批改 DTO 与本断言。
        typeof(MpCardSelfConsumeCellSetRequest).GetProperty(nameof(MpCardSelfConsumeCellSetRequest.NeedVerifyCode))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("need_verify_cod", "照录对齐基准键名（缺尾字母 e），不得驼峰化也不得擅自补 e");

        // 修改应答的 send_check 为布尔（不回传 card_id）。
        typeof(MpCardUpdateResponse).GetProperty(nameof(MpCardUpdateResponse.RequireSendCheck))!
            .PropertyType.Should().Be(typeof(bool), "send_check 为布尔语义（是否进入送审流程）；应答侧必有值 ⇒ 非可空");
    }

    /// <summary>契约守卫 CD7：券码面（核销 / 查码状态 / 解密）的字段名与应答形态。</summary>
    [Fact]
    public void CardCodeSurface_ShouldLockResponseShapes()
    {
        // 核销请求：card_id 可选（官方允许仅凭 code 定位），code 必填。
        JsonNamesOf(typeof(MpCardCodeConsumeRequest)).Should().BeEquivalentTo(new[] { "card_id", "code" });
        typeof(MpCardCodeConsumeRequest).GetProperty(nameof(MpCardCodeConsumeRequest.CardId))!
            .PropertyType.Should().Be(typeof(string), "card_id 为可选（官方「传则校验一致性」语义）");

        // 核销应答只回 card_id 一层，不复用查询详情形态（防把整个 card 对象误建成查询 DTO）。
        OwnJsonNamesOf(typeof(MpCardCodeConsumeResponse)).Should().BeEquivalentTo(new[] { "card", "openid" });
        JsonNamesOf(typeof(MpCardConsumeCardInfo)).Should().BeEquivalentTo(new[] { "card_id" });

        // 查码状态：check_consume 为请求侧可选开关。
        JsonNamesOf(typeof(MpCardCodeGetRequest)).Should().BeEquivalentTo(new[] { "card_id", "code", "check_consume" });

        // 查码应答字段面（会员卡字段与券码字段分处两层：user_card_status 与 card.* 下的字段不混写）。
        OwnJsonNamesOf(typeof(MpCardCodeGetResponse)).Should().BeEquivalentTo(new[]
        {
            "card", "user_card_status", "openid", "nickname", "membership_number", "user_info",
            "bonus", "balance", "order_id", "background_pic_url", "can_consume", "outer_str",
        }, "code/get 应答字段集（对齐基准）；官方键名 user_card_status 与 CLR 名 CardStatus 有意不同");

        // 券码明细层：bonus/balance 在此处属「券码视角」，与应答顶层的会员视角同名不同源 ⇒ 两处并存不合并。
        JsonNamesOf(typeof(MpCardCodeInfo)).Should().BeEquivalentTo(new[]
        {
            "card_id", "code", "card_number", "begin_time", "end_time", "bonus", "balance",
        }, "card 子对象字段集（与顶层 bonus/balance 不同源）");
        JsonNamesOf(typeof(MpCardMemberUserInfo)).Should().BeEquivalentTo(new[]
        {
            "common_field_list", "custom_field_list",
        }, "会员信息按官方分为通用与自定义两组");

        // 解密：请求侧只有 encrypt_code，应答侧只有 code（SDK 不编排「解密 → 核销」两步）。
        JsonNamesOf(typeof(MpCardCodeDecryptRequest)).Should().BeEquivalentTo(new[] { "encrypt_code" });
        OwnJsonNamesOf(typeof(MpCardCodeDecryptResponse)).Should().BeEquivalentTo(new[] { "code" });
    }

    /// <summary>
    /// 契约守卫 CD8：注册三段式 + DTO 全量登记 + <b>未建模 10 族 39 端点的零路由留档</b>。
    /// </summary>
    [Fact]
    public void CardRegistrationAndUnmodeledFamilies_ShouldBeDocumented()
    {
        // 域 DTO 全量登记进 CardJsonContext（AOT 源生成）。
        const int expectedDomainTypes = 90;
        var domainTypes = typeof(MpCardCreateRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == CardNamespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(expectedDomainTypes,
            "卡券域 DTO 共 90 型（三向 base_info + 11×3 券型分支 + 共用子表 + 投放/核销/开关面）");
        foreach (var type in domainTypes)
        {
            CardJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 CardJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 注册三段式：模块枚举 + Add{域}Api() + 生成的 Add{域}WebApiHttpClient()（由编译期保证后者存在）。
        Enum.GetNames(typeof(MpModule)).Should().Contain(RegistryGroupName);
        typeof(MpServiceBuilder).GetMethod("AddCardApi").Should().NotBeNull(
            "AddCardApi() 必须存在（与 MpModuleRegistrar 字典条目同批）");

        // Query 令牌白名单「本域在内」（全量集合由 MpQueryTokenWhitelistGuard QT1 持有）。
        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();
        queryInterfaces.Should().Contain(nameof(IMpCardService));
        queryInterfaces.Should().Contain(nameof(IMpCardCodeService));

        // 本域路由集合 = CD1 的 14 条（按接口归属判定）。
        CardInterfaceRoutes().Should().BeEquivalentTo(Routes.Select(r => r.Route),
            "卡券双接口上的路由必须恰为本批 14 条（新增端点须同批进 CD1 与 MpRouteCountGuard）");

        // <b>前缀共用留档（本守卫实测发现）</b>：<c>/card/</c> 不是卡券域独占前缀 ——
        // 微信发票域（IMpInvoiceService，17 端点）的路由全部落在 <c>/card/invoice/*</c> 之下。
        // ⇒ 任何按裸 "/card/" 前缀统计卡券端点的做法都会把发票域算进来，本域判定一律按接口归属。
        AssemblyRoutesUnder("/card/invoice/").Should().HaveCount(17,
            "发票域 17 端点共用 /card/ 前缀（明细见 MpInvoiceContractGuards）");
        CardInterfaceRoutes().Should().NotContain(r => r.StartsWith("/card/invoice/", StringComparison.Ordinal),
            "卡券域不得声明发票路由");
        AssemblyRoutesUnder("/card/").Except(AssemblyRoutesUnder("/card/invoice/"))
            .Should().BeEquivalentTo(Routes.Select(r => r.Route),
                "程序集内 /card/ 前缀路由 = 卡券 14 + 发票 17，无第三域混入");

        // 未建模族的零路由留档（对齐基准 SKIT 全族 53 端点，本批 14 ⇒ 未建模 39）。
        // 每族都需要独立的票据 / 支付 / 资质核验，刻意不在本批塞进同一注册组。
        foreach (var prefix in new[]
                 {
                     "/card/membercard/",   // 会员卡 6（activate / activateuserform / updateuser / userinfo …）
                     "/card/giftcard/",     // giftcard 族 11（page / order / pay / wxa，与支付强耦合）
                     "/card/paygiftcard/",  // paygiftcard 族 4（需商户号与支付资质）
                     "/card/submerchant/",  // 子商户 4（进件与资质核验面）
                     "/card/storewxa/",     // 门店小程序 2
                     "/card/mpnews/",       // 卡券图文 1
                     "/card/generalcard/",  // generalcard/updateuser 1（券型语义与建卡键名的对应关系待官方逐页核验）
                     "/card/boardingpass/", // 特殊票券 3（值机 / 会议 / 电影票改单）
                     "/card/meetingticket/",
                     "/card/movieticket/",
                     "/card/user/",         // 用户卡包 1（getcardlist）
                 })
        {
            AssemblyRoutesUnder(prefix).Should().BeEmpty(
                $"{prefix} 族未建模 ⇒ 必须零端点（新增时同批扩展 CD1/CD8 与 MpRouteCountGuard）");
        }

        // 券码运维族：本域只建模 consume / get / decrypt 三条。
        AssemblyRoutesUnder("/card/code/").Should().BeEquivalentTo(new[]
        {
            "/card/code/consume", "/card/code/get", "/card/code/decrypt",
        }, "券码运维族未建模 checkcode / deposit / getdepositcount / update / unavailable 五条");
    }

    /// <summary>卡券双接口自身声明的路由（按接口归属判定，不受 <c>/card/invoice/</c> 前缀共用影响）。</summary>
    private static List<string> CardInterfaceRoutes()
        => new[] { typeof(IMpCardService), typeof(IMpCardCodeService) }
            .SelectMany(i => i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Where(u => u != null)
            .Select(u => u!)
            .Distinct(StringComparer.Ordinal)
            .ToList();

    /// <summary>主程序集公开接口上落在指定前缀下的全部路由（跨域视角，用于未建模族的零端点断言）。</summary>
    private static List<string> AssemblyRoutesUnder(string prefix)
        => typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .SelectMany(t => t.GetMethods())
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Where(u => u != null && u.StartsWith(prefix, StringComparison.Ordinal))
            .Select(u => u!)
            .Distinct(StringComparer.Ordinal)
            .ToList();

    private static MethodInfo FindMethod(Type iface, string methodName)
        => iface.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            ?? throw new InvalidOperationException($"{iface.Name}.{methodName} 必须存在");

    private static Type TaskResultType(Type iface, string methodName)
        => FindMethod(iface, methodName).ReturnType.GetGenericArguments()[0];

    /// <summary>取属性集（含继承来的公共实例属性）上的官方 JSON 键名。</summary>
    private static List<string> JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    /// <summary>取<b>本型声明</b>的官方 JSON 键名（排除 <see cref="MpResponse"/> 基类的 errcode / errmsg）。</summary>
    private static List<string> OwnJsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    private static int CountAppBrand(IEnumerable<string> names)
        => names.Count(n => n.Contains("_app_brand_", StringComparison.Ordinal));

    /// <summary>取分支属性（复杂类型属性）指向的类型集合，用于逐分支统计 advanced_info 的分布。</summary>
    private static IEnumerable<Type> BranchTypesOf(Type container)
        => container.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsClass && p.PropertyType != typeof(string))
            .Select(p => p.PropertyType)
            .ToList();

    private static int CountJsonName(IEnumerable<Type> types, string jsonName)
        => types.Count(t => JsonNamesOf(t).Contains(jsonName));

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
