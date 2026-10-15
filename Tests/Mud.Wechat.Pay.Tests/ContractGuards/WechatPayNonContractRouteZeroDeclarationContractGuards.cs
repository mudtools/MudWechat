// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 微信支付非契约路由零声明守卫（追踪号 P0-7-PAY-ZERO-DECLARATION-2026-10-14）：
/// 断言 12 条路由在 <c>Src/Pay/</c> 全部源码内<b>零声明</b>，防「网关还能打通」重新诱导加回。
/// <para>
/// <b>判据均为官方文档面</b>：支持商户类型不是【普通服务商】或本仓已有等价现行路由。
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
/// </summary>
/// <remarks>
/// <para>
/// 本守卫为「清单即契约」型断言：12 条路由的增减都必须先复核官方文档「支持商户」一栏，
/// 不得因网关可达而放宽。
/// </para>
/// </remarks>
public class WechatPayNonContractRouteZeroDeclarationContractGuards
{
    /// <summary>
    /// 非契约路由清单（12 条 = Compliance 2 + MerchantOperate 3 + 尽调族 3 + RealName 1 + 周周减 3）。
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
    };

    /// <summary>扫描根：支付线 Src/Pay/ 下全部 .cs 文件。</summary>
    private static readonly DirectoryInfo PaySrcRoot = new(
        Path.GetFullPath(Path.Combine(typeof(WechatPayNonContractRouteZeroDeclarationContractGuards).Assembly.Location, "..", "..", "..", "..", "..", "..", "Src", "Pay")));

    /// <summary>零声明主断言：清单内每条非契约路由在 <c>Src/Pay/</c> 全源码里以「引号字面量」形态零命中。</summary>
    [Fact]
    public void NonContractRoutes_ShouldHaveZeroDeclarationsInPaySource()
    {
        // 收集全部 .cs 源码内容（排除生成物与测试本身）。
        var csFiles = PaySrcRoot.GetFiles("*.cs", SearchOption.AllDirectories)
            .Where(f => !f.FullName.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                     && !f.FullName.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(csFiles);

        var allSource = string.Join("\n", csFiles.Select(f => File.ReadAllText(f.FullName)));

        foreach (var route in NonContractRoutes)
        {
            // 精确匹配路由字符串（含引号），避免子串误中。
            var pattern = $@"""{Regex.Escape(route)}""";
            var matches = Regex.Matches(allSource, pattern);

            Assert.Empty(matches);
        }
    }

    /// <summary>正对照：已落位路由必须被同一扫描口径命中，证明上面的零命中是真裁决而不是扫描失效。</summary>
    [Fact]
    public void PositiveControl_AlreadyDeclaredRoutes_ShouldBeFound()
    {
        // 正对照：已落位的路由必须能在源码中找到，确保扫描本身有效。
        var positiveRoutes = new[]
        {
            // Transactions 域
            "/v3/pay/transactions/jsapi",
            // MerchantStore 域
            "/v3/merchant-store/stores",
            // MerchantService 域
            "/v3/merchant-service/complaints-v2",
        };

        var csFiles = PaySrcRoot.GetFiles("*.cs", SearchOption.AllDirectories)
            .Where(f => !f.FullName.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                     && !f.FullName.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .ToArray();

        var allSource = string.Join("\n", csFiles.Select(f => File.ReadAllText(f.FullName)));

        foreach (var route in positiveRoutes)
        {
            var pattern = $@"""{Regex.Escape(route)}""";
            var matches = Regex.Matches(allSource, pattern);

            Assert.NotEmpty(matches);
        }
    }
}
