// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 微信支付非契约路由零声明守卫（追踪号 P0-7-PAY-ZERO-DECLARATION-2026-10-14，P1-3 / P1-4 增补 2026-10-15）：
/// 断言 45 条路由（非契约清单 53 行，同路由多方法各归并 1 条）在 <c>Src/Pay/</c> 全部源码内<b>零声明</b>，
/// 防「网关还能打通」重新诱导加回。
/// <para>
/// <b>判据均为官方文档面</b>：支持商户类型不是本线目标商户、官方文档树内<b>无该端点契约页</b>，
/// 或本仓已有等价现行路由。第三方 SDK 的路由清单本身<b>不是</b>契约。
/// </para>
/// <para>
/// 一、<b>合规证明 Compliance（2 端点）</b>：官方页面 4012471357 / 4012471359
/// 「获取合规证明下载链接 / 查询合规证明列表」的支持商户类型为【平台商户】，
/// 非本线目标商户【普通服务商】⇒ 零端点。
/// </para>
/// <para>
/// 二、<b>商户运营 MerchantOperate（3 端点）</b>：官方页面「修改商家简称 / 查询商家简称修改状态 /
/// 撤销商家简称修改申请」的支持商户类型为【平台商户】，非本线目标商户 ⇒ 零端点。
/// </para>
/// <para>
/// 三、<b>商户运营管控尽调族 MerchantOperationManage enhanced-due-diligence（3 端点）</b>：
/// 同前缀 <c>/v3/mch-operation-manage/enhanced-due-diligence</c> 下的三支（提交尽调申请 /
/// 查询尽调申请状态 / 撤销尽调申请）支持商户类型为【平台商户】，与已落位的管控情况查询支
/// （【普通服务商】，GET <c>/v3/mch-operation-manage/merchant-limitations/sub-mchid/{sub_mchid}</c>）
/// 是两套契约 ⇒ 尽调族零端点（守卫 MOM-B2）。
/// </para>
/// <para>
/// 四、<b>实名认证 RealName（1 端点）</b>：官方页面「查询实名认证结果」的支持商户类型为【平台商户】，
/// 非本线目标商户 ⇒ 零端点。
/// </para>
/// <para>
/// 五、<b>商家周周减 MerchantWeeklyDiscount（3 端点）</b>：官方页面「创建代金券批次 /
/// 激活代金券批次 / 查询代金券批次详情」的支持商户类型为【平台商户】，
/// 与本线已落位的代金券域 <see cref="Mud.Wechat.Pay.IWechatPayMarketingFavorService"/>（【普通服务商】面）是两套契约 ⇒ 零端点。
/// </para>
/// <para>
/// 六、<b>商家券与营销八族（P1-3 增补，13 条）</b>：逐页取证（2026-10-15）核得两种失效形态 ——
/// ① <b>官方文档树内无该端点契约页</b>（8 条）：商家券 <c>coupons/{card_id}/send</c>、
/// 补贴凭证四支（<c>subsidy/pay-receipts</c> 三支 + <c>subsidy/return-receipts</c>）、
/// 代金券按日明细两支（<c>refund-flow-by-day</c> / <c>use-flow-by-day</c>，现行页只有不分日的
/// <c>refund-flow</c> / <c>use-flow</c> ⇒ 已落位，守卫 MF5）、合作关系 <c>partnerships/terminate</c>；
/// ② <b>清单出处页与端点无关</b>（5 条）：折扣卡三支与提现券两支的转述出处页现行正文是
/// 「JSAPI 支付产品概述」（<c>merchant/4012062524</c>、<c>partner/4012069852</c>），页内零个
/// HTTP 契约小节 ⇒ 路由核不到官方文档面，按「核实不到即零端点」处置。
/// </para>
/// <para>
/// 七、<b>会员卡（旧版）两族（P1-4 增补，28 行 / 22 条路由）</b>：清单 48~75 行的出处 URL 全部是
/// <b>老版 wiki 路径</b> <c>pay.weixin.qq.com/wiki/doc/apiv3/wxpay/marketing/membercard_open/chapterX_Y.shtml</c>，
/// 逐行取证（2026-10-15）核得它们<b>统一 302 落到同一个页面</b> <c>merchant/4012062524</c>
/// （「JSAPI 支付」产品概述，正文零 HTTP 契约小节、零路由行；<c>?from=</c> 参数逐字记录了原 wiki 路径）⇒
/// 与第六条的折扣卡/提现券两支<b>同一形态、同一重定向目标</b>。且现行文档树的会员卡章节只有
/// <b>品牌会员卡</b>族（<c>/v3/brand/partner/card-member/*</c>，本批已落位 15 端点，
/// 见 <see cref="Mud.Wechat.Pay.IWechatPayBrandCardMemberService"/>）⇒
/// <c>membercard-open</c> 与 <c>membercard-activity</c> 两族在官方文档面<b>无契约页</b>，零声明。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 本守卫为「清单即契约」型断言：45 条路由（53 清单行）的增减都必须先复核官方文档面
/// （支持商户栏 + 契约页正文），不得因网关可达或第三方 SDK 在册而放宽。
/// </para>
/// </remarks>
public class WechatPayNonContractRouteZeroDeclarationContractGuards
{
    /// <summary>
    /// 非契约路由清单（45 条 = Compliance 2 + MerchantOperate 3 + 尽调族 3 + RealName 1 + 周周减 3
    /// + P1-3 商家券与营销 11 + P1-4 会员卡（旧版）两族 22；P1-3 的 13 清单行里「补贴凭证 pay-receipts 的 POST 与 GET」同路由、
    /// 「提现券查询与发放」同路由，P1-4 的 28 清单行里 <c>activities</c>（GET/POST）、<c>callback</c>（POST/GET）、
    /// <c>cards</c>（POST/GET）、<c>cards/{card_id}</c>（GET/PATCH/DELETE）、
    /// <c>cards/{card_id}/codes/{code}</c>（PATCH/GET）各归并为 1 条 ⇒ 各 6 行折成 1 条。
    /// 路由计数按 (方法, 路由) 的去重口径同样为 0，因为这些支都不存在）。
    /// </summary>
    private static readonly string[] NonContractRoutes =
    {
        // 合规证明 Compliance（2 端点）：官方页面 4012471357 / 4012471359 支持商户为【平台商户】。
        "/v3/compliance/certificate/download",
        "/v3/compliance/certificate/query",

        // 商户运营 MerchantOperate（3 端点）：官方页面支持商户为【平台商户】。
        "/v3/merchant-operate/merchant-name-alter/apply",
        "/v3/merchant-operate/merchant-name-alter/application/{application_id}",
        "/v3/merchant-operate/merchant-name-alter/application/{application_id}/cancel",

        // 商户运营管控尽调族（3 端点）：enhanced-due-diligence 支持商户为【平台商户】。
        "/v3/mch-operation-manage/enhanced-due-diligence/submit",
        "/v3/mch-operation-manage/enhanced-due-diligence/{application_id}",
        "/v3/mch-operation-manage/enhanced-due-diligence/{application_id}/cancel",

        // 实名认证 RealName（1 端点）：官方页面支持商户为【平台商户】。
        "/v3/realname/verify-result",

        // 商家周周减 MerchantWeeklyDiscount（3 端点）：官方页面支持商户为【平台商户】。
        "/v3/marketing/favor/platform/media-create",
        "/v3/marketing/favor/platform/media-start",
        "/v3/marketing/favor/platform/media-query/{stock_id}",

        // 商家券发券（1 端点）：官方 busifavor 文档树内无 coupons/{card_id}/send 契约页，
        // 现行发券面是「创建商家券批次 + 传入 user_key 的发放支」⇒ 该路由属第三方转述漂移。
        "/v3/marketing/busifavor/coupons/{card_id}/send",

        // 商家券补贴凭证四支（4 端点）：官方补贴面现行页只有「补贴预下单 / 补贴查询 / 补贴回调」，
        // pay-receipts 与 return-receipts 四支在文档树内无契约页。
        "/v3/marketing/busifavor/subsidy/pay-receipts",
        "/v3/marketing/busifavor/subsidy/pay-receipts/{subsidy_receipt_id}",
        "/v3/marketing/busifavor/subsidy/return-receipts",

        // 代金券按日明细两支（2 端点）：现行页只有不分日的 refund-flow / use-flow（已落位，守卫 MF5）。
        "/v3/marketing/favor/stocks/{stock_id}/refund-flow-by-day",
        "/v3/marketing/favor/stocks/{stock_id}/use-flow-by-day",

        // 合作关系终止（1 端点）：官方 partnerships 面只有「建立 + 查询列表」两支，无 terminate 契约页。
        "/v3/marketing/partnerships/terminate",

        // 折扣卡三支（3 端点）：清单出处页正文是「JSAPI 支付产品概述」，页内零 HTTP 契约小节。
        "/v3/discount-card/cards",
        "/v3/discount-card/cards/{out_card_code}",
        "/v3/discount-card/cards/{out_card_code}/add-user-records",

        // 提现券两支（2 端点）：清单出处页同上，属 JSAPI 支付概述页 ⇒ 路由无官方文档面。
        "/v3/marketing/withdraw-favor/users/{openid}/coupons",

        // 会员卡活动族（4 清单行 → 3 条路由：activities 的 GET 与 POST 同路由）：出处 URL 是老版 wiki 路径，
        // 302 落到 merchant/4012062524「JSAPI 支付」概述页，页内零 HTTP 契约小节 ⇒ 官方文档面无契约页（守卫 P1-4 第七条）。
        "/v3/marketing/membercard-activity/activities",
        "/v3/marketing/membercard-activity/activities/{activity_id}",
        "/v3/marketing/membercard-activity/activities/{activity_id}/terminate",

        // 会员卡（旧版）开放接口族（24 清单行 → 19 条路由：callback / cards / cards/{card_id} /
        // cards/{card_id}/codes/{code} 四条各承载两到三个方法）：同上，wiki 路径全部 302 到同一个概述页；
        // 现行文档树的会员卡章节只有品牌会员卡 /v3/brand/partner/card-member/*（已落位，见下方正对照）。
        "/v3/marketing/membercard-open/activate-informations/{activate_ticket}",
        "/v3/marketing/membercard-open/callback",
        "/v3/marketing/membercard-open/cards",
        "/v3/marketing/membercard-open/cards/{card_id}",
        "/v3/marketing/membercard-open/cards/{card_id}/codes/{code}",
        "/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/activate",
        "/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/bonus/commit",
        "/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/bonus/prepare",
        "/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/rights",
        "/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/unavailable",
        "/v3/marketing/membercard-open/cards/{card_id}/codes/deposit",
        "/v3/marketing/membercard-open/cards/{card_id}/decrypt-code",
        "/v3/marketing/membercard-open/cards/{card_id}/permission-tokens",
        "/v3/marketing/membercard-open/cards/{card_id}/phone-membercard/import",
        "/v3/marketing/membercard-open/cards/{card_id}/qrcode",
        "/v3/marketing/membercard-open/cards/{card_id}/rights",
        "/v3/marketing/membercard-open/cards/{card_id}/upgrade",
        "/v3/marketing/membercard-open/cards/{card_id}/users/import",
        "/v3/marketing/membercard-open/user/cards",
    };

    /// <summary>扫描根：支付线 Src/Pay/ 下全部 .cs 文件。</summary>
    private static readonly DirectoryInfo PaySrcRoot = new(
        Path.GetFullPath(Path.Combine(typeof(WechatPayNonContractRouteZeroDeclarationContractGuards).Assembly.Location, "..", "..", "..", "..", "..", "..", "Src", "Pay")));

    /// <summary>读取 Src/Pay/ 全源码（排除 obj/bin）—— 两个断言共用同一扫描口径。</summary>
    private static string ReadPaySource()
    {
        var csFiles = PaySrcRoot.GetFiles("*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.FullName.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                           && !f.FullName.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(csFiles);

        return string.Join("\n", csFiles.Select(static f => File.ReadAllText(f.FullName)));
    }

    /// <summary>
    /// 路由 → 匹配式：官方清单里的 path 占位符是 snake_case（<c>{stock_id}</c>），而源码声明的是
    /// C# 参数名（<c>{stockId}</c>）⇒ 占位符段一律收成通配，否则「改名落位」会绕过本守卫变成假绿。
    /// </summary>
    private static string RoutePattern(string route) =>
        "\"" + Regex.Replace(route, @"\{[^}]*\}", "[^/\"]*") + "\"";

    /// <summary>零声明主断言：清单内每条非契约路由在 <c>Src/Pay/</c> 全源码里以「引号字面量」形态零命中。</summary>
    [Fact]
    public void NonContractRoutes_ShouldHaveZeroDeclarationsInPaySource()
    {
        var allSource = ReadPaySource();

        // 名册规模（45 条）与文档注释同批：清单行增减须先复核官方文档面，不能只改注释不改数组。
        NonContractRoutes.Should().HaveCount(45, "非契约路由名册规模须与类注释一致（53 清单行按同路由归并）");
        NonContractRoutes.Distinct().Should().HaveCount(45, "同一路径不得在名册里出现两次（归并口径由注释逐族写明）");

        foreach (var route in NonContractRoutes)
        {
            Regex.Matches(allSource, RoutePattern(route)).Should().BeEmpty(
                $"非契约路由 {route} 在 Src/Pay/ 出现声明：判据是官方文档面（支持商户栏 + 契约页正文），不得因网关可达或第三方 SDK 在册而加回");
        }
    }

    /// <summary>正对照：已落位路由必须被同一扫描口径命中，证明上面的零命中是真裁决而不是扫描失效。</summary>
    [Fact]
    public void PositiveControl_AlreadyDeclaredRoutes_ShouldBeFound()
    {
        // 正对照：已落位的路由必须能在源码中找到，确保扫描本身有效。
        // 七条覆盖「纯字面量 / 单个占位符 / 占位符在段中」三种形状，其中后四条与零声明清单成对
        // （favor stocks 族对照 refund-flow-by-day，busifavor users 族对照 withdraw-favor 两支，
        // 品牌会员卡 cards 两支对照旧版 membercard-open cards 族 ⇒ 替代关系一旦被误判为「同一族」即红），
        // 口径一旦漂移即红。
        var positiveRoutes = new[]
        {
            // Transactions 域
            "/v3/pay/transactions/jsapi",
            // MerchantStore 域
            "/v3/merchant-store/stores",
            // MerchantService 域
            "/v3/merchant-service/complaints-v2",
            // MarketingFavor 域：零声明的 refund-flow-by-day 的现行姊妹支（占位符名 = C# 参数名）
            "/v3/marketing/favor/stocks/{stockId}/refund-flow",
            // MarketingBusifavor 域：openid 占位符在 path（与零声明的 withdraw-favor 两支同形状）
            "/v3/marketing/busifavor/users/{openid}/coupons",
            // BrandCardMember 域：零声明的旧版 /v3/marketing/membercard-open/cards 族的现行替代品
            // （同尾段 cards / cards/{card_id} 形状，占位符名 = C# 参数名）
            "/v3/brand/partner/card-member/cards",
            "/v3/brand/partner/card-member/cards/{cardId}",
        };

        var allSource = ReadPaySource();

        foreach (var route in positiveRoutes)
        {
            Regex.Matches(allSource, RoutePattern(route)).Should().NotBeEmpty(
                $"正对照路由 {route} 未被扫描口径命中：扫描根或匹配式已漂移，零声明断言将静默恒真");
        }
    }
}
