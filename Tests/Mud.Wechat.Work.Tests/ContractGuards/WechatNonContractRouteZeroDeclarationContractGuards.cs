// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 非契约路由零声明守卫（追踪号 P2-QY-SKIT-CATALOG-VERIFY-03，2026-10-12 二期企微缺口核验）：
/// 断言 35 条路由在 <c>Src/</c> 全部源码内<b>零声明</b>，防「网关还能打通」重新诱导加回。
/// <para>
/// 两类理由（判据均为官方文档面，与 <c>WechatDataZoneContractGuards</c> 的 DZ5 同源）：
/// </para>
/// <para>
/// 一、<b>官方文档目录树已无对应页面</b>（32 条）：互联企业（linkedcorp）6 条——现行「上下游通讯录管理」
/// 树为 corpgroup 面（95819/96607），全树无「互联企业」服务端页面，且 <c>linkedcorp/message/send</c>
/// 引用的 90250 现指向 <c>/cgi-bin/message/send</c>；任务卡片 2 条（91579/91585 均不在目录树内，
/// 全树标题无「任务卡片」）；汇报导出 2 条（96108 不在目录树内，同族 get_record_list / detail / stat
/// 三页仍在并已落位）；场所码 3 条（URL 本身错位：93514 现指向「政民沟通 &gt; 居民上报 &gt; 获取配置的网格及网格负责人」，
/// 95461/95464 不在目录树内，全树无「场所码」）；成员活跃 1 条（92714 不在目录树内，全树标题无「活跃」）；
/// 自建应用迁移 1 条（96072 现重定向到 <c>/document/40913</c>「自建应用迁移成代开发自建应用」，
/// 正文最后更新停在 2022-08-25 且该页无上一篇/下一篇链接（目录树内页面皆有），
/// 三棵应用类型树的「应用管理」节点均无该条目——自建 90227 树为
/// 概述 90226 / 获取应用 90227 / 设置应用 90228 / 自定义菜单 90231~90233 / 设置工作台自定义展示 92535，
/// 第三方 90363 树与代开发 99617 树亦无；其能力已由目录树内、2024-01-16 更新的 99617
/// 「自建应用迁移成代开发应用」（<c>/cgi-bin/agent/claim_customized_app</c>，字段面完全同形）承载并已落位）；
/// 会议室按会议 ID 查预定 1 条（93620「会议室预定管理」2024-06-24 正文的锚点仅六节——查询预定信息 /
/// 预定会议室 / 通过日程预定 / 通过会议预定 / 取消预定 / 按预定 ID 查详情，全页无
/// <c>get_booking_info_by_meeting_id</c>；「会议室」树的概述 93617 与会议室管理 93619 两页正文亦无该路由，
/// 同域其余 6 条路由均在页内并已落位——追踪号 P2-QY-MEETINGROOM-BYMEETINGID-07）。
/// 微信客服获取企业状态信息 1 条（developer 95153 与 kf.weixin.qq.com 95168 两棵目录树的同名页面正文均只剩概述，
/// 「获取企业验证状态」小节的唯一指引是「可通过获取企业授权信息接口获取」，页内无请求方式/请求地址/参数表
/// ——追踪号 P2-QY-KF-CORPQUALIFICATION-11）。
/// 服务商侧应用管理员列表 1 条（90791 现重定向到旧编号段的裸 URL <c>/document/14945</c>，正文完整但
/// <b>页内无上一篇/下一篇导航、无章节锚点</b>，最后更新 2024-07-12；其现行页是目录树内的 100073
/// 「获取应用管理员列表」，正文自注「旧的使用 suite_access_token 获取管理员的方式已不再维护」，
/// 本仓已落位于 <c>/cgi-bin/agent/get_admin_list</c>）。
/// 服务商硬件设备管理 8 条（清单给出的 90626 / 90753 / 91346 / 91475 / 92770 / 92782 六页现全部重定向到
/// 旧编号段裸 URL——<c>/document/15158</c>、<c>14960</c>、<c>15243</c>、<c>15859</c>、<c>22263</c>、<c>22318</c>，
/// 正文有完整请求地址与参数表，但<b>无上下篇导航、无章节锚点</b>，最后更新停在 2018-11-21 ~ 2020-09-28；
/// <c>reset_secret_no</c> 的清单出处 90753 现行正文（<c>14960</c>「查询设备绑定信息」）只有
/// <c>get_device_auth_info</c> 一个地址，属页面 ID 错配；<c>unionid_to_external_userid_3rd</c> 的清单出处
/// 95327 现重定向到 90556「企业自建应用开发 &gt; 概述」，页内无该路由，其现行落位是「账号ID &gt; ID转换接口」
/// 三棵树 95900 / 97108 / 101134）。硬件面的现行章节树是「智能硬件开发」，其凭证为
/// <c>model_access_token</c> / <c>device_access_token</c>，与三类应用令牌面不同构（见留档 §3.1 的待裁定项）
/// ——追踪号 P2-QY-SERVICE-PLACEMENT-05）。
/// 家校健康码 3 条（2026-10-18 复核：现行目录树全量锚点 1017 条——覆盖自建 / 第三方 / 代开发三棵应用类型树——
/// 检索「健康码」「健康」「qrcode」仅命中健康上报三支 93676~93678（已落位于 <c>/cgi-bin/health/*</c>）与
/// 「获取加入企业二维码」「获取『学校通知』二维码」「会议健康度」，无任何健康码或自定义健康信息页面；
/// 同前缀的学生族 92325~92338 在册且正文可解出请求地址（<c>update_student</c> 已作正对照）
/// ——追踪号 P2-QY-SCHOOL-HEALTHQRCODE-01）。
/// 收款商户号增删 2 条（2026-10-18 复核：现行 93666「收款商户号管理」在树内（上篇 93665 / 下篇 93667，
/// 最后更新 2024-07-18），正文请求地址仅 <c>getmerchant</c> 与 <c>set_mch_use_scope</c> 两支，
/// <c>addmerchant</c> / <c>delmerchant</c> 小节已消失；同页现存两支本仓均已落位 ⇒ 非「整页未渲染」的假阴性
/// ——追踪号 P2-QY-EXTERNALPAY-MERCHANTCRUD-01）。
/// 硬件旧面 1 条（2026-10-18 复核：清单出处 92739「获取设备自定义参数」标题仍在，但 SSR 正文已无
/// 「请求方式 / 请求地址」小节、无参数表、无上下篇导航、不在现行目录树内；同一取法在正对照页 95982
/// 可完整解出 <c>/cgi-bin/openhw/device/get_device_detail</c> 与六行参数表 ⇒ 非「整页未渲染」的假阴性。
/// 硬件面现行契约树为「智能硬件开发 &gt; 硬件云端接入」，凭证为 <c>model_access_token</c> /
/// <c>device_access_token</c>，已整域落位于 <c>IWechatWorkOpenHardwareService</c>
/// ——追踪号 P2-QY-HARDWARE-DEVICEFEATURE-01）。
/// 第三方 SDK 仍携带这些扩展方法只是历史镜像，不构成契约。
/// </para>
/// <para>
/// 二、<b>本仓已有等价现行路由</b>（3 条）：<c>/cgi-bin/user/getuserinfo</c> 与现行
/// <c>/cgi-bin/auth/getuserinfo</c> 是同一能力的旧路由名（官方「网页授权登录 &gt; 获取访问用户身份」
/// 三棵树齐备：91023 自建 / 91437 第三方 / 91707、91711），不再并列声明第二条路由；
/// <c>/cgi-bin/externalcontact/unionid_to_external_userid</c> 的官方现行落位是
/// 「账号ID &gt; ID转换接口 &gt; unionid与external_userid的关联」（95900 / 97108 / 101134），
/// 本仓已声明于 <c>/cgi-bin/idconvert/unionid_to_external_userid</c>；
/// <c>/cgi-bin/service/getuserdetail3rd</c>（缺 <c>auth/</c> 段的旧形态，第三方 SDK 转述遗留）
/// 的官方请求地址在树内 91122 与脱树 legacy 页 <c>/document/15123</c> 两处正文里
/// <b>均为</b> <c>/cgi-bin/service/auth/getuserdetail3rd</c>，本仓已落位于该形态。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 本守卫为「清单即契约」型断言：35 条路由的增减都必须先复核官方文档目录树，不得因网关可达而放宽。
/// <para>
/// 「页面在树内但该端点小节已消失」与「整页下架」同判非契约——官方正文的章节锚点即该页端点全集。
/// </para>
/// </para>
/// </remarks>
public class WechatNonContractRouteZeroDeclarationContractGuards
{
    /// <summary>
    // 非契约路由清单（35 条 = 官方目录已下架 32 + 本仓已有等价现行路由 3）。
    /// </summary>
    private static readonly string[] NonContractRoutes =
    {
        // 互联企业（linkedcorp）6 条：官方目录树无「互联企业」服务端页面。
        "/cgi-bin/linkedcorp/agent/get_perm_list",
        "/cgi-bin/linkedcorp/user/get",
        "/cgi-bin/linkedcorp/user/simplelist",
        "/cgi-bin/linkedcorp/user/list",
        "/cgi-bin/linkedcorp/department/list",
        "/cgi-bin/linkedcorp/message/send",
        // 任务卡片 2 条：91579 / 91585 / 92369 三页均已不在目录树内。
        "/cgi-bin/message/update_taskcard",
        "/cgi-bin/message/get_statistics",
        // 汇报导出 2 条：96108 不在目录树内（同族另三页在，已落位于 Journal 域）。
        "/cgi-bin/oa/journal/export_doc",
        "/cgi-bin/oa/journal/get_export_doc_result",
        // 场所码 3 条：方案给的 URL 本身错位，全树无「场所码」。
        "/cgi-bin/report/sitecode/list",
        "/cgi-bin/report/sitecode/get_site_report_info",
        "/cgi-bin/report/sitecode/get_report_answer",
        // 成员活跃 1 条：92714 不在目录树内。
        "/cgi-bin/user/get_active_stat",
        // 自建应用迁移 1 条：96072 正文重定向到 40913（无上下篇链接、停在 2022-08-25），
        // 三棵应用类型树的「应用管理」节点均无该条目；同能力由目录树内的 99617
        // claim_customized_app 承载（已落位于 IWechatWorkInternalAgentMigrationService）。
        "/cgi-bin/agent/migrate_to_customized_app",
        // 会议室按会议 ID 查预定 1 条：93620 现行正文锚点仅六节，无该小节（概述 93617、管理 93619 亦无）；
        // 同域 get_booking_info（按会议室/位置查预定）为其现行替代，已落位于 IWechatWorkInternalMeetingRoomBookingService。
        "/cgi-bin/oa/meetingroom/get_booking_info_by_meeting_id",
        // 微信客服获取企业状态信息 1 条：developer 95153 与 kf.weixin.qq.com 95168 两棵目录树的同名页面
        // 正文均只剩概述（「企业未完成主体验证时，微信客服仅可累计接待100位客户…」），
        // 其「获取企业验证状态」小节唯一指引是「可通过获取企业授权信息接口获取」，页内无请求方式/地址/参数表；
        // 第三方 SDK 仍携带该方法是历史镜像（其注释即写着企业微信已废弃），不构成契约
        //（追踪号 P2-QY-KF-CORPQUALIFICATION-11，域侧断言见 WechatKfContractGuards KF1i）。
        "/cgi-bin/kf/get_corp_qualification",
        // 服务商侧应用管理员列表 1 条：90791 现重定向到旧编号段裸 URL /document/14945（无上下篇、无章节锚点），
        // 现行页 100073 正文自注「旧的使用 suite_access_token 获取管理员的方式已不再维护」，
        // 本仓已落位于 /cgi-bin/agent/get_admin_list（追踪号 P2-QY-SERVICE-PLACEMENT-05）。
        "/cgi-bin/service/get_admin_list",
        // 服务商硬件设备管理 7 条：90626/90753/91346/91475/92770/92782 六页现重定向到旧编号段裸 URL
        // （/document/15158、14960、15243、15859、22263、22318），正文完整但无上下篇与章节锚点，
        // 最后更新停在 2018-11-21 ~ 2020-09-28；硬件面现行章节树「智能硬件开发」的凭证是
        // model_access_token / device_access_token（见留档 §3.1），三类应用令牌面不承载这批旧路由。
        "/cgi-bin/service/add_device",
        "/cgi-bin/service/get_device_auth_info",
        // 清单页面 ID 错配 1 条：其出处 90753 现行正文（/document/14960「查询设备绑定信息」）
        // 只有 get_device_auth_info 一个请求地址，全页无 reset_secret_no。
        "/cgi-bin/service/reset_secret_no",
        "/cgi-bin/service/list_device",
        "/cgi-bin/service/fetch_device_log",
        "/cgi-bin/service/del_device",
        "/cgi-bin/service/set_printer_support_state",
        // 第三方 unionid 转 external_userid 的旧 service 形态 1 条：清单出处 95327 现重定向到 90556
        // 「企业自建应用开发 > 概述」，页内无该路由；官方现行落位为 ID 转换三棵树 95900 / 97108 / 101134，
        // 本仓已声明于 /cgi-bin/idconvert/unionid_to_external_userid。
        "/cgi-bin/service/externalcontact/unionid_to_external_userid_3rd",
        // 家校健康码 3 条（2026-10-18 复核，追踪号 P2-QY-SCHOOL-HEALTHQRCODE-01）：现行目录树全量锚点
        // （1017 条，覆盖自建 / 第三方 / 代开发三棵应用类型树）内检索「健康码」「健康」「qrcode」，
        // 命中的仅有健康上报三支（93676/93677/93678，已落位于 /cgi-bin/health/*）与「获取加入企业二维码」
        // 「获取『学校通知』二维码」——无任何健康码或自定义健康信息页面。
        // 同前缀的 student 族页面（92325~92338）在册且正文可取到请求地址（见本守卫正对照 update_student），
        // ⇒ 树本身可解析、路由抽取可用，缺页即官方下架，非「未渲染」误判。
        "/cgi-bin/school/user/get_health_qrcode",
        "/cgi-bin/school/user/get_student_customize_health_info",
        "/cgi-bin/school/user/get_teacher_customize_health_info",
        // 收款商户号增删 2 条（2026-10-18 复核，追踪号 P2-QY-EXTERNALPAY-MERCHANTCRUD-01）：现行 93666
        // 「收款商户号管理」（最后更新 2024-07-18，上篇 93665 / 下篇 93667，在树内）正文的请求地址
        // 仅 /cgi-bin/externalpay/getmerchant 与 /cgi-bin/externalpay/set_mch_use_scope 两支，
        // addmerchant / delmerchant 小节已消失 ⇒ 按「页面在树内但该端点小节已消失」同判非契约；
        // 页内现存两支本仓均已落位（见正对照），故本条不是「整页未渲染」的假阴性。
        "/cgi-bin/externalpay/addmerchant",
        "/cgi-bin/externalpay/delmerchant",
        // 硬件旧面 1 条（2026-10-18 复核，追踪号 P2-QY-HARDWARE-DEVICEFEATURE-01）：清单出处 92739
        // 「获取设备自定义参数」标题仍在、但 SSR 正文已无任何「请求方式 / 请求地址」小节与参数表
        //（同页取法在正对照 95982 可完整取到路由 + 六行参数表 + 上下篇导航 ⇒ 非渲染假阴性），
        // 且无上下篇链接、不在现行目录树内；硬件面现行契约是 /cgi-bin/openhw/*（凭证为
        // model_access_token / device_access_token，已整域落位于 IWechatWorkOpenHardwareService）。
        "/cgi-bin/hardware/get_device_feature",
        // 等价现行路由 3 条：旧路由名 / 旧落位，本仓只声明现行路由。
        "/cgi-bin/user/getuserinfo",
        "/cgi-bin/externalcontact/unionid_to_external_userid",
        // 缺 auth/ 段的旧形态（第三方 SDK 转述遗留）：官方树内 91122 与脱树 legacy /document/15123
        // 两处正文的请求地址都是 /cgi-bin/service/auth/getuserdetail3rd，本仓已落位于该形态。
        "/cgi-bin/service/getuserdetail3rd",
    };

    /// <summary>
    /// 正对照路由（本仓<b>已落位</b>的相邻契约路由）：必须被同一趟扫描捕获，
    /// 否则正则或扫描根漂移会让「零声明」断言静默恒真。
    /// </summary>
    private static readonly string[] LandedControlRoutes =
    {
        // 汇报域已落位 3 条（与 export_doc / get_export_doc_result 同前缀，扫描根与正则的活体证明）。
        "/cgi-bin/oa/journal/get_record_list",
        "/cgi-bin/oa/journal/get_record_detail",
        "/cgi-bin/oa/journal/get_stat_list",
        // 消息推送与通讯录成员域的已落位路由（覆盖 message / user 两个本守卫涉猎的前缀）。
        "/cgi-bin/message/send",
        "/cgi-bin/user/get",
        // 网页授权登录的现行路由（旧路由名 getuserinfo 的替代者）。
        "/cgi-bin/auth/getuserinfo",
        // ID 转换的现行落位（externalcontact/unionid_to_external_userid 的替代者）。
        "/cgi-bin/idconvert/unionid_to_external_userid",
        // 应用管理·自建应用迁移的现行落位（migrate_to_customized_app 的替代者，同前缀活体证明）。
        "/cgi-bin/agent/claim_customized_app",
        // 会议室预定管理的现行落位（get_booking_info_by_meeting_id 的替代者，同前缀活体证明）。
        "/cgi-bin/oa/meetingroom/get_booking_info",
        // 微信客服·客户基础信息域的现行落位（get_corp_qualification 同域同前缀活体证明）。
        "/cgi-bin/kf/customer/batchget",
        // 应用管理员列表的现行落位（service/get_admin_list 的替代者，同能力正对照）。
        "/cgi-bin/agent/get_admin_list",
        // service/ 前缀的活体证明：本守卫新增条目全部落在该前缀下，须有同前缀已声明路由作实扫证据
        //（服务商授权编排域与账号 ID 域各一条）。
        "/cgi-bin/service/get_auth_info",
        "/cgi-bin/service/corpid_to_opencorpid",
        // service/externalcontact/ 前缀的活体证明（unionid_to_external_userid_3rd 的同前缀正对照）。
        "/cgi-bin/service/externalcontact/finish_external_userid_migration",
        // 第三方敏感信息的现行路由（service/getuserdetail3rd 旧形态的替代者，同前缀活体证明）。
        "/cgi-bin/service/auth/getuserdetail3rd",
        // 家校通讯录学生族的现行落位（健康码三支的同前缀活体证明：同一 school/user/ 前缀、同一文档树，
        // 既证扫描根覆盖该目录，也证「健康码页面缺失」不是路由抽取失败的假象）。
        "/cgi-bin/school/user/update_student",
        // 收款商户号管理的现行两支（addmerchant / delmerchant 小节消失后同页仅有的路由，同前缀活体证明）。
        "/cgi-bin/externalpay/getmerchant",
        "/cgi-bin/externalpay/set_mch_use_scope",
        // 智能硬件开发·硬件云端接入的现行落位（hardware/get_device_feature 旧面的替代族，
        // 同能力正对照：设备详情端点即 92739 复核时用于自证「取法可用」的对照页）。
        "/cgi-bin/openhw/device/get_device_detail",
    };

    // ------------------------------------------------------------------
    // RD1：35 条非契约路由在 Src/ 全部源码内零声明（扫描以已落位路由作正对照）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 RD1：非契约路由清单锁定 + <c>Src/</c> 全量扫描零命中 + 已落位路由正对照。</summary>
    [Fact]
    public void NonContractRoutes_ShouldHaveZeroDeclaredRoutes_WhenScannedAcrossSrc()
    {
        NonContractRoutes.Should().HaveCount(35,
            "官方目录已下架 31 + 本仓已有等价现行路由 3：清单本身即契约，增删须先复核官方文档目录树");
        NonContractRoutes.Distinct(StringComparer.Ordinal).Should().HaveCount(35, "清单内路由不得重复");

        var routeRegex = new Regex(@"\[(?:Get|Post|Put|Delete|Patch)\(""([^""]+)""\)\]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var forbidden = new HashSet<string>(NonContractRoutes, StringComparer.Ordinal);

        var declared = new HashSet<string>(StringComparer.Ordinal);
        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(GetSolutionRoot(), "Src"), "*.cs",
                     SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal)
                || normalized.Contains("/Generated/", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (System.Text.RegularExpressions.Match match in routeRegex.Matches(File.ReadAllText(file)))
            {
                var route = match.Groups[1].Value;
                declared.Add(route);
                if (forbidden.Contains(route))
                {
                    offenders.Add($"{normalized}: {route}");
                }
            }
        }

        // 扫描非空跑正对照：已落位的相邻契约路由必须被同一趟扫描捕获。
        LandedControlRoutes.Should().OnlyContain(
            route => declared.Contains(route),
            "RD1 的扫描必须能捕获本仓已声明的相邻路由（否则正则/扫描根漂移会让零声明断言静默恒真）");

        offenders.Should().BeEmpty(
            "35 条路由不构成应用侧 HTTP 契约（官方文档目录已移除或本仓已有等价现行路由），" +
            "本仓不得在任何产品线接口上声明（追踪号 P2-QY-SKIT-CATALOG-VERIFY-03）");
    }

    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }
}
